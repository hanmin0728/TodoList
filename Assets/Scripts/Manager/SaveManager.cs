using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public SaveData CurrentData;
    private string _saveFileName = "SaveData.json";

    public Action OnDataLoaded; // 로드 완료 알림용 액션

    private void Start()
    {
        LoadGame();
        StartCoroutine(AutoSaveCoroutine());
    }
    
    
     
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(CurrentData);
        string encryptedData = CryptoUtility.Encrypt(json); // AES 암호화
        string filePath = Path.Combine(Application.persistentDataPath, _saveFileName);

        File.WriteAllText(filePath, encryptedData);
        Debug.Log("데이터 암호화 저장 완료!");
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.O)) // O키를 누르면 폴더가 열림
        {
            EditorUtility.RevealInFinder(Path.Combine(Application.persistentDataPath, _saveFileName));
        }
    }
    public void LoadGame()
    {
        string filePath = Path.Combine(Application.persistentDataPath, _saveFileName);

        if (File.Exists(filePath))
        {
            try
            {
                string encryptedData = File.ReadAllText(filePath);
                string decryptedJson = CryptoUtility.Decrypt(encryptedData); // AES 복호화
                CurrentData = JsonUtility.FromJson<SaveData>(decryptedJson);
                OnDataLoaded?.Invoke();
                Debug.Log("세이브 파일 불러오기 성공!");
            }
            catch (Exception)
            {
                Debug.LogError("세이브 파일 변조 감지! 초기화합니다.");
                CurrentData = new SaveData();
            }
        }
        else
        {
            CurrentData = new SaveData();
        }
    }

    private IEnumerator AutoSaveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f); // 60초마다 저장
            SaveGame();
            Debug.Log("자동 저장 완료!");
        }
    }

    private void OnApplicationPause(bool pause)
    {
        // 앱이 백그라운드로 넘어가거나 다시 돌아올 때 호출
        if (pause) SaveGame();
    }

    private void OnApplicationFocus(bool focus)
    {
        // 유니티 에디터나 모바일에서 포커스를 잃을 때 호출
        if (!focus) SaveGame();
    }
    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        SaveGame();
        Debug.Log("앱 종료 시 데이터 자동 저장 완료!");
    }
}
