using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class StageManager : Singleton<StageManager>
{
    [Header("Progress")]
    [SerializeField] private int currentStageID = 101;
    [SerializeField] private int currentWaveIndex = 1;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 0.2f;
    [SerializeField] private float normalWaveDelay = 1.5f;
    [SerializeField] private float bossClearDelay = 3.0f;

    private readonly Dictionary<int, StageData> stageDataByKey = new Dictionary<int, StageData>();
    private WaitForSeconds spawnIntervalWait;
    private WaitForSeconds normalWaveDelayWait;
    private WaitForSeconds bossClearDelayWait;
    private bool isWaveActive;

    public event Action<int, int> OnWaveChanged;

    public int GetCurrentStageID() => currentStageID;
    public int GetCurrentWaveIndex() => currentWaveIndex;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this)
        {
            return;
        }

        CacheYieldInstructions();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CacheYieldInstructions();
    }
#endif

    private void Start()
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

    public void StartWave(int stageID, int waveIndex)
    {
        if (isWaveActive)
        {
            return;
        }

        int key = MakeStageKey(stageID, waveIndex);
        if (!stageDataByKey.TryGetValue(key, out StageData currentWaveData))
        {
            Debug.Log("[StageManager] All stages cleared or stage data is missing.");
            return;
        }

        StartCoroutine(WaveRoutine(currentWaveData));
        OnWaveChanged?.Invoke(stageID, waveIndex);
    }

    private void InitStageData()
    {
        CSVManager.Instance.OnLoadingComplete -= InitStageData;
        stageDataByKey.Clear();

        List<Dictionary<string, object>> table = CSVManager.Instance.GetTable("StageTable");
        if (table == null)
        {
            Debug.LogError("[StageManager] StageTable is missing. Check CSVManager file names.");
            return;
        }

        foreach (Dictionary<string, object> row in table)
        {
            StageData data = new StageData
            {
                StageID = int.Parse(row["StageID"].ToString()),
                WaveIndex = int.Parse(row["WaveIndex"].ToString()),
                EnemyID = int.Parse(row["EnemyID"].ToString()),
                EnemyCount = int.Parse(row["EnemyCount"].ToString())
            };

            int key = MakeStageKey(data.StageID, data.WaveIndex);
            if (!stageDataByKey.ContainsKey(key))
            {
                stageDataByKey.Add(key, data);
            }
        }

        SaveData saveData = SaveManager.Instance.CurrentData;
        if (saveData != null)
        {
            currentStageID = saveData.GetStageID();
            currentWaveIndex = saveData.GetWaveIndex();
        }

        Debug.Log($"[StageManager] Stage table loaded. Count: {stageDataByKey.Count}");
        StartWave(currentStageID, currentWaveIndex);
    }

    private IEnumerator WaveRoutine(StageData data)
    {
        isWaveActive = true;

        for (int i = 0; i < data.EnemyCount; i++)
        {
            EnemySpawner.Instance.SpawnEnemy(data.EnemyID);
            yield return spawnIntervalWait;
        }

        while (EnemySpawner.Instance.GetActiveEnemyCount() > 0)
        {
            yield return null;
        }

        yield return data.IsBossWave ? bossClearDelayWait : normalWaveDelayWait;

        isWaveActive = false;
        NextWave();
    }

    private void NextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex > 4)
        {
            currentWaveIndex = 1;
            currentStageID++;
            Debug.Log($"[StageManager] Stage cleared. Next stage: {currentStageID}");
        }

        SaveManager.Instance.CurrentData.SetStageProgress(currentStageID, currentWaveIndex);
        SaveManager.Instance.SaveGame();
        StartWave(currentStageID, currentWaveIndex);
    }

    private void CacheYieldInstructions()
    {
        spawnIntervalWait = new WaitForSeconds(Mathf.Max(0f, spawnInterval));
        normalWaveDelayWait = new WaitForSeconds(Mathf.Max(0f, normalWaveDelay));
        bossClearDelayWait = new WaitForSeconds(Mathf.Max(0f, bossClearDelay));
    }

    private static int MakeStageKey(int stageID, int waveIndex)
    {
        return stageID * 100 + waveIndex;
    }
}
