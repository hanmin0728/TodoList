using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GearPopUp : MonoBehaviour
{
    [Header("상단 정보")]
    [SerializeField] private EquipmentSlotUI slot;
    [SerializeField] private TextMeshProUGUI gearNameText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Image rankImage;

    [Header("능력치 생성 설정")]
    [SerializeField] private GameObject statRowPrefab;   // 생성할 Stat_Row 프리팹
    [SerializeField] private Transform equipContent;     // 장착 효과 스크롤뷰의 Content
    [SerializeField] private Transform passiveContent;   // 패시브 효과 스크롤뷰의 Content


    [Header("장착 버튼 UI")]
    [SerializeField] private Button equipButton; // 인스펙터에서 장착 버튼 연결
    [SerializeField] private TextMeshProUGUI equipButtonText; // "장착" 글자 연결
    private EquipmentData data;
    private void OnEnable()
    {
        EquipmentManager.OnEquipmentDataChanged += RefreshPopupUI;
    }

    private void OnDisable()
    {
        EquipmentManager.OnEquipmentDataChanged -= RefreshPopupUI;
    }
    private void RefreshPopupUI()
    {
        if (data == null) return;

        slot.UpdateUI();

        RefreshEquipButtonState();

        RefreshStatList(data);
    }

    // 장비 슬롯을 눌렀을 때 호출될 함수
    public void OpenPopup(EquipmentData data)
    {
        this.data = data;

        slot.Setup(data);

        gearNameText.text = data.Name;
        rankText.text = data.Grade.ToString();

        rankImage.sprite = EquipmentManager.Instance.gradeData.GetSpriteBackground(data.Grade);

        RefreshPopupUI();
        gameObject.SetActive(true);
    }

    public void ClosePopUp()
    {
        gameObject.SetActive(false);
    }

    private void RefreshStatList(EquipmentData data)
    {
        ClearOldRows(equipContent);
        ClearOldRows(passiveContent);

        if (data.EquipStatType_1 != StatType.None)
        {
            CreateStatRow(equipContent, data.EquipStatType_1, data.EquipStatValue_1);
        }

        // 두 번째 장착 능력치
        if (data.EquipStatType_2 != StatType.None)
        {
            CreateStatRow(equipContent, data.EquipStatType_2, data.EquipStatValue_2);
        }

        // 3. 보유 능력치 (패시브) 생성
        if (data.OwnStatType != StatType.None)
        {
            CreateStatRow(passiveContent, data.OwnStatType, data.OwnStatValue);
        }
    }

    private void CreateStatRow(Transform parent, StatType type, double value)
    {
        GameObject go = Instantiate(statRowPrefab, parent);
        StatRowUI rowUI = go.GetComponent<StatRowUI>();
        rowUI.Setup(type.ToString(), value);
    }

    private void ClearOldRows(Transform content)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnClickSynthesisButton()
    {
        bool success = EquipmentManager.Instance.Synthesize(data.ID);

        if (success)
        {
            OpenPopup(data);
        }
        else
        {
        }
    }

    public void OnClickEquipButton()
    {
        if (data == null) return;

        // 장착 실행
        EquipmentManager.Instance.EquipItem(data.ID);

        // 장착 후 버튼 표기 다시 새로고침
        RefreshEquipButtonState();
    }

    /// <summary>
    /// 현재 장비가 장착 중인지 확인하여 버튼 텍스트 변경
    /// </summary>
    private void RefreshEquipButtonState()
    {
        string typeKey = data.EquipType.ToString();
        string equippedID = SaveManager.Instance.CurrentData.GetEquippedID(typeKey);

        // 1. 현재 보고 있는 장비를 장착 중인지 확인
        bool isEquipped = (equippedID == data.ID);
  
        bool wasAcquired = SaveManager.Instance.CurrentData.IsUnlocked(data.ID);

        if (isEquipped)
        {
            equipButtonText.text = "장착 중";
            equipButton.interactable = false;
        }
        else if (!wasAcquired)
        {
            // 딕셔너리에 ID가 아예 없으므로 한 번도 얻은 적 없는 상태
            equipButtonText.text = "미보유";
            equipButton.interactable = false;
        }
        else
        {
            // 딕셔너리에 ID가 존재한다면  장착 가능!
            equipButtonText.text = "장착";
            equipButton.interactable = true;
        }
    }
}
