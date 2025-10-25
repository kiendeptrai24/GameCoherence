using UnityEngine;
using Coherence;
using Coherence.Toolkit;
using UnityEngine.UI;
using TMPro;
using Coherence.Samples.Kien;

public class SendChat : MonoBehaviour
{
    public CoherenceSync _sync;
    [SerializeField] private Button sendButton;
    [SerializeField] private Button play;
    public TMP_InputField chatInputField;

    private void Awake()
    {
        _sync = GetComponent<CoherenceSync>();
        sendButton.onClick.AddListener(() =>
        {
            Debug.Log("onClick");
            if (_sync != null)
            {
                Debug.Log("Send message");
                if (string.IsNullOrWhiteSpace(chatInputField.text)) return;
                _sync.SendCommand<SendChat>(nameof(SendMessege), MessageTarget.Other, chatInputField.text);
                chatInputField.text = "";
            }
        });
        play.onClick.AddListener(() =>
        {
            SceneLoadManager.Instance.LoadRegularScene("Game");
        });
    }
    public void SendMessege(string message)
    {
        // if (client == _sync)
        // {
        Debug.Log("you: " + message);
        // }
        // else
        // {
        //     Debug.Log("other client: " + message);
        // }
    }
}