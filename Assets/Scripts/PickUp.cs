using System;
using Coherence.FirstSteps;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public float radius = 1.5f;   // Bán kính vùng kiểm tra
    public LayerMask pickupLayer; // Layer chứa vật thể nhặt được
    public Transform grabAction;  // Vị trí gắn vật khi nhặt
    public Transform grabPosition;
    private Transform items;      // Vật đang cầm
    private Grabbable pendingItem;
    private Charactor character;
    public Action<bool> animAction;
    private void Awake()
    {
        character = GetComponent<Charactor>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (items == null)
                TryPickup();
            else
                DropItem();
        }
    }

    private void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(grabAction.position, radius, pickupLayer);

        foreach (Collider hit in hits)
        {
            var item = hit.GetComponent<Grabbable>();
            if (item != null)
            {
                pendingItem = item;
                item.PickupValidated += OnPickupValidated;
                item.RequestPickup();
                break;
            }
        }
    }

    private void OnPickupValidated(bool authenticated)
    {
        if (pendingItem == null) return;

        pendingItem.PickupValidated -= OnPickupValidated;

        if (authenticated)
        {
            animAction?.Invoke(true);
            items = pendingItem.transform;
            items.SetParent(transform);
            items.position = grabPosition.position;
        }

        pendingItem = null;
    }

    private void DropItem()
    {
        if (items == null) return;

        var grab = items.GetComponent<Grabbable>();
        if (grab != null)
        {
            grab.Release();
        }

        items.SetParent(null);
        items = null;
        animAction?.Invoke(false);
    }

    private void OnDrawGizmos()
    {
        if (grabAction != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(grabAction.position, radius);
        }
    }
}
