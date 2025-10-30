using System;
using Coherence.Samples.Kien;
using UnityEngine;

[Serializable]
public class CharactorInfo
{
    public string name;
    public PlayerInteractable.CharactorType type;
    public GameObject prefab;
    public CharactorInfo(string name, PlayerInteractable.CharactorType type, GameObject prefab)
    {
        this.name = name;
        this.type = type;
        this.prefab = prefab;
    }
    public CharactorInfo(string name, PlayerInteractable.CharactorType type)
    {
        this.name = name;
        this.type = type;
    }
    public CharactorInfo() { }
}