using System;
using UnityEngine;

public class CurrencyManager : Singleton<CurrencyManager>
{
    // 골드가 변할 때마다 UI나 다른 스크립트에게 알려주는 Event
    public Action<long> OnGoldChanged;

    /// <summary>
    /// 골드 획득시 호출
    /// </summary>
    public void AddGold(long amount)
    {
        SaveManager.Instance.CurrentData.Gold += amount;

        // 골드가 올랐다고 방송을 켭니다. (UI가 듣고 알아서 숫자를 바꿈!)
        OnGoldChanged?.Invoke(SaveManager.Instance.CurrentData.Gold);
    }
}
