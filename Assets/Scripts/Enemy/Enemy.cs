using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data;

    public float currentHp;

    public void Init(EnemyData newData)
    {
        data = newData;
        currentHp = data.hp; // 최대 체력 정보를 가져와 현재 체력 초기화
    }

}
