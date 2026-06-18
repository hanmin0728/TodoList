using Cysharp.Threading.Tasks.Triggers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIManager : MonoBehaviour
{
    [Header("UI Panels (화면 패널 관리)")]
    [SerializeField] private GameObject nicknamePopup;
    [SerializeField] private GameObject loadingPanel;     

    [Header("Main Login UI")]
    [SerializeField] private Button guestLoginButton;      

    [Header("Nickname Popup UI")]
    [SerializeField] private TMP_InputField nicknameInput; 
    [SerializeField] private Button confirmButton;        

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
        nicknameInput.text = ""; 
    }
   
    public void SetLoading(bool isLoading)
    {
        nicknamePopup.SetActive(false);
        guestLoginButton.gameObject.SetActive(false);
        loadingPanel.SetActive(isLoading);
    }

 
}
