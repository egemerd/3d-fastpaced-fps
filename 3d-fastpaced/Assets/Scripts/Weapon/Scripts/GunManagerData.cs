using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "GunManagerData", menuName = "ScriptableObjects/GunManagerData", order = 1)]
public class GunManagerData : ScriptableObject
{
    public AllGunData[] guns;
}

[System.Serializable]
public struct AllGunData
{
    public string gunName;
    public GunData gunDataSO;
}
