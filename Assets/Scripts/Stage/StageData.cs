using UnityEngine;

public class StageData
{
    public int StageID;
    public int WaveIndex;
    public int EnemyID;
    public int EnemyCount;

    // 웨이브 인덱스가 4면 자동으로 보스 웨이브
    public bool IsBossWave => WaveIndex == 4;
}
