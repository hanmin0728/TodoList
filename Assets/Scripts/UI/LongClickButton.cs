using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
   private bool isPressed = false;
    
    [Header("설정")]
    public float initialDelay = 0.5f;   // 처음 연속 강화를 시작할 때까지의 대기 시간
    public float repeatInterval = 0.1f; // 연속 강화 간격 (0.1초마다 강화)

    private float timer = 0f;
    private bool isRepeating = false;

    private bool wasChanged = false; // 실제 강화 여부

    // 외부에서 등록할 강화 로직 액션
    public Action onLongClick;

    void Update()
    {
        if (!isPressed) return;

        timer += Time.deltaTime;

        if (!isRepeating)
        {
            // 처음에 꾹 눌렀을 때 대기 시간 체크
            if (timer >= initialDelay)
            {
                isRepeating = true;
                timer = 0f;
            }
        }
        else
        {
            // 대기 시간이 지난 후부터 일정 간격마다 실행
            if (timer >= repeatInterval)
            {
                onLongClick?.Invoke();
                timer = 0f;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        wasChanged = false;
        timer = 0f;
        isRepeating = false;

        // 누르는 순간 즉시 첫 번째 강화 실행
        onLongClick?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;

        if (wasChanged)
        {
            SaveManager.Instance.SaveGame();
            wasChanged = false;
        }
    }

    // 오브젝트가 비활성화될 때 초기화 (버그 방지)
    private void OnDisable()
    {
        isPressed = false;
    }
    public void SetChanged()
    {
        wasChanged = true;
    }
}
