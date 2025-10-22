using System;
using System.Collections.Generic;
using Coherence.Connection;
using Coherence.Toolkit;
using UnityEngine;


public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _characterPrefab;
    GameObject playerReference;
    private CoherenceBridge _coherenceBridge;
    public bool chosePlayer = false;

    public void SetPlayerPrefab(GameObject prefab)
    {
        chosePlayer = true;
        _characterPrefab = prefab;
    }

    private void OnEnable()
    {
        _coherenceBridge = FindAnyObjectByType<CoherenceBridge>();
        _coherenceBridge.onConnected.AddListener(OnConnected);
        _coherenceBridge.onDisconnected.AddListener(OnDisconnected);
    }

    private void OnDisconnected(CoherenceBridge arg0, ConnectionCloseReason arg1)
    {
        Destroy(playerReference);
    }

    private void OnConnected(CoherenceBridge arg0)
    {
        playerReference = Instantiate(_characterPrefab, Vector3.zero, Quaternion.identity);
    }
}
