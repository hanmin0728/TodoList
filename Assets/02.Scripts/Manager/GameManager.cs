using UnityEngine;

public sealed class GameManager : Singleton<GameManager>
{
    [SerializeField] private ScrollingBackground background;

    [SerializeField] private float spawnBaseY = 0f;
    [SerializeField] private float spawnOffsetX = 2.0f;
    [SerializeField] private Vector2 spawnRandomYRange = new Vector2(-0.2f, 0.2f);

    private Camera mainCamera;
    private Transform mainCameraTransform;

    public PlayerController Player { get; private set; }
    

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this)
        {
            return;
        }

        TryCacheMainCamera();
    }

    private void OnDestroy()
    {
        if (Player != null)
        {
            Player.OnPlayerDied -= HandlePlayerDeath;
        }
    }

    public void RegisterPlayer(PlayerController player)
    {
        Player = player;
        Player.OnPlayerDied += HandlePlayerDeath;
    }
    public void RestartWave()
    {
        Player.Revive();
        StageManager.Instance.ResetToFirstWave();
    }

    public void HandlePlayerDeath()
    {
        StageManager.Instance.StopWave();
    }

    public Vector2 EnemySpawnPosition()
    {
        if (mainCamera == null && !TryCacheMainCamera())
        {
            Debug.LogWarning("[GameManager] 메인 카메라를 찾을 수 없어 기본 스폰 위치를 반환합니다.");
            return new Vector2(spawnOffsetX, spawnBaseY);
        }

       
        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;  // 카메라 화면의 가로 절반 길이 
        float spawnX = mainCameraTransform.position.x + cameraHalfWidth + spawnOffsetX;
        float spawnY = spawnBaseY + Random.Range(spawnRandomYRange.x, spawnRandomYRange.y);

        return new Vector2(spawnX, spawnY);
    }
    private bool TryCacheMainCamera()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCameraTransform = mainCamera.transform;
            return true;
        }
        return false;
    }

    public void MoveBackground(float speed)
    {
        background?.Scroll(speed);
    }


    private void OnDrawGizmos()
    {
        Camera cam = Application.isPlaying ? mainCamera : Camera.main;
        Transform camTransform = cam != null ? cam.transform : null;

        if (cam == null || camTransform == null) return;

        float cameraHalfWidth = cam.orthographicSize * cam.aspect;
        float targetX = camTransform.position.x + cameraHalfWidth + spawnOffsetX;

        Vector2 boxCenter = new Vector2(targetX, spawnBaseY);
        Vector2 boxSize = new Vector2(0.5f, spawnRandomYRange.y - spawnRandomYRange.x);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

}
