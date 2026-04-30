using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUIManager : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;

    [Header("스크롤 뷰 오브젝트 (탭 전환용)")]
    [SerializeField] private GameObject weaponScrollView; 
    [SerializeField] private GameObject ringScrollView;
    [SerializeField] private Toggle weaponToggle; 
    [SerializeField] private Toggle ringToggle;   

    [Header("슬롯이 생성될 부모")]
    [SerializeField] private Transform weaponContent;
    [SerializeField] private Transform ringContent;


    private List<EquipmentSlotUI> allSlots = new List<EquipmentSlotUI>();

    private bool isCreated = false; // 🌟 중복 생성 방지 플래그
    void Start()
    {
        ChangeSubTab(0);

        if (EquipmentManager.Instance.IsInitialized)
        {
            InitSlots();
        }
        else
        {
            EquipmentManager.Instance.OnDataInitialized += InitSlots;
        }
    }

    private void InitSlots()
    {
        if (isCreated)
        {
            RefreshAllSlots();
            return;
        }

        EquipmentManager.Instance.OnDataInitialized -= InitSlots;

        var dataDic = EquipmentManager.Instance.EquipDataDic;

        foreach (var data in dataDic.Values)
        {
            Transform targetParent = (data.EquipType == EquipmentType.Weapon) ? weaponContent : ringContent;

            if (targetParent != null)
            {
                var go = Instantiate(slotPrefab, targetParent);
                var slot = go.GetComponent<EquipmentSlotUI>();
                slot.Setup(data); // 여기서 최초 1회 데이터 연결
                allSlots.Add(slot);
            }
        }

        Canvas.ForceUpdateCanvases();
        ResetScroll(weaponScrollView);

        isCreated = true;
        Debug.Log("<color=green>[UI]</color> 슬롯 최초 1회 생성 완료 (메모리 최적화)");
    }

    private void RefreshAllSlots()
    {
        foreach (var slot in allSlots)
        {
            slot.UpdateUI(); 
        }
    }

    //Weapon/Ring 토글에 연결 0이면 무기 1이면 반지
    public void ChangeSubTab(int index)
    {
        weaponScrollView.SetActive(index == 0);
        weaponToggle.isOn = (index == 0);

        ringScrollView.SetActive(index == 1);
        ringToggle.isOn = (index == 1);

        if (index == 0) ResetScroll(weaponScrollView);
        else ResetScroll(ringScrollView);
    }

    private void ResetScroll(GameObject scrollViewObj)
    {
        ScrollRect sr = scrollViewObj.GetComponent<ScrollRect>();
        if (sr != null)
        {
            sr.verticalNormalizedPosition = 1f; // 1.0f가 맨 위, 0.0f가 맨 아래입니다.
        }
    }
}
