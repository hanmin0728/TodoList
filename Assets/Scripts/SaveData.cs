using System;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public long Gold = 0;

    // 능력치 레벨들을 관리할 리스트 
    public List<StatSaveData> StatLevels = new List<StatSaveData>();

}

[System.Serializable]
public class StatSaveData
{
    public string StatID; // CSV의 StatID 
    public int Level;     // 현재 레벨

    public StatSaveData(string id, int level)
    {
        StatID = id;
        Level = level;
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