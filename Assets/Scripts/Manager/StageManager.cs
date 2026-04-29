using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageManager : Singleton<StageManager>
{
    [Header("진행 상태")]
    private int currentStageID = 101;
    private int currentWaveIndex = 1;
    public int GetCurrentStageID() => currentStageID;
    public int GetCurrentWaveIndex() => currentWaveIndex;

    private bool isWaveActive = false;

    private Dictionary<string, StageData> stageDataDic = new Dictionary<string, StageData>();

    [Header("시간 설정")]
    [SerializeField] private float normalWaveDelay = 1.5f; // 일반 웨이브 간 대기 시간
    [SerializeField] private float bossClearDelay = 3.0f;  // 보스 클리어 후 대기 시간

    public event Action<int, int> OnWaveChanged;
    void Start()
    {
        if (CSVManager.Instance.IsInitialized)
        {
            InitStageData();
        }
        else
        {
            CSVManager.Instance.OnLoadingComplete += InitStageData;
        }
    }

    private void InitStageData()
    {
        CSVManager.Instance.OnLoadingComplete -= InitStageData;
        stageDataDic.Clear();

        var table = CSVManager.Instance.GetTable("StageTable");
        foreach (var row in table)
        {
            StageData data = new StageData
            {
                StageID = int.Parse(row["StageID"].ToString()),
                WaveIndex = int.Parse(row["WaveIndex"].ToString()),
                EnemyID = int.Parse(row["EnemyID"].ToString()),
                EnemyCount = int.Parse(row["EnemyCount"].ToString())
            };

            string key = $"{data.StageID}_{data.WaveIndex}";
            if (!stageDataDic.ContainsKey(key))
            {
                stageDataDic.Add(key, data);
            }
        }
        
        Debug.Log($"<color=green>스테이지 데이터 {stageDataDic.Count}개 로드 완료!</color>");

        if (SaveManager.Instance != null && SaveManager.Instance.CurrentData != null)
        {
            currentStageID = SaveManager.Instance.CurrentData.GetStageID();
            currentWaveIndex = SaveManager.Instance.CurrentData.GetWaveIndex();
        }

        StartWave(currentStageID, currentWaveIndex);
    }
    public void StartWave(int stageID, int waveIndex)
    {
        if (isWaveActive) return;

        string key = $"{stageID}_{waveIndex}";

        if (stageDataDic.TryGetValue(key, out StageData currentWaveData))
        {
            StartCoroutine(WaveRoutine(currentWaveData));
            OnWaveChanged?.Invoke(stageID, waveIndex);
        }
        else
        {
            Debug.Log("<color=yellow>모든 스테이지를 클리어했습니다!</color>");
        }
    }
    private IEnumerator WaveRoutine(StageData data)
    {
        isWaveActive = true;

        if (data.IsBossWave)
        {
            Debug.Log($"<color=red>⚠️ 보스 등장! Stage {data.StageID} - Boss Wave ⚠️</color>");
        }
        else
        {
        }

        // 몬스터 소환
        for (int i = 0; i < data.EnemyCount; i++)
        {
            EnemySpawner.Instance.SpawnEnemy(data.EnemyID);
            yield return new WaitForSeconds(0.2f); // 소환 간격
        }

        yield return new WaitUntil(() => EnemySpawner.Instance.GetActiveEnemyCount() == 0);

        if (data.IsBossWave)
        {
            yield return new WaitForSeconds(bossClearDelay);
        }
        else
        {
            yield return new WaitForSeconds(normalWaveDelay);
        }

        isWaveActive = false;
        NextWave();
    }

    private void NextWave()
    {
        currentWaveIndex++;

        // 4웨이브(보스)까지 깼다면 다음 스테이지로!
        if (currentWaveIndex > 4)
        {
            currentWaveIndex = 1;
            currentStageID++;
            Debug.Log($"<color=green>스테이지 클리어! 다음 스테이지: {currentStageID}</color>");
        }

        SaveManager.Instance.CurrentData.SetStageProgress(currentStageID, currentWaveIndex);
        SaveManager.Instance.SaveGame(); 

        StartWave(currentStageID, currentWaveIndex);
    }
}
