using System.Collections;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    [Header("진행 상태")]
    public int _currentStageID = 101;
    public int _currentWaveIndex = 1;

    [Header("설정")]
    [SerializeField] private float _delayBetweenWaves = 300.0f; // 소환 완료 후 다음 웨이브까지 대기 시간
    private bool _isWaveActive = false;

    void Start()
    {
        // CSV 로딩 완료 후 첫 스테이지 시작
        if (CSVManager.Instance.IsInitialized)
        {
            StartWave(_currentStageID, _currentWaveIndex);
        }
        else
        {
            CSVManager.Instance.OnLoadingComplete += () => StartWave(_currentStageID, _currentWaveIndex);
        }
    }

    // 진입점 함수: 외부나 내부에서 스테이지 시작을 명령할 때 사용
    public void StartWave(int stageID, int waveIndex)
    {
        if (_isWaveActive) return;
        LoadAndStartWave(stageID, waveIndex);
    }

    private void LoadAndStartWave(int stageID, int waveIndex)
    {
        var table = CSVManager.Instance.GetTable("StageTable");
        var row = table.Find(r =>
            r["StageID"].ToString() == stageID.ToString() &&
            r["WaveIndex"].ToString() == waveIndex.ToString());

        if (row == null)
        {
            Debug.Log("<color=yellow>모든 스테이지 데이터가 끝났습니다.</color>");
            return;
        }

        // StageData 객체 생성 및 데이터 주입
        StageData currentWaveData = new StageData();
        currentWaveData.StageID = int.Parse(row["StageID"].ToString());
        currentWaveData.waveIndex = int.Parse(row["WaveIndex"].ToString());
        currentWaveData.enemyID = int.Parse(row["EnemyID"].ToString());
        currentWaveData.enemyCount = int.Parse(row["EnemyCount"].ToString());

        StartCoroutine(WaveRoutine(currentWaveData));
    }

    private IEnumerator WaveRoutine(StageData data)
    {
        _isWaveActive = true;
        //Debug.Log($"<color=cyan>웨이브 시작:</color> Stage {data.StageID} - Wave {data.waveIndex}");

        for (int i = 0; i < data.enemyCount; i++)
        {
            EnemySpawner.Instance.SpawnEnemy(data.enemyID);
            yield return new WaitForSeconds(0.8f);
        }

        //Debug.Log("<color=white>소환 완료! 잠시 후 다음 웨이브로 넘어갑니다.</color>");

        yield return new WaitForSeconds(_delayBetweenWaves);

        _isWaveActive = false;
        NextWave();
    }

    private void NextWave()
    {
        _currentWaveIndex++;

        if (_currentWaveIndex > 4) // 4웨이브 규칙
        {
            _currentWaveIndex = 1;
            _currentStageID++;
            Debug.Log($"<color=green>스테이지 클리어! 다음 스테이지: {_currentStageID}</color>");
        }

        StartWave(_currentStageID, _currentWaveIndex);
    }
}
