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
    [SerializeField] private Image progressFill;      // 보유량 게이지 (Image Type: Filled)
    
    
    



    public EquipmentData Data { get; private set; }

    /// <summary>
    /// 인벤토리 생성 시 최초 1회 설정
    /// </summary>
    public void Setup(EquipmentData data)
    {
        Data = data;

        tierText.text = $"T{data.Tier}";
        SetGradeColor(data.Grade);
        UpdateUI();
    }

    /// <summary>
    /// 데이터가 변경되었을 때(합성, 소환 등) 호출하여 화면 갱신
    /// </summary>
    public void UpdateUI()
    {
        if (Data == null) return;

    }

    /// <summary>
    /// 슬롯 클릭 시 호출 (버튼 이벤트 연결)
    /// </summary>
    public void OnClickSlot()
    {
        Debug.Log($"{Data.Name} 클릭됨!");
    }

    private void SetGradeColor(GradeType grade)
    {
        switch (grade)
        {
            case GradeType.Normal: rankBGImage.color = Color.gray; break;
            case GradeType.Rare: rankBGImage.color = Color.blue; break;
            case GradeType.Epic: rankBGImage.color = Color.magenta; break;
            case GradeType.Legend: rankBGImage.color = new Color(1f, 0.5f, 0f); break; // 주황색
        }
    }
}
