public sealed class StageData
{
    public int StageID;
    public int WaveIndex;
    public int EnemyID;
    public int EnemyCount;

    public bool IsBossWave => WaveIndex == 4;
}
