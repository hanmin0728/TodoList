using System;
using UnityEngine;

public sealed class CurrencyManager : Singleton<CurrencyManager>
{
    public event Action<double> OnGoldChanged;
    public event Action<double> OnGemChanged;

    public void AddGold(double amount)
    {
        if (amount <= 0)
        {
            return;
        }

        SaveData saveData = SaveManager.Instance.CurrentData;
        double nextGold = saveData.GetGold() + amount;
        saveData.SetGold(nextGold);

        OnGoldChanged?.Invoke(nextGold);
    }

    public void AddGem(double amount)
    {
        if (amount <= 0)
        {
            return;
        }

        SaveData saveData = SaveManager.Instance.CurrentData;
        double nextGem = saveData.GetGem() + amount;
        saveData.SetGem(nextGem);

        OnGemChanged?.Invoke(nextGem);
    }

    public bool TrySpendGold(double amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        SaveData saveData = SaveManager.Instance.CurrentData;
        double currentGold = saveData.GetGold();

        if (currentGold < amount)
        {
            Debug.Log("[CurrencyManager] Not enough gold.");
            return false;
        }

        double nextGold = currentGold - amount;
        saveData.SetGold(nextGold);
        OnGoldChanged?.Invoke(nextGold);
        return true;
    }

    public bool TrySpendGem(double amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        SaveData saveData = SaveManager.Instance.CurrentData;
        double currentGem = saveData.GetGem();

        if (currentGem < amount)
        {
            Debug.Log("[CurrencyManager] Not enough gems.");
            return false;
        }

        double nextGem = currentGem - amount;
        saveData.SetGem(nextGem);
        OnGemChanged?.Invoke(nextGem);
        return true;
    }

}
