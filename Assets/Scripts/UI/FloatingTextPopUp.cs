using DG.Tweening;
using TMPro;
using UnityEngine;
public enum TextType
{
    NormalDamage,
    CriticalDamage,
    GoldDrop,
    Heal
}

public class FloatingTextPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private Poolable _poolable;
    private void Awake()
    {
        _poolable = GetComponent<Poolable>();
    }

    public void Setup(string content, TextType type)
    {
        text.text = content;
        text.alpha = 1f;

        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        // 타입에 따라 다른 연출 적용
        switch (type)
        {
            case TextType.NormalDamage:
                text.color = Color.yellow;
                seq.Append(transform.DOScale(0.006f, 0.006f).SetEase(Ease.OutBack));
                seq.Join(transform.DOLocalMoveY(transform.position.y + 0.25f, 0.4f));
                break;

            case TextType.GoldDrop:
                break;
        }

        // 공통 마무리 (사라지고 풀로 돌아가기)
        seq.Insert(0.5f, text.DOFade(0f, 0.3f));
        seq.OnComplete(() =>
        {
            _poolable.Release();
        });
    }
}
