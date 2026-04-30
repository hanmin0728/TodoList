using UnityEngine;

public class TabUIManager : MonoBehaviour
{
    [Header("탭 패널들 (Character, Skill, Gear, Store 순서)")]
    [SerializeField] private GameObject[] tabPanels;

    void Start()
    {
        OpenTab(0);
    }
    public void OpenTab(int index)
    {
        if (index < 0 || index >= tabPanels.Length) return;

        for (int i = 0; i < tabPanels.Length; i++)
        {
            if (tabPanels[i] == null) continue;
            tabPanels[i].SetActive(i == index);
        }
    }

}
