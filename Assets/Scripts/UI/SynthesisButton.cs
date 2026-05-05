using DG.Tweening;
using UnityEngine;

public sealed class SynthesisButton : MonoBehaviour
{
    [SerializeField] private RectTransform hammerIcon;

    private Tween pulseTween;

    private void OnEnable()
    {
        EquipmentManager.EquipmentDataChanged += RefreshButtonState;
        RefreshButtonState();
    }

    private void OnDisable()
    {
        EquipmentManager.EquipmentDataChanged -= RefreshButtonState;
        StopPulse();
    }

    private void RefreshButtonState()
    {
        bool canSynthesize = EquipmentManager.Instance != null && EquipmentManager.Instance.CanSynthesizeAny();

        if (canSynthesize)
        {
            StartPulse();
        }
        else
        {
            StopPulse();
        }
    }

    private void StartPulse()
    {
        if (pulseTween != null || hammerIcon == null)
        {
            return;
        }

        pulseTween = hammerIcon.DOScale(1.1f, 0.6f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopPulse()
    {
        if (pulseTween == null)
        {
            return;
        }

        pulseTween.Kill();
        pulseTween = null;

        if (hammerIcon != null)
        {
            hammerIcon.localScale = Vector3.one;
        }
    }
}
