using Coherence.Samples.RoomsDialog;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Coherence.Samples.Kien
{ 
    public class PlayerInteractable : MonoBehaviour , IInteractable
    {
        public enum CharactorType
        {
            None,
            Player,
            Robot
        }
        [SerializeField] private GameObject RecHighLight;
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private InputField playerNameInputField;
        private RoomView roomsDialogUI;
        private string nameChar;
        public CharactorType charactorType;
        private void Awake() {
            roomsDialogUI = FindAnyObjectByType<RoomView>();
            playerNameInputField = roomsDialogUI.playerNameInputField;
            nameChar = playerNameInputField.text;
            nameTxt.gameObject.SetActive(false);
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
        public string GetName() => nameChar;
    }
}
