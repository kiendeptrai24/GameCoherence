using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharactorUI : MonoBehaviour
{
    [SerializeField] private Button character1;
    [SerializeField] private Button character2;
    [SerializeField] private TextMeshProUGUI characterChose;
    public GameObject player1Reference;
    public GameObject player2Reference;
    private PlayerSpawner playerSpawner;
    private void Awake() {
        playerSpawner = FindAnyObjectByType<PlayerSpawner>();
        characterChose.text = "choose your character";
    }
    private void Start()
    {
        character1.onClick.AddListener(() =>
        {
            playerSpawner.SetPlayerPrefab(player1Reference);
            characterChose.text = "you chose Player 1";
        });
        character2.onClick.AddListener(() =>
        {
            playerSpawner.SetPlayerPrefab(player2Reference);
            characterChose.text = "you chose Player 2";
        });
    }
    
}
