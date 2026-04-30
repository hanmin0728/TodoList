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

    private EquipmentData data;

    // 장비 슬롯을 눌렀을 때 호출될 함수
    public void OpenPopup(EquipmentData data)
    {
        this.data = data;

        slot.Setup(data);

        gearNameText.text = data.Name;
        rankText.text = data.Grade.ToString();

        rankImage.sprite = EquipmentManager.Instance.gradeData.GetSpriteBackground(data.Grade);

        RefreshStatList(data);

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
            // "재료가 부족합니다" 같은 알림 띄우기
        }
    }
}
