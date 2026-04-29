using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    // 성능 최적화를 위한 프리팹 캐싱 
    private Dictionary<int, GameObject> prefabCache = new Dictionary<int, GameObject>();
    
    public int activeEnemyCount = 0;
    
    public void SpawnEnemy(int id)
    {
        var row = CSVManager.Instance.GetDataById("EnemyTable", "EnemyID", id.ToString());

        if (row == null)
        {
            Debug.LogError($"EnemyID {id}의 데이터를 찾을 수 없습니다.");
            return;
        }
 
        GameObject targetPrefab = GetEnemyPrefab(id);

        if (targetPrefab == null) 
            return;

        EnemyData newEnemyData = new EnemyData();
        newEnemyData.EnemyId = int.Parse(row["EnemyID"].ToString());
        newEnemyData.Name = row["Name"].ToString();
        newEnemyData.hp = float.Parse(row["HP"].ToString());
        newEnemyData.atk = float.Parse(row["ATK"].ToString());
        newEnemyData.def = float.Parse(row["DEF"].ToString());
        newEnemyData.moveSpeed = float.Parse(row["MoveSpeed"].ToString());
        newEnemyData.attackRange = float.Parse(row["AttackRange"].ToString());
        newEnemyData.attackDelay = float.Parse(row["AttackDelay"].ToString());
        newEnemyData.goldReward = long.Parse(row["GoldReward"].ToString());

        Vector2 spawnPos = GameManager.Instance.GetSpawnPosition();
        GameObject obj = PoolManager.Instance.Spawn(targetPrefab, spawnPos, Quaternion.identity);
        obj.GetComponent<EnemyBase>().Init(newEnemyData);

        activeEnemyCount++;
    }
  
    /// <summary>
    /// ID를 기반으로 프리팹을 로드하고 캐싱하는 함수
    /// </summary>
    private GameObject GetEnemyPrefab(int id)
    {
        // 아직 캐시에 없다면 Resources 폴더에서 불러옴
        if (!prefabCache.ContainsKey(id))
        {
            EnemyType enemyType = (EnemyType)id;
            string prefabName = enemyType.ToString();

            string path = $"Enemies/{prefabName}";

            GameObject loadedPrefab = Resources.Load<GameObject>(path);

            if (loadedPrefab == null)
            {
                Debug.LogError($"경로에 프리팹이 없습니다! : Resources/{path} (ID: {id}가 Enum에 정의되어 있는지 확인하세요!)");
                return null;
            }

            // 찾은 프리팹을 딕셔너리에 저장
            prefabCache[id] = loadedPrefab;
        }

        return prefabCache[id];
    }

    public int GetActiveEnemyCount()
    {
        return activeEnemyCount;
    }
    public void OnEnemyDeath()
    {
        activeEnemyCount--;

        if (activeEnemyCount <= 0)
        {
            activeEnemyCount = 0;
        }
    }
}
