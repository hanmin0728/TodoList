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

    [SerializeField] private GameObject stageBar;

    [Header("Boss UI")]
    [SerializeField] private GameObject bossUIContainer;
    [SerializeField] private Slider bossHPSlider;
    [SerializeField] private Image timerImage;
    [SerializeField] private TextMeshProUGUI bossTimerText;
    [SerializeField] private Button bossRetryBtn;

    private void Awake()
    {
        bossUIContainer.SetActive(false);
        bossRetryBtn.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnWaveChanged += UpdateUI;
            StageManager.Instance.OnBossEnter += StartBossInit;
            StageManager.Instance.OnBossCleared += HideBossUI;
            StageManager.Instance.OnBossHpChanged += UpdateBossHP;

            StageManager.Instance.OnBossTimerUpdated += UpdateBossTimer;
            StageManager.Instance.OnBossFailed += HandleBossFailed;
        }

        bossRetryBtn.onClick.AddListener(OnClickBossRetry);
    }

    private void OnDisable()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnWaveChanged -= UpdateUI;
            StageManager.Instance.OnBossEnter -= StartBossInit;
            StageManager.Instance.OnBossCleared -= HideBossUI;
            StageManager.Instance.OnBossHpChanged -= UpdateBossHP;

            StageManager.Instance.OnBossTimerUpdated -= UpdateBossTimer;
            StageManager.Instance.OnBossFailed -= HandleBossFailed;
        }

        bossRetryBtn.onClick.RemoveListener(OnClickBossRetry);
    }
    private void HandleBossFailed()
    {
        HideBossUI(); 
        bossRetryBtn.gameObject.SetActive(true);
    }
    private void OnClickBossRetry()
    {
        bossRetryBtn.gameObject.SetActive(false);

        StageManager.Instance.RequestBossRetry();
    }

    private void StartBossInit(float maxHp)
    {
        bossUIContainer.SetActive(true);
        stageBar.SetActive(false);
        bossHPSlider.maxValue = maxHp;
        bossHPSlider.value = maxHp;
    }
    private void UpdateBossTimer(float currentTime, float maxTime)
    {
        timerImage.fillAmount = currentTime / maxTime;
        bossTimerText.SetText(Mathf.Max(0, Mathf.CeilToInt(currentTime)).ToString()); 
    }

    private void UpdateBossHP(float currentHp)
    {
        bossHPSlider.value = currentHp;
    }

    public void HideBossUI()
    {
        bossUIContainer.SetActive(false);
        stageBar.SetActive(true);
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

  

  
}
