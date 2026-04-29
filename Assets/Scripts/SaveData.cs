using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    #region 재화
    [SerializeField] private double gold = 0;
    public double GetGold() => gold;
    public void SetGold(double cost)
    {
        gold = cost;
    }
    #endregion

    #region stage 관련
    [SerializeField] private int currentStageID = 101;
    [SerializeField] private int currentWaveIndex = 1;

    public int GetStageID() => currentStageID;
    public int GetWaveIndex() => currentWaveIndex;
    public void SetStageProgress(int stageID, int waveIndex)
    {
        if (stageID < 101 || waveIndex < 1) return;

        currentStageID = stageID;
        currentWaveIndex = waveIndex;
    }
    #endregion
    #region Upgrade Data
    // Key: ID, Value: Level
    [SerializeField] private SerializationDictionary<string, int> UpgradeLevels = new SerializationDictionary<string, int>();

    public int GetUpgradeLevel(string id) => UpgradeLevels.ContainsKey(id) ? UpgradeLevels[id] : 0;
    public void SetUpgradeLevel(string id, int level) => UpgradeLevels[id] = level;
    #endregion

  
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

            gold = 100;
            currentStageID = 101;
            currentWaveIndex = 1;
        }
    }

}


