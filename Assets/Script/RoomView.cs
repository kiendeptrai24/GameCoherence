namespace Coherence.Samples.Kien
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cloud;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class RoomView : MonoBehaviour
    {
        [Header("References")]
        public GameObject dashBoard;
        public GameObject connectDialog;
        public GameObject disconnectDialog;
        public GameObject createRoomPanel;
        public GameObject regionSection;
        public GameObject noRSPlaceholder;
        public GameObject noCloudPlaceholder;
        public GameObject noRoomsAvailable;
        public GameObject loadingSpinner;
        public GameObject noBridgeFound;
        public GameObject noProjectSelected;
        public GameObject noCloudLoginFound;
        public GameObject noCloudRegionsAvailable;
        public Font boldFont;
        public Font normalFont;
        public Text cloudText;
        public Text lanText;
        public Text joinRoomTitleText;
        public ConnectDialogRoomView templateRoomView;
        public InputField roomNameInputField;
        public InputField playerNameInputField;
        public Toggle lanOnlineToggle;
        public InputField roomLimitInputField;
        public Dropdown regionDropdown;
        public Button refreshRegionsButton;
        public Button refreshRoomsButton;
        public Button joinRoomButton;
        public Button showCreateRoomPanelButton;
        public Button hideCreateRoomPanelButton;
        public Button createAndJoinRoomButton;
        public Button disconnectButton;
        public GameObject popupDialog;
        public Text popupTitleText;
        public Text popupText;
        public Button popupDismissButton;

        private ListView roomsListView;
        private string initialJoinRoomTitle;

        // Events for RoomPresenter to subscribe to
        public event UnityEngine.Events.UnityAction<bool> OnModeChanged;
        public event UnityEngine.Events.UnityAction OnRefreshRoomsClicked;
        public event UnityEngine.Events.UnityAction OnJoinRoomClicked;
        public event UnityEngine.Events.UnityAction OnShowCreateRoomPanelClicked;
        public event UnityEngine.Events.UnityAction OnHideCreateRoomPanelClicked;
        public event UnityEngine.Events.UnityAction OnCreateAndJoinRoomClicked;
        public event UnityEngine.Events.UnityAction OnRefreshRegionsClicked;
        public event UnityEngine.Events.UnityAction<int> OnRegionSelectionChanged;
        public event UnityEngine.Events.UnityAction OnDisconnectClicked;
        public event UnityEngine.Events.UnityAction OnPopupDismissClicked;


        private void Awake()
        {

            if (SimulatorUtility.IsSimulator)
            {
                gameObject.SetActive(false);
            }
            DontDestroyOnLoad(gameObject);

        }
        private void OnEnable()
        {

            var eventSystems = FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (eventSystems.Length == 0)
            {
                var eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
                eventSystem.gameObject.AddComponent<StandaloneInputModule>();
                Debug.LogWarning("EventSystem not found on the scene. Adding one now.\nConsider creating an EventSystem yourself to forward UI input.", eventSystem);
            }

            lanOnlineToggle.onValueChanged.AddListener((_) => { OnModeChanged?.Invoke(_); });
            joinRoomButton.onClick.AddListener(() => { OnJoinRoomClicked?.Invoke(); });
            showCreateRoomPanelButton.onClick.AddListener(() => { OnShowCreateRoomPanelClicked?.Invoke(); });
            hideCreateRoomPanelButton.onClick.AddListener(() => { OnHideCreateRoomPanelClicked?.Invoke(); });
            createAndJoinRoomButton.onClick.AddListener(() => { OnCreateAndJoinRoomClicked?.Invoke(); });
            regionDropdown.onValueChanged.AddListener((_) => { OnRegionSelectionChanged?.Invoke(_); });
            refreshRegionsButton.onClick.AddListener(() => { OnRefreshRegionsClicked?.Invoke(); });
            refreshRoomsButton.onClick.AddListener(() => { OnRefreshRoomsClicked?.Invoke(); });
            disconnectButton.onClick.AddListener(() => { OnDisconnectClicked?.Invoke(); });
            popupDismissButton.onClick.AddListener(() => { OnPopupDismissClicked?.Invoke(); });

            popupDialog.SetActive(false);
            templateRoomView.gameObject.SetActive(false);

            roomsListView = new ListView
            {
                Template = templateRoomView,
                onSelectionChange = view => joinRoomButton.interactable = view != default && view.RoomData.UniqueId != default(RoomData).UniqueId
            };

            initialJoinRoomTitle = joinRoomTitleText.text;
        }

        private void OnDisable()
        {
            lanOnlineToggle.onValueChanged.RemoveListener(OnModeChanged);
            joinRoomButton.onClick.RemoveListener(OnJoinRoomClicked);
            showCreateRoomPanelButton.onClick.RemoveListener(OnShowCreateRoomPanelClicked);
            hideCreateRoomPanelButton.onClick.RemoveListener(OnHideCreateRoomPanelClicked);
            createAndJoinRoomButton.onClick.RemoveListener(OnCreateAndJoinRoomClicked);
            regionDropdown.onValueChanged.RemoveListener(OnRegionSelectionChanged);
            refreshRegionsButton.onClick.RemoveListener(OnRefreshRegionsClicked);
            refreshRoomsButton.onClick.RemoveListener(OnRefreshRoomsClicked);
            disconnectButton.onClick.RemoveListener(OnDisconnectClicked);
            popupDismissButton.onClick.RemoveListener(OnPopupDismissClicked);
        }
        public void SetActiveDashBoard(bool isActive) => dashBoard.SetActive(isActive);

        public void UpdateUIState(UIState state)
        {
            loadingSpinner.SetActive(state == UIState.LoadingSpinner);
            noRSPlaceholder.SetActive(state == UIState.NoReplicationServerFound);
            noRoomsAvailable.SetActive(state == UIState.NoRoomsExist);
            createRoomPanel.SetActive(state == UIState.CreatingRoom);
            noBridgeFound.SetActive(state == UIState.NoBridgeFound);
            noCloudPlaceholder.SetActive(state == UIState.CloudRoomsNotAvailable);
            noProjectSelected.SetActive(state == UIState.NoProjectSelected);
            noCloudLoginFound.SetActive(state == UIState.NoCloudLoginFound);
            noCloudRegionsAvailable.SetActive(state == UIState.NoCloudRegionsAvailable);
        }

        public void UpdateDialogsVisibility(bool isConnected)
        {
            connectDialog.SetActive(!isConnected);
            disconnectDialog.SetActive(isConnected);
        }

        public void UpdateRoomList(IReadOnlyList<RoomData> rooms, ulong lastCreatedRoomUid)
        {
            roomsListView.SetSource(rooms, lastCreatedRoomUid);
            joinRoomTitleText.text = $"{initialJoinRoomTitle} ({rooms.Count})";
            joinRoomButton.interactable = roomsListView.Selection != default;
        }

        public void ClearRoomList()
        {
            roomsListView.Clear();
            joinRoomTitleText.text = $"{initialJoinRoomTitle} (0)";
        }

        public void ShowError(string title, string message)
        {
            popupDialog.SetActive(true);
            popupTitleText.text = title;
            popupText.text = message;
        }

        public void HideError()
        {
            popupDialog.SetActive(false);
        }

        public void UpdateModeUI(bool isLocalMode)
        {
            regionDropdown.interactable = !isLocalMode;
            regionSection.SetActive(!isLocalMode);
            noRSPlaceholder.SetActive(isLocalMode);
            noRoomsAvailable.SetActive(false);
            noCloudPlaceholder.SetActive(!isLocalMode);
            cloudText.font = isLocalMode ? normalFont : boldFont;
            lanText.font = isLocalMode ? boldFont : normalFont;
        }

        public void UpdateRegionDropdown(IReadOnlyList<string> regions)
        {
            var dropdownOptions = new List<Dropdown.OptionData>();
            foreach (var region in regions)
            {
                dropdownOptions.Add(new Dropdown.OptionData(region));
            }
            regionDropdown.options = dropdownOptions;
            regionDropdown.captionText.text = regions.Count > 0 ? regions[0] : "";
        }

        public void SetRoomSectionButtonInteractability(bool interactable)
        {
            showCreateRoomPanelButton.interactable = interactable;
            refreshRegionsButton.interactable = interactable;
            refreshRoomsButton.interactable = interactable;
            joinRoomButton.interactable = interactable && roomsListView.Selection != default;
        }

        public RoomData GetSelectedRoom() => roomsListView.Selection.RoomData;
        public string GetRoomName() => roomNameInputField.text;
        public int GetRoomMaxPlayers() => int.TryParse(roomLimitInputField.text, out var limit) ? limit : 10;
        public bool HasPlayerSelected() => FindAnyObjectByType<SelectionCharactor>().character;
        public class ListView
        {
            public ConnectDialogRoomView Template;
            public Action<ConnectDialogRoomView> onSelectionChange;

            public ConnectDialogRoomView Selection
            {
                get => selection;
                set
                {
                    if (selection != value)
                    {
                        selection = value;
                        lastSelectedId = selection == default ? default : selection.RoomData.UniqueId;
                        onSelectionChange?.Invoke(Selection);
                        foreach (var viewRow in Views)
                        {
                            viewRow.IsSelected = selection == viewRow;
                        }
                    }
                }
            }

            public List<ConnectDialogRoomView> Views { get; }
            private ConnectDialogRoomView selection;
            private HashSet<ulong> displayedIds = new();
            private ulong lastSelectedId;

            public ListView(int capacity = 50)
            {
                Views = new List<ConnectDialogRoomView>(capacity);
            }

            public void SetSource(IReadOnlyList<RoomData> dataSource, ulong idToSelect = default)
            {
                if (dataSource.Count == Views.Count && dataSource.All(s => displayedIds.Contains(s.UniqueId)))
                {
                    return;
                }

                displayedIds = new HashSet<ulong>(dataSource.Select(d => d.UniqueId));

                Clear();

                if (dataSource.Count <= 0)
                {
                    return;
                }

                var sortedData = dataSource.ToList();
                sortedData.Sort((roomA, roomB) =>
                {
                    var strCompare = String.CompareOrdinal(roomA.RoomName, roomB.RoomName);
                    if (strCompare != 0)
                    {
                        return strCompare;
                    }

                    return (int)(roomA.UniqueId - roomB.UniqueId);
                });

                if (idToSelect == default && lastSelectedId != default)
                {
                    idToSelect = lastSelectedId;
                }

                foreach (var data in sortedData)
                {
                    var view = MakeViewItem(data);
                    Views.Add(view);
                    if (data.UniqueId == idToSelect)
                    {
                        Selection = view;
                    }
                }
            }

            private ConnectDialogRoomView MakeViewItem(RoomData data, bool isSelected = false)
            {
                var view = Instantiate(Template, Template.transform.parent);
                view.RoomData = data;
                view.IsSelected = isSelected;
                view.OnClick = () => Selection = view;
                view.gameObject.SetActive(true);
                return view;
            }

            public void Clear()
            {
                Selection = default;
                foreach (var view in Views)
                {
                    Destroy(view.gameObject);
                }

                Views.Clear();
            }
        }
    }
}