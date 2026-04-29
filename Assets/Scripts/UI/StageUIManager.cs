using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private Image stageProgressImage;

    [SerializeField] private float fillSpeed = 2.0f;

    private Coroutine fillCoroutine;
    private bool isFirstLoad = true;

    private void Awake()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnWaveChanged += UpdateUI;
        }
    }


    private void UpdateUI(int stageID, int waveIndex)
    {
        int displayStage = stageID - 100; 
        stageText.text = $"Stage {displayStage}-{waveIndex}";

        float targetValue = (waveIndex - 1) / 3.0f;

        if (isFirstLoad)
        {
            if (fillCoroutine != null) StopCoroutine(fillCoroutine);

            stageProgressImage.fillAmount = targetValue; 
            isFirstLoad = false; 
        }
        else 
        {
            if (fillCoroutine != null) StopCoroutine(fillCoroutine);
            fillCoroutine = StartCoroutine(AnimateSlider(targetValue));
        }
    }
    private IEnumerator AnimateSlider(float targetValue)
    {
        // 스테이지가 1로 초기화되었다면 fillAmout 초기화
        if (targetValue == 0 && stageProgressImage.fillAmount > 0.5f)
        {
            stageProgressImage.fillAmount = 0;
        }

        while (!Mathf.Approximately(stageProgressImage.fillAmount, targetValue))
        {
            stageProgressImage.fillAmount = Mathf.MoveTowards(stageProgressImage.fillAmount, targetValue,Time.deltaTime * fillSpeed);
            yield return null;
        }

    }
    private void OnDestroy()
    {
        if (StageManager.Instance != null)
            StageManager.Instance.OnWaveChanged -= UpdateUI;
    }
}
