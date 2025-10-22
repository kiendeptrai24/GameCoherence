using UnityEngine;
using Coherence;
using Coherence.Toolkit;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class NetworkObjectSpawner : Singleton<NetworkObjectSpawner>
{
    private CoherenceBridge _bridge;
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("LoadingScene") || scene.name == "LoadingScene")
        {
            Debug.Log($"Skip CoherenceBridge check in {scene.name}");
            return;
        }

        // ✅ Dùng 'scene' thay vì 'gameObject.scene'
        if (!CoherenceBridgeStore.TryGetBridge(SceneManager.GetActiveScene(), out _bridge))
        {
            Debug.LogError($"{nameof(CoherenceBridge)} required on the scene.\n" +
                            "Add one via 'GameObject > coherence > Bridge'. " + SceneManager.GetActiveScene().name, this);
            return;
        }

        Debug.Log($"Bridge found in {scene.name}");
    }


    public void SpawnNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation, System.Action<GameObject> onSpawned = null)
    {
        if (_bridge == null)
        {
            Debug.LogError("No CoherenceBridge found!");
            return;
        }

        var spawned = Instantiate(prefab, position, rotation);
        onSpawned?.Invoke(spawned);
    }
}
