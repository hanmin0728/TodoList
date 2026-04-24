using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public SaveData CurrentData;
    private string _saveFileName = "SaveData.json";

    private void Start()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(CurrentData);
        string encryptedData = CryptoUtility.Encrypt(json); // AES 암호화
        string filePath = Path.Combine(Application.persistentDataPath, _saveFileName);

        File.WriteAllText(filePath, encryptedData);
        Debug.Log("데이터 암호화 저장 완료!");
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
}
