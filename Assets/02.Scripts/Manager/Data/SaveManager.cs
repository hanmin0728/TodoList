using System;
using System.Collections;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;


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

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
        {
            return;
        }

        LoadGame();
    }

    private void Start()
    {
        StartCoroutine(AutoSaveCoroutine());
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

            await Task.Run(() =>
            {
                string encryptedData = CryptoUtility.Encrypt(json);
                File.WriteAllText(filePath, encryptedData);
            });
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
        if (isPaused) SaveGameSync(); 
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) SaveGameSync();
    }

    protected override void OnApplicationQuit()
    {
        SaveGameSync(); 
        base.OnApplicationQuit();
    }

    private static string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }
}
