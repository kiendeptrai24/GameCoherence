using UnityEngine;
using Coherence;
using Coherence.Toolkit;
using UnityEngine.UI;
using TMPro;
using Coherence.Samples.Kien;

public class SendChat : MonoBehaviour
{
    private CoherenceSync _sync;
    [SerializeField] private Button sendButton;
    [SerializeField] private Button play;
    public TMP_InputField chatInputField;

    private void Awake()
    {
        _sync = GetComponent<CoherenceSync>();
        sendButton.onClick.AddListener(() =>
        {
            Debug.Log("onClick");
            ClientSendMessage(chatInputField.text);
        });
        play.onClick.AddListener(() =>
        {
            SceneLoadManager.Instance.LoadRegularScene("Game");
        });
    }
    private void ClientSendMessage(string message)
    {
        if (_sync == null) return;

        if (string.IsNullOrWhiteSpace(message)) return;
        Debug.Log("Send message");
        _sync.SendCommand<SendChat>(nameof(ReceiveMessege), MessageTarget.Other, message);
        chatInputField.text = "";
    }

    public void ReceiveMessege(string message)
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