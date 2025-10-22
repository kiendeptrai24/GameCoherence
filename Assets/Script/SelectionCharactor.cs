using System;
using UnityEngine;
namespace Coherence.Samples.Kien
{

    public class SelectionCharactor : MonoBehaviour
    {
        public string namechar;
        public PlayerInteractable.CharactorType charactorType;
        [SerializeField] private LayerMask whatIsPlayer;
        private IInteractable objInteract;
        private PlayerInteractable player;
        public GameObject character;
        public ServicePlayerInfo.CharacterInfo GetCharactor()
        {
            player = character.GetComponent<PlayerInteractable>();
            charactorType = player.charactorType;
            namechar = player.GetName();
            ServicePlayerInfo.CharacterInfo newchar = new ServicePlayerInfo.CharacterInfo();
            newchar.type = charactorType;
            newchar.name = namechar;
            return newchar;
        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, whatIsPlayer))
                {
                    objInteract?.EndInteract();
                    objInteract = hit.collider.GetComponent<IInteractable>();
                    objInteract?.Interact();
                    character = hit.collider.gameObject;
                    ServicePlayerInfo.Instance.SetCharactor(GetCharactor());
                }
            }
        }

    }
}