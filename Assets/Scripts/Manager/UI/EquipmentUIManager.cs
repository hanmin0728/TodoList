using System.Collections.Generic;
using UnityEngine;

public class EquipmentUIManager : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform contentParent;

    private List<EquipmentSlotUI> allSlots = new List<EquipmentSlotUI>();

    void Start()
    {
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
        EquipmentManager.Instance.OnDataInitialized -= InitSlots;

        var dataDic = EquipmentManager.Instance.EquipDataDic;

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
        allSlots.Clear();


        foreach (var data in dataDic.Values)
        {
            var go = Instantiate(slotPrefab, contentParent);
            var slot = go.GetComponent<EquipmentSlotUI>();
            slot.Setup(data);
            allSlots.Add(slot);
        }
    }

    private void RefreshAllSlots()
    {
        foreach (var slot in allSlots)
        {
            slot.UpdateUI(); // 각 슬롯에게 "새 데이터로 다시 그려!"라고 명령
        }
    }
}
