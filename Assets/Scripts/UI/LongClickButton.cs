using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Repeat Settings")]
    [SerializeField] private float initialDelay = 0.5f;
    [SerializeField] private float repeatInterval = 0.1f;

    private Action clickAction;
    private float timer;
    private bool isPressed;
    private bool isRepeating;
    private bool wasChanged;

    private void Update()
    {
        if (!isPressed)
        {
            return;
        }

        timer += Time.unscaledDeltaTime;

        if (!isRepeating)
        {
            if (timer < initialDelay)
            {
                return;
            }

            isRepeating = true;
            timer = 0f;
            return;
        }

        if (timer < repeatInterval)
        {
            return;
        }

        clickAction?.Invoke();
        timer = 0f;
    }

    public void SetClickAction(Action action)
    {
        clickAction = action;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        wasChanged = false;
        timer = 0f;
        isRepeating = false;
        clickAction?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopPressAndSaveIfChanged();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopPressAndSaveIfChanged();
    }

    public void SetChanged()
    {
        wasChanged = true;
    }

    private void OnDisable()
    {
        StopPressAndSaveIfChanged();
    }

    private void StopPressAndSaveIfChanged()
    {
        if (!isPressed && !wasChanged)
        {
            return;
        }

        isPressed = false;
        timer = 0f;
        isRepeating = false;

        if (!wasChanged)
        {
            return;
        }

        SaveManager.Instance.SaveGame();
        wasChanged = false;
    }
}
