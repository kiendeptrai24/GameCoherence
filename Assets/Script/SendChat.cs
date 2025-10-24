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
                _sync.SendCommand<SendChat>(nameof(OnChatMessage),
                    MessageTarget.Other, chatInputField.text);
                _sync.SendCommand<SendChat>(nameof(SendCommand),
                    MessageTarget.All);
                chatInputField.text = "";
            }
        });
        play.onClick.AddListener(() =>
        {
            SceneLoadManager.Instance.LoadNetworkScene("Game");
        });
    }
    public void OnChatMessage(string message)
    {
        Debug.Log($"Received chat message: {message}");
    }
    public void SendCommand()
    {
        Debug.Log("SendCommand");
    }
}