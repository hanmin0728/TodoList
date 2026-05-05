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

        CacheMainCamera();
    }

    public void RegisterPlayer(PlayerController player)
    {
        Player = player;
    }

    public Vector2 GetSpawnPosition()
    {
        if (mainCamera == null)
        {
            CacheMainCamera();
        }

        if (mainCamera == null)
        {
            return new Vector2(spawnOffsetX, spawnBaseY);
        }

        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float spawnX = mainCameraTransform.position.x + cameraHalfWidth + spawnOffsetX;
        float spawnY = spawnBaseY + Random.Range(spawnRandomYRange.x, spawnRandomYRange.y);

        return new Vector2(spawnX, spawnY);
    }

    public void MoveBackground(float speed)
    {
        if (background == null)
        {
            return;
        }

        background.Scroll(speed);
    }

    private void CacheMainCamera()
    {
        mainCamera = Camera.main;
        mainCameraTransform = mainCamera != null ? mainCamera.transform : null;
    }
}
