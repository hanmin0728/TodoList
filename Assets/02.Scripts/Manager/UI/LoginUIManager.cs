using Cysharp.Threading.Tasks.Triggers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIManager : MonoBehaviour
{
    [Header("UI Panels (화면 패널 관리)")]
    [SerializeField] private GameObject nicknamePopup;
    [SerializeField] private GameObject loadingPanel;      // 빙글빙글 도는 로딩창

    [Header("Main Login UI")]
    [SerializeField] private Button guestLoginButton;      // 게스트 로그인

    [Header("Nickname Popup UI")]
    [SerializeField] private TMP_InputField nicknameInput; 
    [SerializeField] private Button confirmButton;         // 닉네임 확인 버튼

    [SerializeField] private Transform nicknameWarningPos;         

    public event Action<string> OnNicknameConfirmed;

    private void Awake()
    {
        guestLoginButton.onClick.AddListener(() => ShowNicknamePopup());
        confirmButton.onClick.AddListener(() => OnNicknameConfirmed?.Invoke(nicknameInput.text.Trim()));

        InitializeUI();
    }
     public Transform GetNicknameTransform()
    {
        return nicknameWarningPos;
    }
    private void InitializeUI()
    {
        nicknamePopup.SetActive(false);
        loadingPanel.SetActive(false);
    }

    public void NewUserSettingUI()
    {
        guestLoginButton.gameObject.SetActive(true);
        loadingPanel.SetActive(false);
        nicknamePopup.SetActive(false);
    }

    public void ShowNicknamePopup()
    {
        nicknamePopup.SetActive(true);
        guestLoginButton.gameObject.SetActive(false);
        nicknameInput.text = ""; // 팝업이 열릴 때 이전 입력값 초기화
    }
   
    public void SetLoading(bool isLoading)
    {
        nicknamePopup.SetActive(false);
        guestLoginButton.gameObject.SetActive(false);
        loadingPanel.SetActive(isLoading);
    }

 
}
