using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    public TextMeshProUGUI shopLevelText;
    public ShopPopUp shopPopUp;
    private int oneSummonCost = 100;
    private int tenSummonCost = 1000;
    private void Start()
    {
        UpdateShopLevelUI();
    }

    private void ExecuteSummon(string category, int count, int cost)
    {
        if (SaveManager.Instance.CurrentData.GetGem() < cost)
        {
            Debug.Log("다이아가 부족합니다!");
            // TODO: "재화가 부족합니다" 안내 팝업 
            return;
        }

        double gem = SaveManager.Instance.CurrentData.GetGem();
        SaveManager.Instance.CurrentData.SetGem(gem - cost);

        //소환 로직 실행
        List<string> results = ShopManager.Instance.SummonItems(category, count);

        //UI 갱신
        UpdateShopLevelUI();

        //연출 
        shopPopUp.ShowPopup(results);
    }

 
    public void OnClickWeaponSummon()
    {
        ExecuteSummon("Weapon", 1, oneSummonCost);
    }

    public void OnClickWeaponSummonTen()
    {
        ExecuteSummon("Weapon", 10, tenSummonCost);
    }

    public void OnClickRingSummon()
    {
        ExecuteSummon("Ring", 1, oneSummonCost);
    }

    public void OnClickRingSummonTen()
    {
        ExecuteSummon("Ring", 10, tenSummonCost);
    }


    public void UpdateShopLevelUI()
    {
        int currentLevel = SaveManager.Instance.CurrentData.GetShopLevel();
        shopLevelText.text = $"Store Level {currentLevel}";
    }
}
