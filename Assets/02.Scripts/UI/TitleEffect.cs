using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
public sealed class TitleEffect : MonoBehaviour
{
    [Header("설정값")]
    public float moveStrength = 20f; 
    public float duration = 0.5f;    
    public float delayPerChar = 0.1f; // 글자마다 시작되는 시간차

    public Transform[] charTransforms; //Title 6글자

    void Start()
    {
        for (int i = 0; i < charTransforms.Length; i++)
        {
            charTransforms[i]
                .DOMoveY(charTransforms[i].position.y + moveStrength, duration)
                .SetLoops(-1, LoopType.Yoyo) 
                .SetEase(Ease.InOutSine)    
                .SetDelay(i * delayPerChar); // 0.1초씩 밀어서 시작
        }
    }
}
