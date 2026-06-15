using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class EquipmentUIManager : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;

    [Header("Scroll Views")]
    [SerializeField] private GameObject weaponScrollView;
    [SerializeField] private GameObject ringScrollView;
    [SerializeField] private Toggle weaponToggle;
    [SerializeField] private Toggle ringToggle;

    [Header("Slot Parents")]
    [SerializeField] private Transform weaponContent;
    [SerializeField] private Transform ringContent;

    [SerializeField] private GearPopUp gearPopUp;

    private readonly List<EquipmentSlotUI> allSlots = new List<EquipmentSlotUI>();
    private ScrollRect weaponScrollRect;
    private ScrollRect ringScrollRect;
    private bool isCreated;

    private void Awake()
    {
        if (weaponScrollView != null)
        {
            weaponScrollRect = weaponScrollView.GetComponent<ScrollRect>();
        }

        if (ringScrollView != null)
        {
            ringScrollRect = ringScrollView.GetComponent<ScrollRect>();
        }
    }

    private void OnEnable()
    {
        EquipmentSlotUI.SlotClicked += ShowGearPopup;
        EquipmentManager.EquipmentDataChanged += RefreshAllSlots;
    }

    private void OnDisable()
    {
        EquipmentSlotUI.SlotClicked -= ShowGearPopup;
        EquipmentManager.EquipmentDataChanged -= RefreshAllSlots;
    }

    private void Start()
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

    public void ChangeSubTab(int index)
    {
        bool isWeaponTab = index == 0;

        weaponScrollView.SetActive(isWeaponTab);
        ringScrollView.SetActive(!isWeaponTab);
        weaponToggle.isOn = isWeaponTab;
        ringToggle.isOn = !isWeaponTab;

        ResetScroll(isWeaponTab ? weaponScrollRect : ringScrollRect);
    }

    private void InitSlots()
    {
        EquipmentManager.Instance.OnDataInitialized -= InitSlots;

        if (isCreated)
        {
            RefreshAllSlots();
            return;
        }

        foreach (EquipmentData data in EquipmentManager.Instance.GetAllEquipmentData())
        {
            Transform targetParent = data.EquipType == EquipmentType.Weapon ? weaponContent : ringContent;
            if (targetParent == null)
            {
                continue;
            }

            GameObject slotObject = Instantiate(slotPrefab, targetParent);
            EquipmentSlotUI slot = slotObject.GetComponent<EquipmentSlotUI>();
            slot.Setup(data);
            allSlots.Add(slot);
        }

        Canvas.ForceUpdateCanvases();
        ResetScroll(weaponScrollRect);
        isCreated = true;
    }

    private void RefreshAllSlots()
    {
        for (int i = 0; i < allSlots.Count; i++)
        {
            allSlots[i].UpdateUI();
        }
    }

    private void ShowGearPopup(EquipmentData data)
    {
        gearPopUp.OpenPopup(data);
    }

    private static void ResetScroll(ScrollRect scrollRect)
    {
        if (scrollRect == null)
        {
            return;
        }

        scrollRect.verticalNormalizedPosition = 1f;
    }
}
