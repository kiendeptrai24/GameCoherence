using Coherence.Connection;
using Coherence.Toolkit;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CoherenceBridge _coherenceBridge;
    private void OnEnable()
    {
        _coherenceBridge = FindAnyObjectByType<CoherenceBridge>();
        _coherenceBridge.onDisconnected.AddListener(OnDisconnected);
    }

    private void OnDisconnected(CoherenceBridge arg0, ConnectionCloseReason arg1)
    {
        Destroy(gameObject);
    }

}