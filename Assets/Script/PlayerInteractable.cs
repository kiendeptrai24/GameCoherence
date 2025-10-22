using Coherence.Samples.RoomsDialog;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractable : MonoBehaviour , IInteractable
{
    [SerializeField] private GameObject RecHighLight;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private InputField playerNameInputField;
    private RoomsDialogUI roomsDialogUI;
    private string nameChar;
    private void Awake() {
        roomsDialogUI = FindAnyObjectByType<RoomsDialogUI>();
        playerNameInputField = roomsDialogUI.playerNameInputField;
        nameChar = playerNameInputField.text;
        playerNameInputField.onValueChanged.AddListener((value) =>
        {
            ChangeName(value);
        });
        playerNameInputField.onEndEdit.AddListener((value) =>
        {
            ChangeName(value);
        });
    }
    private void Start() {
        RecHighLight.SetActive(false);
    }
    public void Interact()
    {
        RecHighLight.SetActive(true);
        nameTxt.gameObject.SetActive(true);
        ChangeName(nameChar);
    }
    public void EndInteract()
    {
        RecHighLight.SetActive(false);
        nameTxt.gameObject.SetActive(false);
    }
    public void ChangeName(string name)
    {
        nameChar = name;
        nameTxt.text = nameChar;
    }
}
