using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ShopPopUp : MonoBehaviour
{
    [SerializeField] private Animator chestAnim;
    [SerializeField] private RawImage chestRawImage;
    public GameObject panel;

    public Transform slotParent;       // 슬롯들이 생성될 부모
    public GameObject slotPrefab;
   
    [Header("등급별 파티클 색상")]
    public Color normalColor = Color.green;
    public Color rareColor = Color.blue;
    public Color epicColor = new Color(0.5f, 0f, 0.5f); //보라
    public Color legendColor = Color.yellow;

    [SerializeField] private ParticleSystem[] glowParticles;

    [Header("연출 설정")]
    [SerializeField] private float newItemshakeDuration = 1.5f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float chestOpenDelay = 3f; // 상자 열리고 팝업 나올 때까지 대기 시간

    public float slotDelay = 0.2f;     // 슬롯 따다닥 간격

    [SerializeField] private Vector2 singleSlotSize = new Vector2(200, 200); // 1개 뽑을 때 크기
    [SerializeField] private Vector2 multiSlotSize = new Vector2(130, 130);  // 10개 뽑을 때 크기

    public void RefreshSettingUI()
    {
        panel.SetActive(false);

        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        Color resetColor = chestRawImage.color;
        resetColor.a = 1f;
        chestRawImage.color = resetColor;
    }

    public void ShowPopup(List<string> resultIDs)
    {
        gameObject.SetActive(true);
        
        RefreshSettingUI();

        chestRawImage.gameObject.SetActive(true);


        GridLayoutGroup grid = slotParent.GetComponent<GridLayoutGroup>();
        if (resultIDs.Count == 1)
        {
            grid.cellSize = singleSlotSize;
            grid.padding.bottom = 300;
        }
        else
        {
            grid.cellSize = grid.cellSize = singleSlotSize;
            grid.padding.bottom = 300; ;
            grid.padding.bottom = 500;
        }

        chestAnim.Play("ANIM_Chest_Royal_Intro", -1, 0f);

        GradeType maxGrade = GradeType.Normal;
        bool hasNewItem = false;

        foreach (string id in resultIDs)
        {
            var data = EquipmentManager.Instance.EquipDataDic[id];

            if (data.Grade > maxGrade)
                maxGrade = data.Grade;

            if (SaveManager.Instance.CurrentData.GetEquipCount(id) == 1)
                hasNewItem = true;
        }

        //등급에 따라 파티클 색상 설정
        SetParticleColor(maxGrade);

        // 연출
        StartCoroutine(ChestAnimationRoutine(resultIDs, hasNewItem));

    }

    private void SetParticleColor(GradeType maxGrade)
    {
        Color targetColor = normalColor;
        if (maxGrade == GradeType.Rare) targetColor = rareColor;
        else if (maxGrade == GradeType.Epic) targetColor = epicColor;
        else if (maxGrade == GradeType.Legend) targetColor = legendColor;

        foreach (ParticleSystem particle in glowParticles)
        {
            if (particle != null)
            {
                var main = particle.main;
                main.startColor = targetColor;
            }
        }
    }

    private IEnumerator ChestAnimationRoutine(List<string> resultIDs, bool hasNewItem)
    {
        // 신규 장비가 있다면 상자 오래 흔들기
        if (hasNewItem)
        {
            chestRawImage.rectTransform.DOShakeAnchorPos(newItemshakeDuration, 30f, 40);
            yield return new WaitForSeconds(newItemshakeDuration);
        }
        else
        {
            chestRawImage.rectTransform.DOShakeAnchorPos(shakeDuration, 20f, 30);
            yield return new WaitForSeconds(shakeDuration);
        }

        chestAnim.Play("ANIM_Chest_Royal_Open", -1, 0f);


        foreach (ParticleSystem particle in glowParticles)
        {
            if (particle != null) particle.Play();
        }

        //상자 애니메이션 보여줄 시간
        yield return new WaitForSeconds(chestOpenDelay);

        chestRawImage.DOFade(0f, 0.3f);
        yield return new WaitForSeconds(0.3f);

        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        foreach (string id in resultIDs)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotParent);
            EquipmentSlotUI slotUI = slotGO.GetComponent<EquipmentSlotUI>();

            if (EquipmentManager.Instance.EquipDataDic.TryGetValue(id, out var data))
            {
                slotUI.Setup(data, true);
                slotUI.UpdateUI();
            }

            slotGO.transform.localScale = Vector3.zero;
            slotGO.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(slotDelay);
        }
    }



}
