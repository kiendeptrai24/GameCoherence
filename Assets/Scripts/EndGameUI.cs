
using Coherence.Samples.Kien;
using Coherence.Toolkit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private Button exit;
    private void Start()
    {
        var bridge = FindAnyObjectByType<CoherenceBridge>();
        var roomPresenter = FindAnyObjectByType<RoomPresenter>();
        exit.onClick.AddListener(() =>
        {
            if (bridge)
            {
                bridge.Disconnect();
                Destroy(roomPresenter.gameObject);
                SceneManager.LoadScene("StartGame");
            }
        });

    }
}