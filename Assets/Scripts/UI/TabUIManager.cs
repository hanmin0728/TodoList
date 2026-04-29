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

            // 내가 누른 인덱스와 같으면 true(켜기), 다르면 false(끄기)
            tabPanels[i].SetActive(i == index);
        }

        Debug.Log($"<color=yellow>[TabSystem]</color> {index}번 패널 활성화 완료");
    }

}
