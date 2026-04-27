using System;
using UnityEngine;

public class CurrencyManager : Singleton<CurrencyManager>
{
    // 골드가 변할 때마다 UI나 다른 스크립트에게 알려주는 Event
    public Action<double> OnGoldChanged;

    /// <summary>
    /// 골드 획득시 호출
    /// </summary>
    public void AddGold(long amount)
    {
        SaveManager.Instance.CurrentData.Gold += amount;

        OnGoldChanged?.Invoke(SaveManager.Instance.CurrentData.Gold);
    }
    
    public  bool TryUpgrade(double amount)
    {
        if (SaveManager.Instance.CurrentData.Gold >= amount)
        {
            SaveManager.Instance.CurrentData.Gold -= amount;
            OnGoldChanged?.Invoke(SaveManager.Instance.CurrentData.Gold);
            return true;
        }
        else
        {
            Debug.Log("재화 부족");
            return false;
        }
    }
}
