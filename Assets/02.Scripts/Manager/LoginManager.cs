using Cysharp.Threading.Tasks;
using Firebase.Auth;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement; 
public class LoginManager : MonoBehaviour
{
    [SerializeField] private LoginUIManager loginUIManager;
    private AuthService authService;


    private bool isProcessing;     // 중복 로그인 시도 방지를 위한 플래그

    private CancellationTokenSource cts;   // 비동기 작업 중 씬 전환/앱 종료 시 메모리 누수 및 크래시 방지용 토큰 소스

    private void Awake()
    {
        authService = new AuthService(); // 통신 서비스 객체 생성
        cts = new CancellationTokenSource();
    }

    private void OnEnable()
    {
        loginUIManager.OnNicknameConfirmed += HandleNicknameConfirmed;
    }
    private void OnDestroy()
    {
        loginUIManager.OnNicknameConfirmed -= HandleNicknameConfirmed;

        // 매니저가 파괴될 때 실행 중인 모든 서버 통신 작업 즉시 중단
        cts?.Cancel();
        cts?.Dispose();
    }

    private void Start()
    {
        // 앱 실행 즉시 자동 로그인 로직 개시
        TryAutoLoginAsync(cts.Token).Forget();
    }

    /// <summary>
    /// 자동 로그인 앱 실행 시 기기에 저장된 유효한 세션이 있는지 확인하고 메인 씬으로 전환합니다.
    /// </summary>
    private async UniTaskVoid TryAutoLoginAsync(CancellationToken cancellationToken)
    {
        // 기존 인증 세션이 서버상에서도 유효한지 검증 (로그인 시도 X)
        bool isSessionValid = await authService.TryInitializeSessionAsync();

        if (isSessionValid)
        {
            // 닉네임 유무 검증
            bool isNewUser = await authService.CheckIsNewUserAsync(authService.CurrentUser.UserId, cancellationToken);
            
            if (!isNewUser)
            {
                loginUIManager.SetLoading(true);

                // 정식 유저라면 서버 데이터를 로컬로 불러온 뒤 게임 진입
                await SaveManager.Instance.LoadFromServerAsync();
                NavigateToMainGame();
                return;
            }
        }

        // 세션 없거나 신규 유저라면 UI 활성화
        loginUIManager.NewUserSettingUI();
    }

    private void HandleNicknameConfirmed(string nickname)
    {
        if (string.IsNullOrEmpty(nickname) || nickname.Length < 2 || nickname.Length > 8)
        {
            Debug.LogWarning("2글자 이상 8글자 이하로 입력하세요");
            ShowWarningText("2글자 이상 8글자 이하로 입력하세요");
            return;
        }

        // 조건에 맞다면 진짜 계정 생성/로그인 로직(비동기)을 가동시킴
        ProcessRegistrationAndLoginAsync(nickname, cts.Token).Forget();
    }
    private void ShowWarningText(string message)
    {
        TextType popupType = TextType.NicknameWarning;
        FloatingTextPopUpManager.Instance.Show(message, loginUIManager.GetNicknameTransform(), popupType);
    }

    /// <summary>
    /// 신규 계정 가입 파이프라인: [계정 생성 -> 닉네임 저장 -> 데이터 백업 -> 씬 전환]
    /// </summary>
    private async UniTaskVoid ProcessRegistrationAndLoginAsync(string nickname, CancellationToken cancellationToken)
    {
        if (isProcessing) return;
        isProcessing = true;

        loginUIManager.SetLoading(true);

        // Firebase 익명 계정 실제 생성
        bool isAccountCreated = await authService.TryCreateAccountAsync(cancellationToken);
        if (!isAccountCreated) { AbortTransaction(); return; }

        // DB에 유저 프로필 저장
        bool isProfileSaved = await authService.SaveNewUserNicknameAsync(nickname, cancellationToken);
        if (isProfileSaved)
        {
            // 성공 시 SaveManager를 통해 초기 데이터 서버 동기화
            await SaveManager.Instance.SaveToServerAsync();
            NavigateToMainGame();
        }
        else
        {
            // [롤백 처리] 닉네임 저장 실패 시, 서버에 빈 UID 계정만 남지 않도록 즉시 계정 삭제
            Debug.LogError("[LoginManager] 닉네임 저장 실패! 계정을 롤백합니다.");
            await authService.DeleteAccountAsync();
            AbortTransaction();
        }
    }

    /// <summary>
    /// 통신 중 에러 발생 시 UI 초기 상태로 
    /// </summary>
    private void AbortTransaction()
    {
        isProcessing = false;
        loginUIManager.NewUserSettingUI();
    }

    /// <summary>
    /// 메인 게임 씬 로드
    /// </summary>
    private void NavigateToMainGame()
    {
        SceneManager.LoadSceneAsync("Main").ToUniTask().Forget();
    }

}
