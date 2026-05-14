using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShopPopUp : MonoBehaviour
{
    [SerializeField] private Animator chestAnim;
    [SerializeField] private RawImage chestRawImage;
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefab;

    [Header("Grade Particle Colors")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color rareColor = Color.blue;
    [SerializeField] private Color epicColor = new Color(0.5f, 0f, 0.5f);
    [SerializeField] private Color legendColor = Color.yellow;
    [SerializeField] private ParticleSystem[] glowParticles;

    [Header("Animation Settings")]
    [SerializeField] private float newItemshakeDuration = 1.5f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float chestOpenDelay = 3f;
    [SerializeField] private float slotDelay = 0.2f;
    [SerializeField] private Vector2 singleSlotSize = new Vector2(200, 200);
    [SerializeField] private Vector2 multiSlotSize = new Vector2(130, 130);

    private readonly List<EquipmentSlotUI> resultSlots = new List<EquipmentSlotUI>();
    private GridLayoutGroup gridLayoutGroup;
    private Coroutine popupRoutine;

    private readonly int animIntroHash = Animator.StringToHash("ANIM_Chest_Royal_Intro");
    private readonly int animOpenHash = Animator.StringToHash("ANIM_Chest_Royal_Open");
    
    private Vector2 initialChestAnchorPos;

    private void Awake()
    {
        gridLayoutGroup = slotParent.GetComponent<GridLayoutGroup>();
        initialChestAnchorPos = chestRawImage.rectTransform.anchoredPosition;
    }

    public void RefreshSettingUI()
    {
        if (popupRoutine != null)
        {
            StopCoroutine(popupRoutine);
            popupRoutine = null;
        }

        panel.SetActive(false);
        SetResultSlotsActive(0);

        chestRawImage.DOKill();
        chestRawImage.rectTransform.anchoredPosition = initialChestAnchorPos;
        chestRawImage.gameObject.SetActive(true);

        Color resetColor = chestRawImage.color;
        resetColor.a = 1f;
        chestRawImage.color = resetColor;
    }

    public void ShowPopup(List<string> resultIDs)
    {
        gameObject.SetActive(true);
        RefreshSettingUI();
        ApplyGridLayout(resultIDs.Count);

        chestAnim.Play(animIntroHash, -1, 0f);

        GradeType maxGrade = GradeType.Normal;
        bool hasNewItem = false;

        for (int i = 0; i < resultIDs.Count; i++)
        {
            if (!EquipmentManager.Instance.TryGetEquipmentData(resultIDs[i], out EquipmentData data))
            {
                continue;
            }

            if (data.Grade > maxGrade)
            {
                maxGrade = data.Grade;
            }

            if (SaveManager.Instance.CurrentData.GetEquipCount(resultIDs[i]) == 1)
            {
                hasNewItem = true;
            }
        }

        SetParticleColor(maxGrade);
        popupRoutine = StartCoroutine(ChestAnimationRoutine(resultIDs, hasNewItem));
    }

    private IEnumerator ChestAnimationRoutine(List<string> resultIDs, bool hasNewItem)
    {
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

        chestAnim.Play(animOpenHash, -1, 0f);
        PlayGlowParticles();

        yield return new WaitForSeconds(chestOpenDelay);

        chestRawImage.DOFade(0f, 0.3f);
        yield return new WaitForSeconds(0.3f);

        panel.SetActive(true);
        panel.transform.DOKill();
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        for (int i = 0; i < resultIDs.Count; i++)
        {
            if (!EquipmentManager.Instance.TryGetEquipmentData(resultIDs[i], out EquipmentData data))
            {
                continue;
            }

            EquipmentSlotUI slot = GetOrCreateResultSlot(i);
            Transform slotTransform = slot.transform;

            slot.gameObject.SetActive(true);
            slot.Setup(data, true);
            slotTransform.DOKill();
            slotTransform.localScale = Vector3.zero;
            slotTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(slotDelay);
        }

        SetResultSlotsActive(resultIDs.Count);
        popupRoutine = null;
    }

    private EquipmentSlotUI GetOrCreateResultSlot(int index)
    {
        if (index < resultSlots.Count)
        {
            return resultSlots[index];
        }

        GameObject slotObject = Instantiate(slotPrefab, slotParent);
        EquipmentSlotUI slot = slotObject.GetComponent<EquipmentSlotUI>();
        resultSlots.Add(slot);
        return slot;
    }

    private void SetResultSlotsActive(int activeCount)
    {
        for (int i = 0; i < resultSlots.Count; i++)
        {
            resultSlots[i].gameObject.SetActive(i < activeCount);
        }
    }

    private void ApplyGridLayout(int resultCount)
    {
        if (gridLayoutGroup == null)
        {
            return;
        }

        bool isSingleResult = resultCount == 1;
        gridLayoutGroup.cellSize = isSingleResult ? singleSlotSize : multiSlotSize;
        gridLayoutGroup.padding.bottom = isSingleResult ? 300 : 500;
    }

    private void SetParticleColor(GradeType maxGrade)
    {
        Color targetColor = normalColor;

        if (maxGrade == GradeType.Rare)
        {
            targetColor = rareColor;
        }
        else if (maxGrade == GradeType.Epic)
        {
            targetColor = epicColor;
        }
        else if (maxGrade == GradeType.Legend)
        {
            targetColor = legendColor;
        }

        for (int i = 0; i < glowParticles.Length; i++)
        {
            ParticleSystem particle = glowParticles[i];
            if (particle == null)
            {
                continue;
            }

            ParticleSystem.MainModule main = particle.main;
            main.startColor = targetColor;
        }
    }

    private void PlayGlowParticles()
    {
        for (int i = 0; i < glowParticles.Length; i++)
        {
            if (glowParticles[i] != null)
            {
                glowParticles[i].Play();
            }
        }
    }


}
