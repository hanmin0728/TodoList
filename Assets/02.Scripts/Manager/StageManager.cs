using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public enum StageState
{
    Normal,
    Boss,
    BossFail
}

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

    private StageState currentState = StageState.Normal;
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

        bool isBossWave = (data.WaveIndex == 4);
        currentState = isBossWave ? StageState.Boss : StageState.Normal;

        if (currentState == StageState.Boss)
        {
            EnterBoss();
        }

        for (int i = 0; i < data.EnemyCount; i++)
        {
            EnemySpawner.Instance.SpawnEnemy(data.EnemyID);
            yield return spawnIntervalWait;
        }

        while (EnemySpawner.Instance.GetActiveEnemyCount() > 0)
        {
            yield return null;
        }

        if (currentState == StageState.Boss)
        {
            ClearBoss();
        }

        yield return data.IsBossWave ? bossClearDelayWait : normalWaveDelayWait;

        isWaveActive = false;
        NextWave();
    }
    private void EnterBoss()
    {
        Debug.Log("[StageManager] 보스 웨이브 진입!");
        SoundManager.Instance.PlayBGM(SoundEnum.BgmType.Boss);

    }
    private void ClearBoss()
    {
        Debug.Log("[StageManager] 보스 처치 성공!");
        SoundManager.Instance.PlayBGM(SoundEnum.BgmType.Main);
    }

    private void NextWave()
    {
        if (currentState == StageState.BossFail)
        {
            Debug.Log("[StageManager] 보스전 패배. 재도전 대기 중...");
            return;
        }

        currentWaveIndex++;

        if (currentWaveIndex > 4)
        {
            currentWaveIndex = 1;
            currentStageID++;
            Debug.Log($"[StageManager] Stage cleared. Next stage: {currentStageID}");
        }

        SaveManager.Instance.CurrentData.SetStageProgress(currentStageID, currentWaveIndex);
        SaveManager.Instance.SaveGameSync();
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
