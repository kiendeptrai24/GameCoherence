namespace Coherence.Samples.Kien
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cloud;
    using Connection;
    using UnityEngine;

    public class RoomPresenter : MonoBehaviour
    {
        private RoomView view;
        private LobbyManager lobbyManager;
        private UIState onlineModeUIState = UIState.LoadingSpinner;
        private UIState localModeUIState = UIState.LoadingSpinner;
        private bool wasCloudModeEnabled = true;
        private bool joinNextCreatedRoom;
        private ulong lastCreatedRoomUid;
        private void Awake() {
            // Add component
            lobbyManager = FindAnyObjectByType<LobbyManager>();
            view = GetComponent<RoomView>();

            // Subscribe to view events
            view.OnModeChanged += OnModeChanged;
            view.OnRefreshRoomsClicked += OnRefreshRoomsClicked;
            view.OnJoinRoomClicked += OnJoinRoomClicked;
            view.OnShowCreateRoomPanelClicked += ShowCreateRoomPanel;
            view.OnHideCreateRoomPanelClicked += HideCreateRoomPanel;
            view.OnCreateAndJoinRoomClicked += CreateRoomAndJoin;
            view.OnRefreshRegionsClicked += RefreshCloudRoomsRegions;
            view.OnRegionSelectionChanged += OnRegionSelectionChanged;
            view.OnDisconnectClicked += lobbyManager.Disconnect;
            view.OnPopupDismissClicked += view.HideError;

            // Subscribe to lobby manager events
            lobbyManager.OnRoomsFetched += OnRoomsFetched;
            lobbyManager.OnRoomCreated += OnRoomCreated;
            lobbyManager.OnRegionsChanged += OnRegionsChanged;
            lobbyManager.OnConnectionError += OnConnectionError;
            lobbyManager.OnBridgeConnected += () => view.UpdateDialogsVisibility(true);
            lobbyManager.OnBridgeDisconnected += () => view.UpdateDialogsVisibility(false);
        }

        public void Initialize()
        {
            view.UpdateUIState(UIState.LoadingSpinner);
            if (lobbyManager.IsLocalServerAvailable())
            {
                OnModeChanged(true); // Default to local mode if server is available
            }
            else
            {
                OnModeChanged(false); // Default to cloud mode
            }
        }

        private void OnModeChanged(bool localMode)
        {
            view.UpdateModeUI(localMode);
            lobbyManager.SetMode(localMode);
            view.UpdateUIState(localMode ? localModeUIState : onlineModeUIState);
        }

        private void OnRefreshRoomsClicked()
        {
            lobbyManager.RefreshRooms();
        }

        private void OnJoinRoomClicked()
        {
            if (!view.HasPlayerSelected())
            {
                view.ShowError("Error", "Please choose player before joining room");
                return;
            }
            RoomData selectedRoom = view.GetSelectedRoom();

            lobbyManager.JoinRoom(selectedRoom);
            view.UpdateUIState(UIState.LoadingSpinner);

        }

        private void ShowCreateRoomPanel()
        {
            view.UpdateUIState(UIState.CreatingRoom);
        }

        private void HideCreateRoomPanel()
        {
            view.UpdateUIState(lobbyManager.IsCloudModeEnabled ? onlineModeUIState : localModeUIState);
        }

        private void CreateRoomAndJoin()
        {
            if (!view.HasPlayerSelected())
            {
                view.ShowError("Error", "Please choose player before creating room");
                return;
            }
            joinNextCreatedRoom = true;
            CreateRoom();
        }

        private void CreateRoom()
        {
            var options = RoomCreationOptions.Default;
            options.KeyValues.Add(RoomData.RoomNameKey, view.GetRoomName());
            options.MaxClients = view.GetRoomMaxPlayers();
            lobbyManager.CreateRoom(options);
            HideCreateRoomPanel();
        }

        private void RefreshCloudRoomsRegions()
        {
            lobbyManager.RefreshRegions();
            view.UpdateUIState(UIState.LoadingSpinner);
        }

        private void OnRegionSelectionChanged(int index)
        {
            lobbyManager.SelectRegion(index);
            view.UpdateUIState(UIState.LoadingSpinner);
        }

        private void OnRoomsFetched(RequestResponse<IReadOnlyList<RoomData>> response)
        {
            view.SetRoomSectionButtonInteractability(true);
            if (response.Status != RequestStatus.Success)
            {
                view.ClearRoomList();
                view.ShowError("Error fetching rooms", DialogUI.GetErrorFromResponse(response));
                view.UpdateUIState(UIState.NoRoomsExist);
                return;
            }

            var rooms = response.Result;
            view.UpdateRoomList(rooms, lastCreatedRoomUid);
            lastCreatedRoomUid = 0;
            view.UpdateUIState(rooms.Count > 0 ? UIState.Ready : UIState.NoRoomsExist);
        }

        private void OnRoomCreated(RequestResponse<RoomData> response)
        {
            if (response.Status != RequestStatus.Success)
            {
                joinNextCreatedRoom = false;
                view.ShowError("Error creating room", DialogUI.GetErrorFromResponse(response));
                return;
            }

            var createdRoom = response.Result;
            if (joinNextCreatedRoom)
            {
                joinNextCreatedRoom = false;
                lobbyManager.JoinRoom(createdRoom);
            }
            else
            {
                lastCreatedRoomUid = createdRoom.UniqueId;
                lobbyManager.RefreshRooms();
            }
        }

        private void OnRegionsChanged(RequestResponse<IReadOnlyList<string>> response)
        {
            if (response.Status != RequestStatus.Success)
            {
                view.ShowError("Error refreshing regions", DialogUI.GetErrorFromResponse(response));
                view.UpdateUIState(lobbyManager.HasRegions() ? UIState.Ready : UIState.NoCloudRegionsAvailable);
                return;
            }

            view.UpdateRegionDropdown(response.Result);
            lobbyManager.SetInitialRegion();
            view.UpdateUIState(UIState.Ready);
        }

        private void OnConnectionError(ConnectionException exception)
        {
            var (title, message) = exception.GetPrettyMessage();
            view.ShowError(title, message);
            view.UpdateUIState(lobbyManager.IsCloudModeEnabled ? onlineModeUIState : localModeUIState);
            lobbyManager.RefreshRooms();
        }
    }
}