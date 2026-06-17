using DG.Tweening;
using TMPro;
using UnityEngine;

public enum TextType
{
    NormalDamage,
    CriticalDamage,
    GoldDrop,
    NicknameWarning 
}

public class FloatingTextPopUp : MonoBehaviour
{
    private const float DamageTextScale = 0.0082f;
    private const float DamageMoveY = 0.25f;
    private const float DamageMoveDuration = 0.4f;
    private const float FadeDelay = 0.5f;
    private const float FadeDuration = 0.3f;

    private const float NicknameWaringTextScale = 1f;
    private const float NicknameWaringMoveY = 0.25f;
    private const float NicknameWaringDuration = 0.4f;


    [SerializeField] private TextMeshProUGUI text;

    private Poolable poolable;
    private TweenCallback releaseCallback;

    private void Awake()
    {
        poolable = GetComponent<Poolable>();
        releaseCallback = Release;
    }

    public void Setup(float value, TextType type)
    {
        text.SetText("{0:0}", value);
        Play(type);
    }

    public void Setup(string content, TextType type)
    {
        text.SetText(content);
        Play(type);
    }

    private void Play(TextType type)
    {
        transform.DOKill();
        text.DOKill();
        text.alpha = 1f;

        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        ApplyStyle(type, sequence);

        sequence.Insert(FadeDelay, text.DOFade(0f, FadeDuration));
        sequence.OnComplete(releaseCallback);
    }

    private void ApplyStyle(TextType type, Sequence sequence)
    {
        switch (type)
        {
            case TextType.NormalDamage:
                text.color = Color.yellow;
                AppendDamageTween(sequence);
                break;
            case TextType.CriticalDamage:
                text.color = Color.red;
                AppendDamageTween(sequence);
                break;
            case TextType.GoldDrop:
            case TextType.NicknameWarning:
                text.color = Color.red;
                AppendNicknameWarningTween(sequence);
                break;
        }
    }

    private void AppendDamageTween(Sequence sequence)
    {
        sequence.Append(transform.DOScale(DamageTextScale, DamageTextScale).SetEase(Ease.OutBack));
        sequence.Join(transform.DOLocalMoveY(DamageMoveY, DamageMoveDuration));
    }

    private void AppendNicknameWarningTween(Sequence sequence)
    {
        sequence.Append(transform.DOScale(NicknameWaringTextScale, NicknameWaringTextScale).SetEase(Ease.OutBack));
        sequence.Join(transform.DOLocalMoveY(NicknameWaringMoveY, NicknameWaringDuration));
    }

    private void Release()
    {
        if (poolable != null)
        {
            poolable.Release();
        }
    }
}
