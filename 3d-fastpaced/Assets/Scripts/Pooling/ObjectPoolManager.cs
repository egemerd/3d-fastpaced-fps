using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    // Singleton instance
    public static ObjectPoolManager Instance { get; private set; }

    [Header("Pool Configuration")]
    [SerializeField] private List<PoolList> pools = new List<PoolList>();

    // Hýzlý eriþim için dictionary
    private Dictionary<string, PoolList> poolDictionary = new Dictionary<string, PoolList>();

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePools();
    }

    /// <summary>
    /// Tüm havuzlarý baþlangýçta doldur
    /// </summary>
    private void InitializePools()
    {
        foreach (PoolList pool in pools)
        {
            if (pool.prefab == null)
            {
                Debug.LogWarning($"[ObjectPool] Pool '{pool.poolName}' has no prefab!");
                continue;
            }

            // Parent transform oluþtur (Hierarchy düzeni için)
            if (pool.parentTransform == null)
            {
                GameObject parent = new GameObject($"Pool_{pool.poolName}");
                parent.transform.SetParent(transform);
                pool.parentTransform = parent.transform;
            }

            // Baþlangýç objelerini oluþtur
            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = CreateNewObject(pool);
                pool.pooledObjects.Add(obj);
            }

            // Dictionary'e ekle
            poolDictionary[pool.poolName] = pool;

            Debug.Log($"[ObjectPool] '{pool.poolName}' initialized with {pool.initialSize} objects.");
        }
    }

    /// <summary>
    /// Yeni bir obje oluþtur
    /// </summary>
    private GameObject CreateNewObject(PoolList pool)
    {
        GameObject obj = Instantiate(pool.prefab, pool.parentTransform);
        obj.SetActive(false);
        obj.name = $"{pool.prefab.name}_{pool.pooledObjects.Count}";
        return obj;
    }

    /// <summary>
    /// Pool'dan obje al - EN ÖNEMLÝ METHOD
    /// </summary>
    public GameObject GetPooledObject(string poolName)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogError($"[ObjectPool] Pool '{poolName}' not found!");
            return null;
        }

        PoolList pool = poolDictionary[poolName];

        // Pasif obje ara
        foreach (GameObject obj in pool.pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // Havuzda yer yoksa ve büyüyebiliyorsa
        if (pool.canGrow)
        {
            GameObject newObj = CreateNewObject(pool);
            pool.pooledObjects.Add(newObj);
            newObj.SetActive(true);
            Debug.Log($"[ObjectPool] '{poolName}' expanded. New size: {pool.pooledObjects.Count}");
            return newObj;
        }

        Debug.LogWarning($"[ObjectPool] '{poolName}' is full!");
        return null;
    }

    /// <summary>
    /// Objeyi pool'a geri koy
    /// </summary>
    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);
        obj.transform.SetParent(GetParentForObject(obj));
    }

    /// <summary>
    /// Obje hangi pool'a ait?
    /// </summary>
    private Transform GetParentForObject(GameObject obj)
    {
        foreach (PoolList pool in pools)
        {
            if (pool.pooledObjects.Contains(obj))
            {
                return pool.parentTransform;
            }
        }
        return transform;
    }

    /// <summary>
    /// Gecikme ile pool'a dön
    /// </summary>
    public void ReturnToPoolAfterDelay(GameObject obj, float delay)
    {
        if (obj == null) return;
        StartCoroutine(ReturnAfterDelayCoroutine(obj, delay));
    }

    private System.Collections.IEnumerator ReturnAfterDelayCoroutine(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null) // Obje destroy edilmemiþ mi kontrol et
        {
            ReturnToPool(obj);
        }
    }
}
