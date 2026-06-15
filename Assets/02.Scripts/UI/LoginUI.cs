using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class LoginUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button anonymousLoginButton;
    [SerializeField] private GameObject loadingIndicator; // 로딩 스피너 UI 등

    private AuthService _authService;
    private bool _isProcessing; // 중복 클릭 방지용 플래그

    private void Awake()
    {
        _authService = new AuthService();

        // 유니티 버튼 이벤트에 비동기 메서드 연결 (Forget을 통해 비동기 흐름을 안전하게 실행)
        anonymousLoginButton.onClick.AddListener(() => OnClickAnonymousLogin().Forget());
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지를 위한 리스너 해제
        anonymousLoginButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 익명 로그인 버튼 클릭 시 실행되는 비동기 메서드
    /// </summary>
    private async UniTaskVoid OnClickAnonymousLogin()
    {
        // 1. 중복 클릭 방지 검사
        if (_isProcessing) return;
        _isProcessing = true;

        // 2. UI 상태 변경 (버튼 비활성화 및 로딩창 활성화)
        SetUIState(isReadOnly: true);

        Debug.Log("[UI] 익명 로그인 요청 시작...");

        // 3. 기존에 만든 Firebase 초기화 및 로그인 함수 호출 및 대기
        bool isSuccess = await _authService.InitializeAndSignInAsync();

        // 4. 결과 처리
        if (isSuccess)
        {
            string uid = _authService.CurrentUser.UserId;
            Debug.Log($"[UI] 로그인 성공 완료! UID: {uid}");

            // TODO: 성공 후 메인 로비 씬으로 전환하거나, Firestore 데이터 로드 로직 실행
            // SceneManager.LoadSceneAsync("LobbyScene").Forget();
        }
        else
        {
            Debug.LogError("[UI] 로그인 실패. 다시 시도해 주세요.");

            // 실패했을 경우 유저가 다시 누를 수 있도록 UI 복구
            SetUIState(isReadOnly: false);
            _isProcessing = false;
        }
    }

    /// <summary>
    /// 로그인 진행 상태에 따라 UI 상호작용을 제어합니다.
    /// </summary>
    private void SetUIState(bool isReadOnly)
    {
        // 진행 중일 때는 버튼을 누르지 못하게 막음
        anonymousLoginButton.interactable = !isReadOnly;

        // 로딩 인디케이터 켜고 끄기
        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(isReadOnly);
        }
    }
}
