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
        _sync = GetComponent<CoherenceSync>();
        sendMessegeBtn.onClick.AddListener(() =>
        {
            OnSendButtonClicked();

        });
        play.onClick.AddListener(() =>
        {
            SceneLoadManager.Instance.LoadRegularScene("Game");
            Destroy(gameObject);
        });
    }
    void Start()
    {
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
            Debug.Log("you: " + text);
        else
            Debug.Log("other client: " + text);
        var prefab = isMine ? rightMessagePrefab : leftMessagePrefab;
        GameObject msg = Instantiate(prefab, content);

        // Gán nội dung
        TMP_Text msgText = msg.GetComponent<TMP_Text>();
        msgText.text = text;

        // Cập nhật layout trước khi cuộn
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();

        // Cuộn mượt xuống cuối
        StartCoroutine(SmoothScrollToBottom());
    }
    public void ReceiveMessege(string message)
    {
        AddMessage(message, false);
    }
    private IEnumerator SmoothScrollToBottom()
    {
        yield return null; // Đợi 1 frame
        float duration = 0.2f;
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
