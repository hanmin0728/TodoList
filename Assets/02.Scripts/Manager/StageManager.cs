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

    public event Action<float> OnBossEnter;     
    public event Action OnBossCleared;            
    public event Action<float> OnBossHpChanged;
    private const float BossTimeLimit = 15f; // 보스전 제한 시간

    public event Action<float, float> OnBossTimerUpdated; 
    public event Action OnBossFailed;

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
      
        if (isBossWave)
        {
            // 4웨이브면 무조건 보스전 상태
            currentState = StageState.Boss;
        }
        else
        {
            // 4웨이브가 아닐 때, 현재 보스 실패로 인한 BossFail 상태라면 상태를 Normal로 바꾸지 않고 그대로 유지
            if (currentState != StageState.BossFail)
            {
                currentState = StageState.Normal;
            }
        }

        if (currentState == StageState.Boss)
        {
            float maxHp = EnemySpawner.Instance.GetEnemyMaxHp(data.EnemyID);
            EnterBoss(maxHp);
        }

        for (int i = 0; i < data.EnemyCount; i++)
        {
            EnemySpawner.Instance.SpawnEnemy(data.EnemyID);
            yield return spawnIntervalWait;
        }
        
        float currentBossTime = BossTimeLimit;

        while (EnemySpawner.Instance.GetActiveEnemyCount() > 0)
        {
            if (currentState == StageState.Boss)
            {
                currentBossTime -= Time.deltaTime;
                OnBossTimerUpdated?.Invoke(currentBossTime, BossTimeLimit);

                // 타임 오버 체크
                if (currentBossTime <= 0f)
                {
                    FailBoss();
                    yield break; 
                }
            }

            yield return null;
        }

        if (currentState == StageState.Boss)
        {
            ClearBoss();
        }

        yield return isBossWave ? bossClearDelayWait : normalWaveDelayWait;

        isWaveActive = false;
        NextWave();
    }
    private void EnterBoss(float maxHp)
    {
        Debug.Log("[StageManager] 보스 웨이브 진입!");
        SoundManager.Instance.PlayBGM(SoundEnum.BgmType.Boss);

        OnBossEnter?.Invoke(maxHp);
    }
    private void ClearBoss()
    {
        Debug.Log("[StageManager] 보스 처치 성공!");
        SoundManager.Instance.PlayBGM(SoundEnum.BgmType.Main);
        OnBossCleared?.Invoke();
    }
    private void FailBoss()
    {
        currentState = StageState.BossFail;
        Debug.Log("[StageManager] 보스 처치 실패! 1웨이브부터 재시작 및 반복 진행!");
        SoundManager.Instance.PlayBGM(SoundEnum.BgmType.Main);
        OnBossFailed?.Invoke();
        EnemySpawner.Instance.ClearAllActiveEnemies(); 

        isWaveActive = false;
        NextWave();
    }
    public void NotifyBossHpChanged(float currentHp)
    {
        OnBossHpChanged?.Invoke(currentHp);
    }

    /// <summary>
    /// 현재 진행 중인 루프를 즉시 중단하고 보스전 진행
    /// </summary>
    public void RequestBossRetry()
    {
        if (currentState != StageState.BossFail)
        {
            return;
        }

        Debug.Log("[StageManager] 보스 재도전 요청! 현재 웨이브를 중단하고 보스전으로 진입합니다.");

        StopAllCoroutines();
        isWaveActive = false; 

        EnemySpawner.Instance.ClearAllActiveEnemies();

        currentWaveIndex = 4;

        StartWave(currentStageID, currentWaveIndex);
    }

    public void ResetToFirstWave()
    {
        StopAllCoroutines(); // 진행 중이던 웨이브 루틴 강제 정지
        isWaveActive = false;
        EnemySpawner.Instance.ClearAllActiveEnemies();
        currentState = StageState.Normal; 
        currentWaveIndex = 1;             

        SaveManager.Instance.CurrentData.SetStageProgress(currentStageID, currentWaveIndex);
        SaveManager.Instance.SaveGameSync();

        Debug.Log("[StageManager] 게임 다시 시작");

        StartWave(currentStageID, currentWaveIndex); 
    }

    public void StopWave()
    {
        StopAllCoroutines();
        isWaveActive = false;
        EnemySpawner.Instance.ClearAllActiveEnemies();

        Debug.Log("[StageManager] 웨이브 중단 및 필드 청소");
    }
    private void NextWave()
    {
        if (currentState == StageState.BossFail)
        {
            currentWaveIndex++;

            // 1~3 웨이브만 무한 반복
            if (currentWaveIndex > 3)
            {
                currentWaveIndex = 1;
            }

            SaveManager.Instance.CurrentData.SetStageProgress(currentStageID, currentWaveIndex);
            SaveManager.Instance.SaveGameSync();
            StartWave(currentStageID, currentWaveIndex);
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
