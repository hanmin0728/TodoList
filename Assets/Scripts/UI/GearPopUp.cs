using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GearPopUp : MonoBehaviour
{
    [Header("Top Info")]
    [SerializeField] private EquipmentSlotUI slot;
    [SerializeField] private TextMeshProUGUI gearNameText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Image rankImage;

    [Header("Stat Rows")]
    [SerializeField] private GameObject statRowPrefab;
    [SerializeField] private Transform equipContent;
    [SerializeField] private Transform passiveContent;

    [Header("Equip Button")]
    [SerializeField] private Button equipButton;
    [SerializeField] private TextMeshProUGUI equipButtonText;

    private readonly List<StatRowUI> equipStatRows = new List<StatRowUI>();
    private readonly List<StatRowUI> passiveStatRows = new List<StatRowUI>();

    private EquipmentData currentData;

    private void OnEnable()
    {
        EquipmentManager.EquipmentDataChanged += RefreshPopupUI;
    }

    private void OnDisable()
    {
        EquipmentManager.EquipmentDataChanged -= RefreshPopupUI;
    }

    public void OpenPopup(EquipmentData data)
    {
        currentData = data;

        slot.Setup(data);
        gearNameText.text = data.Name;
        rankText.text = data.Grade.ToString();
        rankImage.sprite = EquipmentManager.Instance.GradeData.GetSpriteBackground(data.Grade);

        RefreshPopupUI();
        gameObject.SetActive(true);
    }

    public void ClosePopUp()
    {
        gameObject.SetActive(false);
    }

    public void OnClickSynthesisButton()
    {
        if (currentData == null)
        {
            return;
        }

        if (EquipmentManager.Instance.Synthesize(currentData.ID))
        {
            RefreshPopupUI();
        }
    }

    public void OnClickEquipButton()
    {
        if (currentData == null)
        {
            return;
        }

        EquipmentManager.Instance.EquipItem(currentData.ID);
        RefreshEquipButtonState();
    }

    private void RefreshPopupUI()
    {
        if (currentData == null)
        {
            return;
        }

        slot.UpdateUI();
        RefreshEquipButtonState();
        RefreshStatList(currentData);
    }

    private void RefreshStatList(EquipmentData data)
    {
        int equipIndex = 0;
        int passiveIndex = 0;

        if (data.EquipStatType_1 != StatType.None)
        {
            SetStatRow(equipContent, equipStatRows, equipIndex++, data.EquipStatType_1, data.EquipStatValue_1);
        }

        if (data.EquipStatType_2 != StatType.None)
        {
            SetStatRow(equipContent, equipStatRows, equipIndex++, data.EquipStatType_2, data.EquipStatValue_2);
        }

        if (data.OwnStatType != StatType.None)
        {
            SetStatRow(passiveContent, passiveStatRows, passiveIndex++, data.OwnStatType, data.OwnStatValue);
        }

        SetRowsActive(equipStatRows, equipIndex);
        SetRowsActive(passiveStatRows, passiveIndex);
    }

    private void RefreshEquipButtonState()
    {
        SaveData saveData = SaveManager.Instance.CurrentData;
        string equippedId = saveData.GetEquippedID(currentData.EquipType.ToString());
        bool isEquipped = equippedId == currentData.ID;
        bool isUnlocked = saveData.IsUnlocked(currentData.ID);

        if (isEquipped)
        {
            equipButtonText.text = "장착 중";
            equipButton.interactable = false;
            return;
        }

        if (!isUnlocked)
        {
            equipButtonText.text = "미보유";
            equipButton.interactable = false;
            return;
        }

        equipButtonText.text = "장착";
        equipButton.interactable = true;
    }

    private void SetStatRow(Transform parent, List<StatRowUI> rows, int index, StatType type, double value)
    {
        StatRowUI row = GetOrCreateRow(parent, rows, index);
        row.gameObject.SetActive(true);
        row.Setup(type, value);
    }

    private StatRowUI GetOrCreateRow(Transform parent, List<StatRowUI> rows, int index)
    {
        if (index < rows.Count)
        {
            return rows[index];
        }

        GameObject rowObject = Instantiate(statRowPrefab, parent);
        StatRowUI row = rowObject.GetComponent<StatRowUI>();
        rows.Add(row);
        return row;
    }

    private static void SetRowsActive(List<StatRowUI> rows, int activeCount)
    {
        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].gameObject.SetActive(i < activeCount);
        }
    }
}
