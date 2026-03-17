using UnityEngine;
using UnityEngine.Pool;
public class Poolable : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }

    /// <summary>
    /// 오브젝트의 사용이 끝났을 때 호출하여 풀로 돌려보냄 
    /// </summary>
    public void Release()
    {
        if (gameObject.activeSelf)
        {
            Pool.Release(gameObject);
        }
    }
}
