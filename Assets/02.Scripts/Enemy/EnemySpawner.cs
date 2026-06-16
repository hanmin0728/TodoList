using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemySpawner : Singleton<EnemySpawner>
{
    private readonly Dictionary<int, GameObject> prefabCacheById = new Dictionary<int, GameObject>();
    private readonly Dictionary<int, EnemyData> enemyDataById = new Dictionary<int, EnemyData>();

    private int activeEnemyCount;
    private bool isEnemyDataLoaded;

    private void Start()
    {
        if (CSVManager.Instance.IsInitialized)
        {
            LoadEnemyData();
        }
        else
        {
            CSVManager.Instance.OnLoadingComplete += LoadEnemyData;
        }
    }

    public void SpawnEnemy(int id)
    {
        if (!EnsureEnemyDataLoaded())
        {
            Debug.LogWarning("[EnemySpawner] Enemy data is not ready yet.");
            return;
        }

        if (!enemyDataById.TryGetValue(id, out EnemyData enemyData))
        {
            Debug.LogError($"[EnemySpawner] Enemy data is missing. ID: {id}");
            return;
        }

        GameObject targetPrefab = GetEnemyPrefab(id);
        if (targetPrefab == null)
        {
            return;
        }

        Vector2 spawnPosition = GameManager.Instance.EnemySpawnPosition();
        GameObject spawnedObject = PoolManager.Instance.Spawn(targetPrefab, spawnPosition, Quaternion.identity);

        if (!spawnedObject.TryGetComponent(out EnemyBase enemy))
        {
            Debug.LogError($"[EnemySpawner] Enemy prefab does not have EnemyBase. Prefab: {targetPrefab.name}");
            Poolable poolable = spawnedObject.GetComponent<Poolable>();
            if (poolable != null)
            {
                poolable.Release();
            }

            return;
        }

        enemy.SpawnInit(enemyData);
        activeEnemyCount++;
    }

    public int GetActiveEnemyCount()
    {
        return activeEnemyCount;
    }

    public void OnEnemyDeath()
    {
        activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
    }

    private bool EnsureEnemyDataLoaded()
    {
        if (isEnemyDataLoaded)
        {
            return true;
        }

        if (!CSVManager.Instance.IsInitialized)
        {
            return false;
        }

        LoadEnemyData();
        return isEnemyDataLoaded;
    }

    private void LoadEnemyData()
    {
        CSVManager.Instance.OnLoadingComplete -= LoadEnemyData;
        enemyDataById.Clear();

        List<Dictionary<string, object>> table = CSVManager.Instance.GetTable("EnemyTable");
        if (table == null)
        {
            Debug.LogError("[EnemySpawner] EnemyTable is missing. Check CSVManager file names.");
            return;
        }

        foreach (Dictionary<string, object> row in table)
        {
            try
            {
                EnemyData data = ParseEnemyData(row);
                if (!enemyDataById.ContainsKey(data.EnemyId))
                {
                    enemyDataById.Add(data.EnemyId, data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[EnemySpawner] Failed to parse enemy data. ID: {row["EnemyID"]}, Error: {e.Message}");
            }
        }

        isEnemyDataLoaded = true;
    }

    private static EnemyData ParseEnemyData(Dictionary<string, object> row)
    {
        return new EnemyData
        {
            EnemyId = int.Parse(row["EnemyID"].ToString()),
            Name = row["Name"].ToString(),
            Hp = float.Parse(row["HP"].ToString()),
            HpGrow = float.Parse(row["HP_Grow"].ToString()),
            Atk = float.Parse(row["ATK"].ToString()),
            AtkGrow = float.Parse(row["ATK_Grow"].ToString()),
            MoveSpeed = float.Parse(row["MoveSpeed"].ToString()),
            AttackRange = float.Parse(row["AttackRange"].ToString()),
            AttackDelay = float.Parse(row["AttackDelay"].ToString()),
            GoldReward = long.Parse(row["GoldReward"].ToString()),
            VariationMin = int.Parse(row["VariationMin"].ToString()),
            VariationMax = int.Parse(row["VariationMax"].ToString()),
            IsBoss = bool.Parse(row["IsBoss"].ToString())
        };
    }

    private GameObject GetEnemyPrefab(int id)
    {
        if (prefabCacheById.TryGetValue(id, out GameObject cachedPrefab))
        {
            return cachedPrefab;
        }

        EnemyType enemyType = (EnemyType)id;
        string path = $"Enemies/{enemyType}";
        GameObject loadedPrefab = Resources.Load<GameObject>(path);

        if (loadedPrefab == null)
        {
            Debug.LogError($"[EnemySpawner] Enemy prefab is missing. Resources/{path}, ID: {id}");
            return null;
        }

        prefabCacheById.Add(id, loadedPrefab);
        return loadedPrefab;
    }
}


