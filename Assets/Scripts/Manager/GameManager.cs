using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private ScrollingBackground background;

    [SerializeField] private float spawnBaseY = 0f;
    public PlayerController Player { get; private set; }

    public void RegisterPlayer(PlayerController player)
    {
        Player = player;
    }
    public Vector2 GetSpawnPosition()
    {
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        float spawnX = Camera.main.transform.position.x + camHalfWidth + 2.0f;

        float randomY = Random.Range(-0.2f, 0.2f);

        return new Vector2(spawnX, spawnBaseY + randomY);
    }

    public void MoveBackground(float speed)
    {
        if (background != null)
            background.Scroll(speed);
    }
  

}
