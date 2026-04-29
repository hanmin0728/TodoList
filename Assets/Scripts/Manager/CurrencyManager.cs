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
        double currentGold = SaveManager.Instance.CurrentData.GetGold();
        double nextGold = currentGold + amount;
        SaveManager.Instance.CurrentData.SetGold(nextGold);


        OnGoldChanged?.Invoke(nextGold);
    }
    
    public  bool TryUpgrade(double amount)
    {
        double currentGold = SaveManager.Instance.CurrentData.GetGold();

        if (currentGold >= amount)
        {
            double nextGold = currentGold - amount;
            SaveManager.Instance.CurrentData.SetGold(nextGold);

            OnGoldChanged?.Invoke(nextGold);
            return true;
        }
        else
        {
            Debug.Log("재화 부족");
            return false;
        }
    }
}
