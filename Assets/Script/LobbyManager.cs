namespace Coherence.Samples.Kien
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Cloud;
    using Coherence.Toolkit;
    using Connection;
    using UnityEngine;

    public class LobbyManager : MonoBehaviour
    {
        private CoherenceBridge bridge;
        private ReplicationServerRoomsService replicationServerRoomsService;
        private CloudRooms cloudRooms;
        private CloudRoomsService cloudRoomsService;
        private IRoomsService activeRoomsService;
        private bool isLocalRoomsServiceOnline;
        private CancellationTokenSource cloudModeCancellationTokenSource;
        private CancellationTokenSource localModeCancellationTokenSource;
        private IReadOnlyList<string> cloudRegionOptions = Array.Empty<string>();
        private Coroutine localToggleRefresher;
        private CloudState cloudState = CloudState.Default;
        private LocalState localState = LocalState.Default;
        public bool IsCloudModeEnabled { get; private set; } = true;

        public event Action<RequestResponse<IReadOnlyList<RoomData>>> OnRoomsFetched;
        public event Action<RequestResponse<RoomData>> OnRoomCreated;
        public event Action<RequestResponse<IReadOnlyList<string>>> OnRegionsChanged;
        public event Action<ConnectionException> OnConnectionError;
        public event Action OnBridgeConnected;
        public event Action OnBridgeDisconnected;

        private void Awake()
        {
            if (!CoherenceBridgeStore.TryGetBridge(gameObject.scene, out bridge))
            {
                Debug.LogError($"{nameof(CoherenceBridge)} required on the scene.\n" +
                               "Add one via 'GameObject > coherence > Bridge'.", this);
                return;
            }

            replicationServerRoomsService = new ReplicationServerRoomsService();
            bridge.onConnected.AddListener((_) => { OnBridgeConnected?.Invoke(); });
            bridge.onDisconnected.AddListener((_, _) => {OnBridgeDisconnected?.Invoke();});
            bridge.onConnectionError.AddListener((_, e) => {OnConnectionError?.Invoke(e);});
            localToggleRefresher = StartCoroutine(LocalToggleRefresher());
        }

        private void OnDestroy()
        {
            replicationServerRoomsService?.Dispose();
            cloudModeCancellationTokenSource?.Dispose();
            localModeCancellationTokenSource?.Dispose();
            if (localToggleRefresher != null)
            {
                StopCoroutine(localToggleRefresher);
            }
        }

        public bool IsLocalServerAvailable()
        {
            return replicationServerRoomsService.IsOnline().Result;
        }

        public void SetMode(bool isLocalMode)
        {
            IsCloudModeEnabled = !isLocalMode;
            cloudModeCancellationTokenSource?.Dispose();
            cloudModeCancellationTokenSource = null;
            localModeCancellationTokenSource?.Dispose();
            localModeCancellationTokenSource = null;

            if (isLocalMode)
            {
                SetLocalState(localState);
            }
            else
            {
                SetCloudState(cloudState);
            }
        }

        public void RefreshRooms()
        {
            if (!IsSelectedRoomServiceReady())
                return;

            activeRoomsService.FetchRooms(OnRoomsFetched, null, GetOrCreateActiveCancellationToken());
        }

        public void CreateRoom(RoomCreationOptions options)
        {
            activeRoomsService?.CreateRoom(OnRoomCreated, options, GetOrCreateActiveCancellationToken());
        }

        public void JoinRoom(RoomData roomData)
        {
            bridge.JoinRoom(roomData);
        }

        public void Disconnect()
        {
            bridge.Disconnect();
        }

        public void RefreshRegions()
        {
            if (cloudRooms?.IsLoggedIn == true)
            {
                cloudRooms.RefreshRegions(OnRegionsChanged, GetOrCreateCloudModeCancellationToken());
            }
        }

        public void SelectRegion(int index)
        {
            if (cloudRooms?.IsLoggedIn != true || index >= cloudRegionOptions.Count)
                return;

            cloudRoomsService = cloudRooms.GetRoomServiceForRegion(cloudRegionOptions[index]);
            SetActiveRoomService(cloudRoomsService, isLocal: false);
            RefreshRooms();
        }

        public void SetInitialRegion()
        {
            if (cloudRegionOptions.Count > 0 && cloudRooms?.IsLoggedIn == true)
            {
                cloudRoomsService = cloudRooms.GetRoomServiceForRegion(cloudRegionOptions[0]);
                SetActiveRoomService(cloudRoomsService, isLocal: false);
                SetCloudState(CloudState.FetchingRooms);
            }
        }

        public bool HasRegions() => cloudRegionOptions.Count > 0;

        private bool IsLoggedIn => cloudRooms is { IsLoggedIn: true };
        private bool IsSelectedRoomServiceReady() => cloudRoomsService != null && IsCloudModeEnabled ? IsLoggedIn : isLocalRoomsServiceOnline;

        private CancellationToken GetOrCreateActiveCancellationToken() => GetOrCreateActiveCancellationTokenSource().Token;
        private CancellationToken GetOrCreateCloudModeCancellationToken() => GetOrCreateCloudModeCancellationTokenSource().Token;
        private CancellationTokenSource GetOrCreateActiveCancellationTokenSource() => IsCloudModeEnabled ? GetOrCreateCloudModeCancellationTokenSource() : GetOrCreateLocalModeCancellationTokenSource();
        private CancellationTokenSource GetOrCreateCloudModeCancellationTokenSource() => cloudModeCancellationTokenSource ??= new();
        private CancellationTokenSource GetOrCreateLocalModeCancellationTokenSource() => localModeCancellationTokenSource ??= new();

        private IEnumerator LocalToggleRefresher()
        {
            while (true)
            {
                var task = replicationServerRoomsService.IsOnline();
                yield return new WaitUntil(() => task.IsCompleted);

                isLocalRoomsServiceOnline = task.Result;
                if (!IsCloudModeEnabled)
                {
                    HandleLocalServerStatus(isLocalRoomsServiceOnline);
                }

                yield return new WaitForSeconds(1f);
            }
        }

        private void SetLocalState(LocalState state)
        {
            if (localState == state && !IsCloudModeEnabled)
                return;

            localState = state;
            if (IsCloudModeEnabled)
                return;

            switch (state)
            {
                case LocalState.Default:
                case LocalState.Offline:
                    SetActiveRoomService(replicationServerRoomsService, isLocal: true);
                    break;
                case LocalState.Ready:
                case LocalState.FetchingRooms:
                    SetActiveRoomService(replicationServerRoomsService, isLocal: true);
                    RefreshRooms();
                    break;
            }
        }

        private async void SetCloudState(CloudState state)
        {
            if (cloudState == state && IsCloudModeEnabled)
                return;

            cloudState = state;
            if (!IsCloudModeEnabled)
                return;

            switch (state)
            {
                case CloudState.Default:
                case CloudState.LoggingIn:
                    await LogInToCoherenceCloud();
                    break;
                case CloudState.FetchingRegions:
                    RefreshRegions();
                    break;
                case CloudState.FetchingRooms:
                    RefreshRooms();
                    break;
                case CloudState.Ready:
                    break;
            }
        }

        private async System.Threading.Tasks.Task LogInToCoherenceCloud()
        {
            if (!TryFindCoherenceCloudLogin(out var cloudLogin))
            {
                Debug.LogError("No CoherenceCloudLogin found.");
                return;
            }

            if (cloudLogin.IsLoggedIn)
            {
                cloudRooms = cloudLogin.Services.Rooms;
                SetCloudState(CloudState.FetchingRegions);
                return;
            }

            if (string.IsNullOrEmpty(RuntimeSettings.Instance.ProjectID))
            {
                Debug.LogError("No project selected.");
                return;
            }

            var loginOperation = await cloudLogin.LogInAsync(GetOrCreateCloudModeCancellationToken());
            if (loginOperation.IsCompletedSuccessfully)
            {
                cloudRooms = loginOperation.Result.Services.Rooms;
                SetCloudState(CloudState.FetchingRegions);
            }
            else if (loginOperation.HasFailed)
            {
                DialogUI.GetErrorFromResponse(new RequestResponse<object> { Exception = loginOperation.Error });
            }
        }

        private bool TryFindCoherenceCloudLogin(out CoherenceCloudLogin cloudLogin)
        {
            cloudLogin = FindAnyObjectByType<CoherenceCloudLogin>(FindObjectsInactive.Exclude);
            return cloudLogin != null;
        }

        private void HandleLocalServerStatus(bool isLocalRoomsServiceOnline)
        {
            if (isLocalRoomsServiceOnline)
            {
                SetLocalState(LocalState.Ready);
            }
            else
            {
                SetLocalState(LocalState.Offline);
            }
        }

        private void SetActiveRoomService(IRoomsService service, bool isLocal)
        {
            if (activeRoomsService == service)
                return;

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
}