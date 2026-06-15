using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode] // 에디터에서도 즉시 확인 가능하게 함
public class ScrollViewHeightControl : MonoBehaviour
{
    public RectTransform content;     
    public LayoutElement layoutElement;
    public ScrollRect scrollRect;     

    [Header("높이 제한 설정")]
    public float maxHeight = 200f;     // 늘어나는 능력치 2개일때 높이
    void Update()
    {
        float contentHeight = content.rect.height;

        layoutElement.preferredHeight = Mathf.Min(contentHeight, maxHeight);

        // 2개까지는 스크롤 막고 3개부터 스크롤 시작
        scrollRect.vertical = contentHeight > maxHeight;

        if (contentHeight <= maxHeight)
        {
            content.anchoredPosition = Vector2.zero;
        }
    }
}
