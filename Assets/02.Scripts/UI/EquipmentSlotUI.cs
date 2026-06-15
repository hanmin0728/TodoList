using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class EquipmentSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image rankBGImage;
    [SerializeField] private TextMeshProUGUI tierText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Slider progressFillSlider;
    [SerializeField] private GameObject newBadge;
    [SerializeField] private GameObject equippedBadge;

    private bool isShopSlot;
    private string equipmentTypeKey;

    public static event Action<EquipmentData> SlotClicked;

    public EquipmentData Data { get; private set; }

    public void Setup(EquipmentData data, bool isShop = false)
    {
        Data = data;
        isShopSlot = isShop;
        equipmentTypeKey = GetEquipmentTypeKey(data.EquipType);

        if (rankBGImage != null)
        {
            rankBGImage.sprite = EquipmentManager.Instance.GradeData.GetSprite(data.Grade);
        }

        if (tierText != null)
        {
            tierText.text = $"Tier {data.Tier}";
        }

        if (iconImage != null)
        {
            iconImage.sprite = EquipmentManager.Instance.GetIcon(data);
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (Data == null)
        {
            return;
        }

        SaveData saveData = SaveManager.Instance.CurrentData;
        int currentCount = saveData.GetEquipCount(Data.ID);
        int needCount = Mathf.Max(1, Data.NeedCount);

        if (countText != null)
        {
            countText.text = $"{currentCount}/{needCount}";
        }

        if (progressFillSlider != null)
        {
            progressFillSlider.value = (float)currentCount / needCount;
        }

        if (newBadge != null)
        {
            newBadge.SetActive(saveData.IsNewItem(Data.ID) && currentCount > 0);
        }

        if (equippedBadge == null)
        {
            return;
        }

        if (isShopSlot)
        {
            equippedBadge.SetActive(false);
            return;
        }

        string equippedId = saveData.GetEquippedID(equipmentTypeKey);
        equippedBadge.SetActive(Data.ID == equippedId);
    }

    public void OnClickSlot()
    {
        if (Data == null)
        {
            return;
        }

        EquipmentManager.Instance.MarkItemAsSeen(Data.ID);
        SlotClicked?.Invoke(Data);
    }

    private static string GetEquipmentTypeKey(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                return "Weapon";
            case EquipmentType.Ring:
                return "Ring";
            default:
                return string.Empty;
        }
    }
}
