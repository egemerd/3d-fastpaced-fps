using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolList
{
    [Header("Pool List Settings")]
    public string poolName;
    public GameObject prefab;
    public int initialSize = 10;
    public bool canGrow = true;
    public Transform parentTransform;

    [HideInInspector]
    public List<GameObject> pooledObjects = new List<GameObject>();

}
    

