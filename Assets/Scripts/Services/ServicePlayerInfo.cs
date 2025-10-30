
using System;
using Coherence.Cloud;
using UnityEngine;

namespace Coherence.Samples.Kien
{
    public class ServicePlayerInfo : Singleton<ServicePlayerInfo>
    {
        public CharactorInfo charactor;
        public GameData gameData;
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        public void SetCharactor(CharactorInfo info)
        {
            charactor = info;
        }
        public void SetGameData(RoomData data)
        {
            gameData = new GameData(data);
        }
    }
}