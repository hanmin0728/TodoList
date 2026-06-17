using System;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks; // UniTask 사용
using Firebase.Database;
using Firebase.Auth;

#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class SaveManager : Singleton<SaveManager>
{
    private const string SaveFileName = "SaveData.json";
    private static readonly WaitForSeconds AutoSaveDelay = new WaitForSeconds(60f);

#if UNITY_EDITOR
    [SerializeField] private bool enableEditorHotkeys;
#endif

    // 로드 완료 알림용 액션
    public event Action OnDataLoaded;

    public SaveData CurrentData;

    private bool isSaving = false; // 중복 저장 방지 플래그

    private CancellationTokenSource cts; // 동기 작업을 안전하게 종료하기 위한 토큰 

    private DatabaseReference DBRef => FirebaseDatabase.DefaultInstance.RootReference;
    
    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
        {
            return;
        }
        cts = new CancellationTokenSource();
        LoadGame();
    }

    private void Start()
    {
        StartCoroutine(AutoSaveCoroutine());
    }
    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (enableEditorHotkeys && Input.GetKeyDown(KeyCode.O))
        {
            EditorUtility.RevealInFinder(GetSaveFilePath());
        }

    }
#endif

    #region 로컬 저장 및 로드
    // 동기 저장
    public void SaveGameSync()
    {
        if (CurrentData == null || isSaving) return;

        try
        {
            string json = JsonUtility.ToJson(CurrentData);
            string encryptedData = CryptoUtility.Encrypt(json);
            File.WriteAllText(GetSaveFilePath(), encryptedData);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Sync Save Failed: {e.Message}");
        }
    }

    // 비동기 저장
    public async void SaveGameAsync()
    {
        if (CurrentData == null || isSaving) return;

        isSaving = true;
        try
        {
            string json = JsonUtility.ToJson(CurrentData);
            string filePath = GetSaveFilePath();

            await UniTask.RunOnThreadPool(() =>
            {
                string encryptedData = CryptoUtility.Encrypt(json);
                File.WriteAllText(filePath, encryptedData);
            }, cancellationToken: cts.Token);
        }
        catch (OperationCanceledException)
        {
            // 작업 취소 시 발생하는 정상적인 예외이므로 무시
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Async Save Failed: {e.Message}");
        }
        finally
        {
            isSaving = false;
        }
    }

    public void LoadGame()
    {
        string filePath = GetSaveFilePath();

        if (!File.Exists(filePath))
        {
            CreateNewSave();
            return;
        }

        try
        {
            string encryptedData = File.ReadAllText(filePath);
            string decryptedJson = CryptoUtility.Decrypt(encryptedData);
            CurrentData = JsonUtility.FromJson<SaveData>(decryptedJson) ?? new SaveData();
            OnDataLoaded?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Save load failed. Creating new. Error: {e.Message}");
            CreateNewSave();
        }
    }

    private void CreateNewSave()
    {
        CurrentData = new SaveData();
        OnDataLoaded?.Invoke();
    }
    #endregion

    #region 클라우드 서버 동기화 (새로 추가된 기능)

    /// <summary>
    /// 로컬의 SaveData를 Firebase 서버에 백업
    /// </summary>
    public async UniTask SaveToServerAsync()
    {
        var currentUser = FirebaseAuth.DefaultInstance?.CurrentUser;
        if (currentUser == null || CurrentData == null) return;

        try
        {
            string uid = currentUser.UserId;
            string json = JsonUtility.ToJson(CurrentData);

            //토큰을 넘겨주어 앱 종료 시 통신 안전 중단
            await DBRef.Child("users").Child(uid).Child("saveData")
                       .SetRawJsonValueAsync(json)
                       .AsUniTask()
                       .AttachExternalCancellation(cts.Token);

            Debug.Log("[SaveManager] 클라우드 서버 데이터 백업 완료");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("[SaveManager] 클라우드 저장 중 앱이 종료/전환되어 저장 취소");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 클라우드 저장 실패: {e.Message}");
        }

    }

    /// <summary>
    /// 서버에서 SaveData를 불러와 로컬에 덮어쓰기 (자동 로그인 성공 시 호출)
    /// </summary>
    public async UniTask<bool> LoadFromServerAsync()
    {
        var currentUser = FirebaseAuth.DefaultInstance?.CurrentUser;
        if (currentUser == null) return false;

        try
        {
            string uid = currentUser.UserId;

            //토큰 적용
            var snapshot = await DBRef.Child("users").Child(uid).Child("saveData")
                                      .GetValueAsync()
                                      .AsUniTask()
                                      .AttachExternalCancellation(cts.Token);

            if (snapshot.Exists)
            {
                string serverJson = snapshot.GetRawJsonValue();
                CurrentData = JsonUtility.FromJson<SaveData>(serverJson);

                SaveGameSync(); // 불러온 데이터를 즉시 로컬 기기에도 저장
                OnDataLoaded?.Invoke(); // UI 갱신 이벤트

                Debug.Log("[SaveManager] 서버 데이터 로드 및 적용 성공!");
                return true;
            }
            return false;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("[SaveManager] 서버 로드 중 작업이 취소되었습니다.");
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 서버 로드 실패: {e.Message}");
            return false;
        }
    }
    #endregion

    #region 유니티 생명주기 저장 대응

    private IEnumerator AutoSaveCoroutine()
    {
        while (true)
        {
            yield return AutoSaveDelay;
            SaveGameAsync();
        }
    }

    private void OnApplicationPause(bool isPaused)
    {
        // 스마트폰에서 홈버튼을 눌러 앱이 백그라운드로 내려갈 때
        if (isPaused) SaveGameSync();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // 다른 팝업이 떠서 포커스를 잃을 때
        if (!hasFocus) SaveGameSync();
    }

    protected override void OnApplicationQuit() 
    {
        // 앱이 완전히 종료될 때 로컬에 동기식으로 무조건 박아넣음
        SaveGameSync();


        base.OnApplicationQuit();
    }

    private static string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }
    #endregion
}
