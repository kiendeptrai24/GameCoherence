
using System;
using UnityEngine;

namespace Coherence.Samples.Kien
{
    public class ServicePlayerInfo : Singleton<ServicePlayerInfo>
    {
        [Serializable]
        public class CharacterInfo
        {
            public string name;
            public PlayerInteractable.CharactorType type;
            public GameObject prefab;
            public CharacterInfo(string name, PlayerInteractable.CharactorType type, GameObject prefab)
            {
                this.type = type;
                this.prefab = prefab;
            }
            public CharacterInfo(string name, PlayerInteractable.CharactorType type)
            {
                this.name = name;
                this.type = type;
            }
            public CharacterInfo() {}
        }
        public CharacterInfo charactor;
        private void OnEnable()
        {
            
        }
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        public void SetCharactor(CharacterInfo info)
        {
            charactor = info;
        }
    }
}