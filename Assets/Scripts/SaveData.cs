using System;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public double Gold = 0;

    // Key: ID, Value: Level
    public SerializationDictionary<string, int> UpgradeLevels = new SerializationDictionary<string, int>();

    public int GetUpgradeLevel(string id) => UpgradeLevels.ContainsKey(id) ? UpgradeLevels[id] : 0;
    public void SetUpgradeLevel(string id, int level) => UpgradeLevels[id] = level;
    public SaveData()
    {
        InitDefaultData();
    }
    public void InitDefaultData()
    {
        // 딕셔너리에 데이터가 없을 때만 기본값 세팅 (중복 방지)
        if (UpgradeLevels.ToDictionary().Count == 0)
        { 
            //csv 파일 id랑 같아야함
            UpgradeLevels["Atk"] = 1;      // 공격력
            UpgradeLevels["Hp"] = 1;          // 체력
            UpgradeLevels["AtkSpeed"] = 1;    // 공격속도
            UpgradeLevels["CriticalChance"] = 1; // 치명타확률
            UpgradeLevels["CriticalDamage"] = 1;   // 치명타데미지

            Gold = 100; // 시작 지원금 
        }
    }
}



[System.Serializable]
public class StageWaveSaveData
{
    public int StageID;
    public int WaveIndex;

    public StageWaveSaveData(int stageID, int waveIndex)
    {
        StageID = stageID;
        WaveIndex = waveIndex;
    }
}