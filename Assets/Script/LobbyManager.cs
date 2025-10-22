// using System.Threading;
// using Coherence.Cloud;
// using UnityEngine;

// public class LobbyManager : MonoBehaviour
// {
//     [System.Serializable]
//     public class Room
//     {
//         public string id;
//         public string name;
//         public int playerCount;
//         public int maxPlayers;
//     }
//     private IRoomsService activeRoomsService;
//     private CancellationTokenSource localModeCancellationTokenSource;
//     private void CreateRoom(Room room)
//     {
//         var options = RoomCreationOptions.Default;
//         options.KeyValues.Add(RoomData.RoomNameKey, room.name);
//         options.MaxClients = room.maxPlayers;
//         activeRoomsService?.CreateRoom(OnRoomCreated, options, GetOrCreateActiveCancellationToken());
//         //HideCreateRoomPanel();
//     }
//     private void OnRoomCreated(RequestResponse<RoomData> requestResponse)
//     {
//         // if (requestResponse.Status != RequestStatus.Success)
//         // {
//         //     joinNextCreatedRoom = false;

//         //     var errorMessage = GetErrorFromResponse(requestResponse);
//         //     ShowError("Error creating room", errorMessage);
//         //     Debug.LogException(requestResponse.Exception);
//         //     return;
//         // }

//         // var createdRoom = requestResponse.Result;
//         // if (joinNextCreatedRoom)
//         // {
//         //     joinNextCreatedRoom = false;
//         //     JoinRoom(createdRoom);
//         // }
//         // else
//         // {
//         //     lastCreatedRoomUid = createdRoom.UniqueId;
//         //     RefreshRooms();
//         // }
//     }
//     private CancellationToken GetOrCreateActiveCancellationToken() => GetOrCreateActiveCancellationTokenSource().Token;
//     private CancellationTokenSource GetOrCreateActiveCancellationTokenSource() => GetOrCreateLocalModeCancellationTokenSource();
//     private CancellationTokenSource GetOrCreateLocalModeCancellationTokenSource() => localModeCancellationTokenSource ??= new();
// }
// Copyright (c) coherence ApS.
// See the license file in the package root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coherence.Cloud;
using Coherence.Connection;
using Coherence.Runtime;
using Coherence.Toolkit;
using UnityEngine;


public class LobbyManager : MonoBehaviour
{
    private CoherenceBridge bridge;
    private CloudRooms cloudRooms;
    private CloudRoomsService cloudRoomsService;
    private ReplicationServerRoomsService replicationServerRoomsService;
    private IRoomsService activeRoomsService;
    private bool isLocalRoomsServiceOnline;
    private bool isCloudModeEnabled = true; // Mặc định là Cloud mode
    private CancellationTokenSource cloudModeCancellationTokenSource;
    private CancellationTokenSource localModeCancellationTokenSource;
    private IReadOnlyList<string> cloudRegionOptions = Array.Empty<string>();
    private IReadOnlyList<RoomData> rooms = Array.Empty<RoomData>();
    private bool joinNextCreatedRoom;
    private ulong lastCreatedRoomUid;
    private CloudState cloudState = CloudState.Default;
    private LocalState localState = LocalState.Default;
    // Properties
    private bool IsLoggedIn => cloudRooms is { IsLoggedIn: true };
    private bool IsSelectedRoomServiceReady => cloudRoomsService != null && isCloudModeEnabled ? IsLoggedIn : isLocalRoomsServiceOnline;

    // Events để thông báo cho UI hoặc hệ thống khác
    public Action<IReadOnlyList<RoomData>> OnRoomsUpdated;
    public Action<string> OnError;
    public Action<bool> OnConnectionStatusChanged; // true = connected, false = disconnected

    private async void Start()
    {
        // Kiểm tra và lấy CoherenceBridge
        if (!CoherenceBridgeStore.TryGetBridge(gameObject.scene, out bridge))
        {
            OnError?.Invoke("No CoherenceBridge found. Add via 'GameObject > coherence > Bridge'.");
            return;
        }

        replicationServerRoomsService = new ReplicationServerRoomsService();
        bridge.onConnected.AddListener(OnBridgeConnected);
        bridge.onDisconnected.AddListener(OnBridgeDisconnected);
        bridge.onConnectionError.AddListener(OnConnectionError);

        // Kiểm tra trạng thái local replication server
        isLocalRoomsServiceOnline = await replicationServerRoomsService.IsOnline();
        if (isLocalRoomsServiceOnline)
        {
            SetMode(false); // Chuyển sang Local mode nếu có server
        }
        else
        {
            SetMode(true); // Giữ Cloud mode và login
        }

        // Bắt đầu coroutine kiểm tra trạng thái local server
        StartCoroutine(LocalToggleRefresher());
    }

    private void OnDestroy()
    {
        replicationServerRoomsService?.Dispose();
        cloudModeCancellationTokenSource?.Dispose();
        localModeCancellationTokenSource?.Dispose();

        if (bridge)
        {
            bridge.onConnected.RemoveListener(OnBridgeConnected);
            bridge.onDisconnected.RemoveListener(OnBridgeDisconnected);
            bridge.onConnectionError.RemoveListener(OnConnectionError);
        }
    }

    // Chuyển đổi giữa Cloud và Local mode
    public void SetMode(bool cloudMode)
    {
        isCloudModeEnabled = cloudMode;
        cloudModeCancellationTokenSource?.Dispose();
        cloudModeCancellationTokenSource = null;
        localModeCancellationTokenSource?.Dispose();
        localModeCancellationTokenSource = null;

        if (cloudMode)
        {
            SetCloudState(CloudState.LoggingIn);
        }
        else
        {
            SetLocalState(isLocalRoomsServiceOnline ? LocalState.Ready : LocalState.Offline);
        }
    }

    // Login vào Coherence Cloud
    private async Task LogInToCoherenceCloud()
    {
        if (!TryFindCoherenceCloudLogin(out var cloudLogin))
        {
            OnError?.Invoke("No CoherenceCloudLogin found.");
            return;
        }

        if (cloudLogin.IsLoggedIn)
        {
            cloudRooms = cloudLogin.Services.Rooms;
            SetCloudState(CloudState.FetchingRegions);
            return;
        }

        // if (string.IsNullOrEmpty(RuntimeSettings.Instance.ProjectID))
        // {
        //     OnError?.Invoke("No project selected. Set in coherence > Hub > Cloud.");
        //     return;
        // }

        try
        {
            var loginOperation = await cloudLogin.LogInAsync(GetOrCreateCloudModeCancellationToken());
            if (loginOperation.IsCompletedSuccessfully)
            {
                cloudRooms = loginOperation.Result.Services.Rooms;
                SetCloudState(CloudState.FetchingRegions);
            }
            else
            {
                //OnError?.Invoke(GetLoginErrorMessage(loginOperation.Error));
            }
        }
        catch (Exception e)
        {
            OnError?.Invoke($"Login failed: {e.Message}");
        }
    }

    // Làm mới danh sách rooms
    public void RefreshRooms()
    {
        if (!IsSelectedRoomServiceReady)
        {
            OnError?.Invoke("Room service not ready.");
            return;
        }

        activeRoomsService.FetchRooms(OnRoomsFetched, null, GetOrCreateActiveCancellationToken());
    }

    // Tạo room mới
    public void CreateRoom(string roomName, int maxPlayers = 10)
    {
        var options = RoomCreationOptions.Default;
        options.KeyValues.Add(RoomData.RoomNameKey, roomName);
        options.MaxClients = maxPlayers;
        activeRoomsService?.CreateRoom(OnRoomCreated, options, GetOrCreateActiveCancellationToken());
    }

    // Tạo và tham gia room ngay
    public void CreateAndJoinRoom(string roomName, int maxPlayers = 10)
    {
        joinNextCreatedRoom = true;
        CreateRoom(roomName, maxPlayers);
    }

    // Tham gia room
    public void JoinRoom(RoomData roomData)
    {
        if (roomData.UniqueId == default)
        {
            OnError?.Invoke("Invalid room selected.");
            return;
        }
        bridge.JoinRoom(roomData);
    }

    // Làm mới danh sách regions (Cloud mode)
    public void RefreshCloudRoomsRegions()
    {
        if (!IsLoggedIn)
        {
            OnError?.Invoke("Not logged in to Coherence Cloud.");
            return;
        }
        cloudRooms.RefreshRegions(OnCloudRoomsRegionsChanged, GetOrCreateCloudModeCancellationToken());
    }

    // Rời kết nối
    public void Disconnect()
    {
        bridge.Disconnect();
    }

    // Coroutine kiểm tra trạng thái local server
    private IEnumerator LocalToggleRefresher()
    {
        while (true)
        {
            var task = replicationServerRoomsService.IsOnline();
            yield return new WaitUntil(() => task.IsCompleted);
            isLocalRoomsServiceOnline = task.Result;
            HandleLocalServerStatus(isLocalRoomsServiceOnline);
            yield return new WaitForSeconds(1f);
        }
    }

    // Xử lý trạng thái local server
    private void HandleLocalServerStatus(bool isOnline)
    {
        if (!isCloudModeEnabled && (localState is LocalState.Default or LocalState.Ready or LocalState.Offline))
        {
            SetLocalState(isOnline ? LocalState.Ready : LocalState.Offline);
        }

        if (isOnline)
        {
            SetActiveRoomService(replicationServerRoomsService, isLocal: true);
        }
    }

    // Quản lý trạng thái Local
    private void SetLocalState(LocalState state)
    {
        if (localState == state && !isCloudModeEnabled) return;
        localState = state;

        if (isCloudModeEnabled) return;

        switch (state)
        {
            case LocalState.Offline:
                OnError?.Invoke("No local replication server found.");
                break;
            case LocalState.Ready:
                SetActiveRoomService(replicationServerRoomsService, isLocal: true);
                RefreshRooms();
                break;
            case LocalState.FetchingRooms:
                RefreshRooms();
                break;
        }
    }

    // Quản lý trạng thái Cloud
    private async void SetCloudState(CloudState state)
    {
        if (cloudState == state && isCloudModeEnabled) return;
        cloudState = state;

        if (!isCloudModeEnabled) return;

        switch (state)
        {
            case CloudState.Default:
                SetCloudState(CloudState.LoggingIn);
                break;
            case CloudState.LoggingIn:
                await LogInToCoherenceCloud();
                break;
            case CloudState.FetchingRegions:
                RefreshCloudRoomsRegions();
                break;
            case CloudState.FetchingRooms:
                RefreshRooms();
                break;
            case CloudState.Ready:
                OnRoomsUpdated?.Invoke(rooms);
                break;
        }
    }

    // Chọn service (Cloud hoặc Local)
    private void SetActiveRoomService(IRoomsService service, bool isLocal)
    {
        if (activeRoomsService == service) return;
        activeRoomsService = service;

        if (isLocal)
        {
            if (isLocalRoomsServiceOnline)
            {
                SetLocalState(LocalState.FetchingRooms);
            }
        }
        else if (!IsLoggedIn)
        {
            SetCloudState(CloudState.LoggingIn);
        }
        else if (cloudRegionOptions.Count == 0)
        {
            SetCloudState(CloudState.FetchingRegions);
        }
        else
        {
            SetCloudState(CloudState.FetchingRooms);
        }
    }

    // Callback khi rooms được fetch
    private void OnRoomsFetched(RequestResponse<IReadOnlyList<RoomData>> requestResponse)
    {
        if (requestResponse.Status != RequestStatus.Success)
        {
            OnError?.Invoke(GetErrorFromResponse(requestResponse));
            rooms = Array.Empty<RoomData>();
            OnRoomsUpdated?.Invoke(rooms);
            return;
        }

        rooms = requestResponse.Result;
        OnRoomsUpdated?.Invoke(rooms);

        if (isCloudModeEnabled)
            SetCloudState(CloudState.Ready);
        else
            SetLocalState(LocalState.Ready);
    }

    // Callback khi tạo room
    private void OnRoomCreated(RequestResponse<RoomData> requestResponse)
    {
        if (requestResponse.Status != RequestStatus.Success)
        {
            joinNextCreatedRoom = false;
            OnError?.Invoke(GetErrorFromResponse(requestResponse));
            return;
        }

        var createdRoom = requestResponse.Result;
        if (joinNextCreatedRoom)
        {
            joinNextCreatedRoom = false;
            JoinRoom(createdRoom);
        }
        else
        {
            lastCreatedRoomUid = createdRoom.UniqueId;
            RefreshRooms();
        }
    }

    // Callback khi regions được làm mới
    private void OnCloudRoomsRegionsChanged(RequestResponse<IReadOnlyList<string>> requestResponse)
    {
        if (requestResponse.Status != RequestStatus.Success)
        {
            if (cloudRegionOptions.Count > 0)
            {
                SetCloudState(CloudState.Ready);
            }
            else
            {
                OnError?.Invoke("No cloud regions available.");
            }
            OnError?.Invoke(GetErrorFromResponse(requestResponse));
            return;
        }

        cloudRegionOptions = requestResponse.Result;
        if (cloudRegionOptions.Count > 0 && isCloudModeEnabled && IsLoggedIn)
        {
            cloudRoomsService = cloudRooms.GetRoomServiceForRegion(cloudRegionOptions[0]);
            SetActiveRoomService(cloudRoomsService, isLocal: false);
            SetCloudState(CloudState.FetchingRooms);
        }
    }

    // Callback khi kết nối thay đổi
    private void OnBridgeConnected(CoherenceBridge _) => OnConnectionStatusChanged?.Invoke(true);
    private void OnBridgeDisconnected(CoherenceBridge _, ConnectionCloseReason _x) => OnConnectionStatusChanged?.Invoke(false);
    private void OnConnectionError(CoherenceBridge _, ConnectionException exception)
    {
        var (title, message) = exception.GetPrettyMessage();
        OnError?.Invoke($"{title}: {message}");
        RefreshRooms();
    }

    // Tìm CoherenceCloudLogin
    private bool TryFindCoherenceCloudLogin(out CoherenceCloudLogin cloudLogin)
    {
        cloudLogin = FindAnyObjectByType<CoherenceCloudLogin>(FindObjectsInactive.Exclude);
        return cloudLogin != null;
    }

    // Lấy cancellation token
    private CancellationToken GetOrCreateActiveCancellationToken() => GetOrCreateActiveCancellationTokenSource().Token;
    private CancellationToken GetOrCreateCloudModeCancellationToken() => GetOrCreateCloudModeCancellationTokenSource().Token;
    private CancellationTokenSource GetOrCreateActiveCancellationTokenSource() => isCloudModeEnabled ? GetOrCreateCloudModeCancellationTokenSource() : GetOrCreateLocalModeCancellationTokenSource();
    private CancellationTokenSource GetOrCreateCloudModeCancellationTokenSource() => cloudModeCancellationTokenSource ??= new();
    private CancellationTokenSource GetOrCreateLocalModeCancellationTokenSource() => localModeCancellationTokenSource ??= new();

    // Lấy thông báo lỗi từ login
    // private static string GetLoginErrorMessage(LoginError error)
    // {
    //     return error.Type switch
    //     {
    //         LoginErrorType.SchemaNotFound => "Local schema not uploaded. Use coherence > Upload Schema.",
    //         LoginErrorType.NoProjectSelected => "No project selected. Set in coherence > Hub > Cloud.",
    //         LoginErrorType.ServerError => "Server error during login.",
    //         LoginErrorType.InvalidCredentials => "Invalid credentials provided.",
    //         LoginErrorType.InvalidResponse => "Unable to deserialize server response.",
    //         LoginErrorType.TooManyRequests => "Too many requests. Try again later.",
    //         LoginErrorType.ConnectionError => "Connection failure.",
    //         LoginErrorType.AlreadyLoggedIn => "Already logged in. Logout first.",
    //         LoginErrorType.ConcurrentConnection => "Concurrent connection detected. Another instance is running.",
    //         LoginErrorType.InvalidConfig => "Invalid configuration in Coherence Dashboard.",
    //         LoginErrorType.OneTimeCodeExpired => "One-time code expired.",
    //         LoginErrorType.OneTimeCodeNotFound => "No account linked to authentication method.",
    //         LoginErrorType.IdentityLimit => "Identity limit reached.",
    //         LoginErrorType.IdentityNotFound => "Identity not found.",
    //         LoginErrorType.IdentityTaken => "Identity already linked to another account.",
    //         LoginErrorType.IdentityTotalLimit => "Maximum identity limit reached.",
    //         LoginErrorType.InvalidInput => "Invalid input provided.",
    //         LoginErrorType.PasswordNotSet => "Password not set for player account.",
    //         LoginErrorType.UsernameNotAvailable => "Username already taken.",
    //         LoginErrorType.InternalException => "Internal exception occurred.",
    //         _ => error.Message,
    //     };
    // }

    // Lấy thông báo lỗi từ request
    private static string GetErrorFromResponse<T>(RequestResponse<T> requestResponse)
    {
        if (requestResponse.Exception is not RequestException requestException)
            return "Unknown error.";

        return requestException.ErrorCode switch
        {
            ErrorCode.InvalidCredentials => "Invalid authentication credentials, please login again.",
            ErrorCode.TooManyRequests => "Too many requests. Try again later.",
            ErrorCode.ProjectNotFound => "Project not found. Check runtime key.",
            ErrorCode.SchemaNotFound => "Schema not found. Ensure schema matches replication server.",
            ErrorCode.RSVersionNotFound => "Replication server version not found.",
            ErrorCode.SimNotFound => "Simulator not found. Check slug and schema.",
            ErrorCode.MultiSimNotListening => "Multi-room simulator not listening on required ports.",
            ErrorCode.RoomsSimulatorsNotEnabled => "Simulators not enabled in Coherence Dashboard.",
            ErrorCode.RoomsSimulatorsNotUploaded => "Simulator not uploaded. Use coherence Hub.",
            ErrorCode.RoomsVersionNotFound => "Version not found. Check sim-slug.",
            ErrorCode.RoomsSchemaNotFound => "Schema not found for replication server.",
            ErrorCode.RoomsRegionNotFound => "Region not found. Check Dev Portal.",
            ErrorCode.RoomsInvalidTagOrKeyValueEntry => "Invalid tag or key/value entries.",
            ErrorCode.RoomsCCULimit => "Room CCU limit exceeded.",
            ErrorCode.RoomsNotFound => "Room not found. Refresh room list.",
            ErrorCode.RoomsInvalidSecret => "Invalid room secret.",
            ErrorCode.RoomsInvalidMaxPlayers => "Room max players must be between 1 and configured limit.",
            ErrorCode.InvalidMatchMakingConfig => "Invalid matchmaking configuration.",
            ErrorCode.ClientPermission => "Client restricted from this feature.",
            ErrorCode.CreditLimit => "Monthly credit limit exceeded.",
            ErrorCode.InDeployment => "Resources are being provisioned. Retry later.",
            ErrorCode.FeatureDisabled => "Feature disabled in Coherence Dashboard.",
            ErrorCode.InvalidRoomLimit => "Room max players must be between 1 and 100.",
            _ => requestException.Message,
        };
    }

    private enum CloudState
    {
        Default,
        LoggingIn,
        FetchingRegions,
        FetchingRooms,
        Ready
    }

    private enum LocalState
    {
        Default,
        Offline,
        FetchingRooms,
        Ready
    }
}
