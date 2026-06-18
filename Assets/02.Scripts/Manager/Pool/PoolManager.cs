using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public sealed class PoolManager : Singleton<PoolManager>
{
    [Header("Pool Settings")]
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 200;

    private readonly Dictionary<GameObject, IObjectPool<GameObject>> poolsByPrefab = new Dictionary<GameObject, IObjectPool<GameObject>>();

    private readonly Dictionary<GameObject, Transform> parentsByPrefab = new Dictionary<GameObject, Transform>();


    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (prefab == null) return null;

        if (!poolsByPrefab.TryGetValue(prefab, out IObjectPool<GameObject> pool))
        {
            pool = CreatePool(prefab);
            poolsByPrefab.Add(prefab, pool);
        }

        GameObject obj = pool.Get();
        obj.transform.SetPositionAndRotation(position, rotation);

        if (parent != null)
        {
            obj.transform.SetParent(parent);
        }
        else
        {
            obj.transform.SetParent(parentsByPrefab[prefab]);
        }

        if (obj.TryGetComponent(out Poolable poolable))
        {
            poolable.OnSpawn();
        }

        return obj;
    }

    private IObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        GameObject root = new GameObject($"Pool_{prefab.name}");
        root.transform.SetParent(transform);
        parentsByPrefab.Add(prefab, root.transform);

        return new ObjectPool<GameObject>(
            createFunc: () => CreatePooledObject(prefab),
            actionOnGet: OnGetFromPool,
            actionOnRelease: OnReleaseToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize);
    }

    private GameObject CreatePooledObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, parentsByPrefab[prefab]);

        var poolable = obj.GetComponent<Poolable>() ?? obj.AddComponent<Poolable>();
        poolable.PrefabKey = prefab;
        poolable.Pool = poolsByPrefab[prefab];

        return obj;
    }

    private void OnGetFromPool(GameObject obj) => obj.SetActive(true);

    private void OnReleaseToPool(GameObject obj)
    {
        if (obj.TryGetComponent(out Poolable poolable))
        {
            poolable.OnDespawn();
        }

        if (poolable.PrefabKey != null && parentsByPrefab.TryGetValue(poolable.PrefabKey, out Transform parent))
        {
            obj.transform.SetParent(parent);
        }
        obj.SetActive(false);
    }

    private void OnDestroyPoolObject(GameObject obj) => Destroy(obj);


}
