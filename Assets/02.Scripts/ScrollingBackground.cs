using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] private MeshRenderer backgroundRenderer;
    private float currentOffset = 0f;

    public void Scroll(float speed) 
    {
        currentOffset = Mathf.Repeat(currentOffset + speed * Time.deltaTime, 1f);
        backgroundRenderer.material.mainTextureOffset = new Vector2(currentOffset, 0);
    }

}
