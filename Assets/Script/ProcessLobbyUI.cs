using UnityEngine;
using UnityEngine.UI;

public class ProcessLobbyUI : MonoBehaviour
{
    [SerializeField] private Button OnCreateBtn;
    [SerializeField] private Button OnJoinBtn;
    [SerializeField] private Button OnReloadBtn;
    private LobbyManager lobbyManager;
    private void Awake() {
        lobbyManager = GetComponent<LobbyManager>();
    }

    private void Start() {
        
    }
}
