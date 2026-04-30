using UnityEngine;
using DG.Tweening;
public class SynthesisButton : MonoBehaviour
{
    [SerializeField] private RectTransform hammerIcon; // 망치 아이콘의 RectTransform
    private Tween pulseTween;

    private void OnEnable()
    {
        EquipmentManager.OnEquipmentDataChanged += RefreshButtonState;
        RefreshButtonState();
    }

    private void OnDisable()
    {
        EquipmentManager.OnEquipmentDataChanged -= RefreshButtonState;
        StopPulse();
    }

    private void RefreshButtonState()
    {
        bool canSynthesize = EquipmentManager.Instance.CanSynthesizeAny();

        if (canSynthesize) StartPulse();
        else StopPulse();
    }
    private void StartPulse()
    {
        if (pulseTween != null) return;

        pulseTween = hammerIcon.DOScale(1.1f, 0.6f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
    private void StopPulse()
    {
        if (pulseTween != null)
        {
            pulseTween.Kill();
            pulseTween = null;
            hammerIcon.localScale = Vector3.one; 
        }
    }
}
