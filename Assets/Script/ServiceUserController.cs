using UnityEngine;
using System.Collections.Generic;
using System;
using Coherence.Toolkit;
namespace Coherence.Samples.Kien
{
    public class ServiceUserController : MonoBehaviour
    {
        [Serializable]
        public class CharactorSelect
        {
            public GameObject prefab;
            public PlayerInteractable.CharactorType type;
        }
        private CoherenceBridge _bridge;

        [SerializeField] private NetworkObjectSpawner _spawner;
        [SerializeField] private ServicePlayerInfo _charInfo;


        [SerializeField] private List<CharactorSelect> userPrefabs;
        [SerializeField] private List<Transform> Points;
        [SerializeField] private PlayerInteractable.CharactorType charactorType;
        private readonly Dictionary<PlayerInteractable.CharactorType, GameObject> _activeUsers = new();
        private void Awake()
        {
            _spawner = NetworkObjectSpawner.Instance;

            foreach (CharactorSelect user in userPrefabs)
            {
                _activeUsers.Add(user.type, user.prefab);
            }
            _charInfo = ServicePlayerInfo.Instance;
            if (_charInfo.charactor.type == PlayerInteractable.CharactorType.None)
                return;
            charactorType = _charInfo.charactor.type;
        }
        private void Start()
        {
            SpawnUser(charactorType, GetPointPosition(), Quaternion.identity);
        }

        public void SpawnUser(PlayerInteractable.CharactorType type, Vector3 position, Quaternion rotation)
        {
            charactorType = type;
            GameObject prefab = GetPrefabByType(charactorType);
            if (prefab == null)
            {
                Debug.LogWarning($"No prefab found for type {type}");
                return;
            }

            _spawner.SpawnNetworkObject(prefab, position, rotation, obj =>
            {
                _charInfo.charactor.prefab = GetPrefabByType(_charInfo.charactor.type);
            });
        }

        private GameObject GetPrefabByType(PlayerInteractable.CharactorType type)
        {
            return _activeUsers[type];
        }
        private Vector3 GetPointPosition()
        {
            return Vector3.zero;
        }
    }
}
