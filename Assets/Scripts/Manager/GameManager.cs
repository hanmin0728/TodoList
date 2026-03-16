using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private ScrollingBackground background;

    public void MoveBackground(float speed)
    {
        if (background != null)
            background.Scroll(speed);
    }
}
