using UnityEngine;

public class 
    Singleton<T> : MonoBehaviour where T : MonoBehaviour    
{
    private static T _instance;
    private static bool _applicationIsQuitting = false;

    public static bool HasInstance => _instance != null && !_applicationIsQuitting;
    
    public static T Instance
    {
        get
        {
            // 애플리케이션이 종료 중일 때 인스턴스에 접근하면 오류가 날 수 있으므로 방지
            if (_applicationIsQuitting)
            {
                return null;
            }

            if (_instance == null)
            {
                // 현재 씬에 이미 배치되어 있는지 확인
                _instance = FindFirstObjectByType<T>();

                // 씬에 없다면 새로 생성
                if (_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = typeof(T).ToString() + " (Singleton)";

                    // 씬이 변경되어도 파괴되지 않게 설정
                    DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;

            // 만약 씬에 배치된 상태로 시작했다면 루트 오브젝트로 빼서 DontDestroyOnLoad 적용
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            // 이미 인스턴스가 존재하는데 다른 오브젝트가 있다면 파괴
            Destroy(gameObject);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }
}


