using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class AuthService
{
    private FirebaseAuth auth;
    private DatabaseReference dbRef;

    public FirebaseUser CurrentUser => auth?.CurrentUser;

    /// <summary>
    /// Firebase 초기화, 로컬에 저장된 토큰이 실제 서버에서도 유효한지 검증
    /// </summary>
    public async UniTask<bool> TryInitializeSessionAsync()
    {
        // 기기 호환성 체크
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
        if (dependencyStatus != DependencyStatus.Available)
        {
            Debug.LogError($"[AuthService] Firebase 호환성 오류: {dependencyStatus}");
            return false;
        }
        
        // 인스턴스 초기화
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        // 기존 세션이 있다면 서버와 동기화하여 유효성 확인
        if (CurrentUser != null)
        {
            try
            {
                // 서버에 현재 토큰이 살아있는지 확인 (통신 필수)
                await auth.CurrentUser.ReloadAsync().AsUniTask();
                return true;
            }
            catch
            {
                auth.SignOut(); // 토큰 만료 시 로컬 세션 폐기
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// 닉네임 입력 후 '확인' 버튼을 눌렀을 때 호출하여 실제 익명 계정을 서버에 생성
    /// </summary>
    public async UniTask<bool> TryCreateAccountAsync(CancellationToken ct)
    {
        try
        {
            var result = await auth.SignInAnonymouslyAsync().AsUniTask();
            return result.User != null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthService] 계정 생성 실패: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 계정 생성 후 닉네임 DB 저장에 실패했을 경우, 
    /// 데이터베이스에 불필요한 UID만 남는 현상을 방지하기 위해 계정을 즉시 삭제합니다.
    /// </summary>
    public async UniTask DeleteAccountAsync()
    {
        if (CurrentUser != null)
        {
            Debug.Log("[AuthService] 계정 롤백(삭제) 진행...");
            await CurrentUser.DeleteAsync().AsUniTask();
        }
    }

    /// <summary>
    /// 서버 DB에 해당 UID로 저장된 닉네임이 있는지 확인 데이터가 없다면 가입을 완료하지 않은 '신규 유저'로 판단
    /// </summary>
    public async UniTask<bool> CheckIsNewUserAsync(string uid, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(uid)) return true;

        try
        {
            var snapshot = await dbRef.Child("users").Child(uid).Child("nickname").GetValueAsync().AsUniTask();
            return !snapshot.Exists || string.IsNullOrEmpty(snapshot.Value?.ToString());
        }
        catch { return true; }
    }

    /// <summary>
    /// 닉네임과 계정 생성 시간을 DB에 저장 닉네임이 성공적으로 저장되면 해당 유저는 정식 유저
    /// </summary>
    public async UniTask<bool> SaveNewUserNicknameAsync(string nickname, CancellationToken ct)
    {
        if (CurrentUser == null) return false;

        try
        {
            string uid = CurrentUser.UserId;
            long createdAtUnix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            await dbRef.Child("users").Child(uid).Child("nickname").SetValueAsync(nickname).AsUniTask();
            await dbRef.Child("users").Child(uid).Child("createdAt").SetValueAsync(createdAtUnix).AsUniTask();
            return true;
        }
        catch { return false; }
    }
}