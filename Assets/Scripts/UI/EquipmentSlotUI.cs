using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private Image iconImage;          // 장비 아이콘
    [SerializeField] private Image rankBGImage;       // 등급별 배경 색상
    [SerializeField] private TextMeshProUGUI tierText; // 티어 표시 (예: T1, T2)
    [SerializeField] private TextMeshProUGUI countText;// 보유량 텍스트 (예: 5/5)
    [SerializeField] private Slider progressFillSlider;      // 보유량 게이지 (Image Type: Filled)

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

        Debug.Log(Data.ID +" s    "  +  currentCount);


        countText.text = $"{currentCount}/{needCount}";
        progressFillSlider.value = (float)currentCount / Mathf.Max(1, needCount);
    }

    /// <summary>
    /// 슬롯 클릭 시 호출 (버튼 이벤트 연결)
    /// </summary>
    public void OnClickSlot()
    {
        if (Data == null) return;

        OnSlotClicked?.Invoke(Data);
    }

    private void SetGradeColor(GradeType grade)
    {
        rankBGImage.sprite = EquipmentManager.Instance.gradeData.GetSprite(grade);
    }
}
