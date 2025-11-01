using System;
using System.Collections;
using Coherence;
using Coherence.Samples.Kien;
using Coherence.Toolkit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [Header("UI References")]
    private CoherenceSync _sync;
    public ScrollRect scrollRect;
    public Transform content;
    public GameObject leftMessagePrefab;
    public GameObject rightMessagePrefab;
    [SerializeField] private TMP_InputField messegeField;
    [SerializeField] private Button sendMessegeBtn;
    [SerializeField] private Button play;
    private void Awake()
    {
        sendMessegeBtn.onClick.AddListener(() =>
        {
            OnSendButtonClicked();

        });
        play.onClick.AddListener(() =>
        {
            string mode = ServicePlayerInfo.Instance.gameData.roomData.KV["GameMode"];
            if (Enum.TryParse<GameMode>(mode, out var gameMode))
            {
                Debug.Log(gameMode);

                switch (gameMode)
                {
                    case GameMode.Timer:
                        SceneLoadManager.Instance.LoadRegularScene("Game 2");
                        break;
                    case GameMode.Delivery:
                        SceneLoadManager.Instance.LoadRegularScene("Game");
                        break;
                }
            }
            Destroy(gameObject);
        });
    }
    void Start()
    {
        _sync = GetComponent<CoherenceSync>();
        foreach (Transform child in content)
            Destroy(child.gameObject);
        messegeField.ActivateInputField();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            OnSendButtonClicked();
    }
    public void OnSendButtonClicked()
    {

        if (string.IsNullOrWhiteSpace(messegeField.text))
            return;
        ClientSendMessage(messegeField.text);
        AddMessage(messegeField.text, true);
        messegeField.text = "";
        messegeField.ActivateInputField();
    }
    private void ClientSendMessage(string message)
    {
        if (_sync == null) return;

        if (string.IsNullOrWhiteSpace(message)) return;
        _sync.SendCommand<ChatUI>(nameof(ReceiveMessege), MessageTarget.Other, message);
    }
    public void AddMessage(string text, bool isMine)
    {
        if (isMine)
            text = text + " :You";
        else
            text = "Other client: " + text;

        var prefab = isMine ? rightMessagePrefab : leftMessagePrefab;
        GameObject msg = Instantiate(prefab, content);

        TMP_Text msgText = msg.GetComponent<TMP_Text>();
        msgText.text = text;

        // Cập nhật layout
        Canvas.ForceUpdateCanvases();

        // Gọi coroutine cuộn mượt (sẽ đợi 1 frame để layout hoàn tất)
        StartCoroutine(SmoothScrollToBottom());
    }
    public void ReceiveMessege(string message)
    {
        AddMessage(message, false);
    }
    private IEnumerator SmoothScrollToBottom()
    {
        // Đợi 1 frame để Unity cập nhật lại layout sau khi thêm phần tử
        yield return null;

        Canvas.ForceUpdateCanvases();

        float duration = 0.15f;
        float time = 0f;
        float startPos = scrollRect.verticalNormalizedPosition;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPos, 0f, t);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f;
    }

}
