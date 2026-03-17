using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<GameObject, IObjectPool<GameObject>> pools = new Dictionary<GameObject, IObjectPool<GameObject>>();
   
    public GameObject Spawn(GameObject prefab, Vector2 position, Quaternion rotation)
    {
        // 해당 프리팹의 풀이 없다면 새로 제작
        if (!pools.ContainsKey(prefab))
        {
            pools[prefab] = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Instantiate(prefab);

                    // 생성된 오브젝트에 Poolable 붙이기
                    Poolable poolable = obj.GetComponent<Poolable>();
                    if (poolable == null) poolable = obj.AddComponent<Poolable>();

                    poolable.Pool = pools[prefab]; // 자신이 돌아갈 집(Pool)을 기억하게 함
                    return obj;
                },
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: true,
                defaultCapacity: 10,
                maxSize: 100 // 최대 개수는 프로젝트에 맞게 조절 가능
            );
        }
        GameObject spawnedObj = pools[prefab].Get();
        spawnedObj.transform.SetPositionAndRotation(position, rotation);

        return spawnedObj;
    }
}