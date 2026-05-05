using System;
using System.Collections;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveManager : Singleton<SaveManager>
{
    private const string SaveFileName = "SaveData.json";
    private static readonly WaitForSeconds AutoSaveDelay = new WaitForSeconds(60f);

#if UNITY_EDITOR
    [SerializeField] private bool enableEditorHotkeys;
#endif

    // 로드 완료 알림용 액션
    public event Action OnDataLoaded;

    public SaveData CurrentData { get; private set; }

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
        if (!enableEditorHotkeys)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            EditorUtility.RevealInFinder(GetSaveFilePath());
        }

    }
#endif

    public void SaveGame()
    {
        if (CurrentData == null)
        {
            return;
        }

        string json = JsonUtility.ToJson(CurrentData);
        string encryptedData = CryptoUtility.Encrypt(json);
        File.WriteAllText(GetSaveFilePath(), encryptedData);
    }

    public void LoadGame()
    {
        string filePath = GetSaveFilePath();

        if (!File.Exists(filePath))
        {
            CurrentData = new SaveData();
            OnDataLoaded?.Invoke();
            return;
        }

        try
        {
            string encryptedData = File.ReadAllText(filePath);
            string decryptedJson = CryptoUtility.Decrypt(encryptedData);
            CurrentData = JsonUtility.FromJson<SaveData>(decryptedJson);

            if (CurrentData == null)
            {
                CurrentData = new SaveData();
            }

            OnDataLoaded?.Invoke();
        }
        catch (Exception e)
        {
            //세이브 파일이 없는 신규 유저
            Debug.LogError($"[SaveManager] Failed to load save file. A new save will be created. Error: {e.Message}");
            CurrentData = new SaveData();
            OnDataLoaded?.Invoke();
        }
    }

    private IEnumerator AutoSaveCoroutine()
    {
        while (true)
        {
            yield return AutoSaveDelay;
            SaveGame();
        }
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            SaveGame();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveGame();
        }
    }

    protected override void OnApplicationQuit()
    {
        SaveGame();
        base.OnApplicationQuit();
    }

    private static string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }
}
