using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [SerializeField] private GameObject enemyPrefab; // 적 기본 껍데기

    public void SpawnEnemy(int id)
    {

        var row = CSVManager.Instance.GetDataById("EnemyTable", "EnemyID", id.ToString());

        if (row == null) return;

        EnemyData newEnemyData = new EnemyData();
        newEnemyData.EnemyId = int.Parse(row["EnemyID"].ToString());
        newEnemyData.Name = row["Name"].ToString();
        newEnemyData.hp = float.Parse(row["HP"].ToString());
        newEnemyData.atk = float.Parse(row["ATK"].ToString());
        newEnemyData.def = float.Parse(row["DEF"].ToString());
        newEnemyData.moveSpeed = float.Parse(row["MoveSpeed"].ToString());
        newEnemyData.goldReward = float.Parse(row["GoldReward"].ToString());


        // 적오브젝트 소환 이후 스테이지 매니저에서 정보 받아와 위치 계산
        Vector2 spawnPos = Vector2.zero;
        GameObject obj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // 4. 소환된 적에게 데이터 주입
        obj.GetComponent<Enemy>().Init(newEnemyData);
    }
}
