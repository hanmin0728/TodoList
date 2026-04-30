using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private Image iconImage;          // 장비 아이콘
    [SerializeField] private Image rankBGImage;       // 등급별 배경 색상
    [SerializeField] private TextMeshProUGUI tierText; // 티어 표시 
    [SerializeField] private TextMeshProUGUI countText;// 보유량 텍스트
    [SerializeField] private Slider progressFillSlider;      // 보유량 게이지

    [SerializeField] private GameObject newBadge; // 새로 얻었을때 n 표시
    [SerializeField] private GameObject equippedBadge;

    public EquipmentData Data { get; private set; }

    public static Action<EquipmentData> OnSlotClicked;


    /// <summary>
    /// 인벤토리 생성 시 최초 1회 설정
    /// </summary>
    public void Setup(EquipmentData data)
    {
        Data = data;

        SetGradeColor(data.Grade);
        
        tierText.text = $"Tier {data.Tier}";

        iconImage.sprite = EquipmentManager.Instance.GetIcon(data);
        UpdateUI();
    }

    /// <summary>
    /// 데이터가 변경되었을 때 화면 갱신
    /// </summary>
    public void UpdateUI()
    {
        if (Data == null) return;

        int currentCount = SaveManager.Instance.CurrentData.GetEquipCount(Data.ID);
        int needCount = Data.NeedCount;

        countText.text = $"{currentCount}/{needCount}";
        progressFillSlider.value = (float)currentCount / Mathf.Max(1, needCount);

        bool isNew = SaveManager.Instance.CurrentData.IsNewItem(Data.ID);
        newBadge.SetActive(isNew && currentCount > 0);

        string typeKey = Data.EquipType.ToString();
        string equippedID = SaveManager.Instance.CurrentData.GetEquippedID(typeKey);

        // 현재 슬롯의 ID가 장착된 ID와 일치하는지 확인
        bool isEquipped = (Data.ID == equippedID);
        equippedBadge.SetActive(isEquipped);
    }

    /// <summary>
    /// 슬롯 클릭 시 호출 (버튼 이벤트 연결)
    /// </summary>
    public void OnClickSlot()
    {
        if (Data == null) return;

        if (SaveManager.Instance.CurrentData.IsNewItem(Data.ID))
        {
            SaveManager.Instance.CurrentData.SetNewStatus(Data.ID, false);
            SaveManager.Instance.SaveGame(); 
            UpdateUI(); 
        }

        OnSlotClicked?.Invoke(Data);
    }

    private void SetGradeColor(GradeType grade)
    {
        rankBGImage.sprite = EquipmentManager.Instance.gradeData.GetSprite(grade);
    }
}
