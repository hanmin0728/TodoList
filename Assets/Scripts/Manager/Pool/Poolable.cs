using UnityEngine;
using UnityEngine.Pool;
public class Poolable : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }

    /// 풀에서 꺼내질 때 초기화 로직 실행
    public virtual void OnSpawn() { }

    /// <summary>
    /// 풀로 돌아가기 직전에 실행될 정리 로직 
    /// </summary>
    public virtual void OnDespawn() { }

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
