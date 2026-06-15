using System;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Cysharp.Threading.Tasks;

public class AuthService 
{
    private FirebaseAuth _auth;
    public FirebaseUser CurrentUser { get; private set; }

    /// <summary>
    /// Firebase 초기화 및 익명 로그인 전체 흐름을 실행합니다.
    /// </summary>
    public async UniTask<bool> InitializeAndSignInAsync()
    {
        // Firebase 라이브러리 종속성 검사
        bool isReady = await CheckFirebaseDependenciesAsync();
        if (!isReady) return false;

        _auth = FirebaseAuth.DefaultInstance; 

        // 익명 로그인 시도
        return await SignInAnonymouslyAsync();
    }

    private async UniTask<bool> CheckFirebaseDependenciesAsync()
    {
        try
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();

            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("[Firebase] 초기화 성공");
                return true;
            }

            Debug.LogError($"[Firebase] 시스템 요구사항을 충족하지 못함: {dependencyStatus}");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Firebase] 초기화 중 예외 발생: {ex.Message}");
            return false;
        }
    }

    private async UniTask<bool> SignInAnonymouslyAsync()
    {
        try
        {
            // 이미 로그인된 상태인지 확인 (자동 로그인)
            if (_auth.CurrentUser != null)
            {
                CurrentUser = _auth.CurrentUser;
                Debug.Log($"[Firebase] 기존 익명 계정으로 자동 로그인됨. UID: {CurrentUser.UserId}");
                return true;
            }

            // 신규 익명 로그인
            AuthResult result = await _auth.SignInAnonymouslyAsync().AsUniTask();
            CurrentUser = result.User;

            Debug.Log($"[Firebase] 신규 익명 로그인 성공! UID: {CurrentUser.UserId}");
            return true;
        }
        catch (FirebaseException ex)
        {
            // Firebase 관련 구체적인 에러 코드 처리
            Debug.LogError($"[Firebase] 로그인 실패 에러코드: {ex.ErrorCode}");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Firebase] 로그인 중 알 수 없는 오류: {ex.Message}");
            return false;
        }
    }
}

