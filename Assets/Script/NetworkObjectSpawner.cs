using UnityEngine;
using Coherence;
using Coherence.Toolkit;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using Coherence.Log;

public class NetworkObjectSpawner : Singleton<NetworkObjectSpawner>
{
    private CoherenceBridge _bridge;
    protected override void Awake()
    {
        base.Awake();
        _bridge = FindAnyObjectByType<CoherenceBridge>();
        DontDestroyOnLoad(gameObject);
    }
    private IEnumerator Start()
    {
        yield return null; 
        if (!CoherenceBridgeStore.TryGetBridge(SceneManager.GetActiveScene(), out _bridge))
        {
            Debug.LogError("Bridge not found yet.");
        }
    }


    public void SpawnNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation, System.Action<GameObject> onSpawned = null)
    {
        if (_bridge == null)
        {
            Debug.LogError("No CoherenceBridge found!");
            return;
        }
        Debug.Log("spawn");
        var spawned = Instantiate(prefab, position, rotation);
        var sync = spawned.GetComponent<CoherenceSync>();
        sync.TransferAuthority(_bridge.ClientID);
        onSpawned?.Invoke(spawned);
        
    }
}
