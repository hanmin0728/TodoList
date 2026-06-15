using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class ShopUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shopLevelText;
    [SerializeField] private ShopPopUp shopPopUp;
    [SerializeField] private int oneSummonCost = 100;
    [SerializeField] private int tenSummonCost = 1000;

    private void Start()
    {
        UpdateShopLevelUI();
    }

    public void OnClickWeaponSummon()
    {
        ExecuteSummon(EquipmentType.Weapon, 1, oneSummonCost);
    }

    public void OnClickWeaponSummonTen()
    {
        ExecuteSummon(EquipmentType.Weapon, 10, tenSummonCost);
    }

    public void OnClickRingSummon()
    {
        ExecuteSummon(EquipmentType.Ring, 1, oneSummonCost);
    }

    public void OnClickRingSummonTen()
    {
        ExecuteSummon(EquipmentType.Ring, 10, tenSummonCost);
    }

    public void UpdateShopLevelUI()
    {
        int currentLevel = SaveManager.Instance.CurrentData.GetShopLevel();
        shopLevelText.SetText("Store Level {0}", currentLevel);
    }

    private void ExecuteSummon(EquipmentType equipmentType, int count, int cost)
    {
        if (!ShopManager.Instance.CanSummonEquipment())
        {
            Debug.LogWarning("[ShopUIManager] Shop is not ready yet.");
            return;
        }

        if (!CurrencyManager.Instance.TrySpendGem(cost))
        {
            return;
        }

        List<string> results = ShopManager.Instance.SummonItems(equipmentType, count);
        UpdateShopLevelUI();
        shopPopUp.ShowPopup(results, equipmentType);
    }
}
