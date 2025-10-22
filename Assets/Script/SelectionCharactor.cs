using UnityEngine;

public class SelectionCharactor : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsPlayer;
    private IInteractable objInteract;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity,whatIsPlayer))
            {
                objInteract?.EndInteract();
                objInteract = hit.collider.GetComponent<IInteractable>(); 
                objInteract?.Interact();
            }
        }
    }
    
}
