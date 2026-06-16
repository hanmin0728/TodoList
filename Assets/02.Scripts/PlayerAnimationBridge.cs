using UnityEngine;

public class AnimationBridge : MonoBehaviour
{
    private PlayerController player;

    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    public void OnAttackHit()
    {
        if (player != null) player.OnAttackHit();
    }

    public void OnAttackSequenceFinished()
    {
        if (player != null) player.OnAttackSequenceFinished();
    }
}
