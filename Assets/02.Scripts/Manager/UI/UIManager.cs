using NUnit.Framework.Internal;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject diePopUP;

    private void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            GameManager.Instance.Player.OnPlayerDied += PlayerDie;
        }

    }
    private void OnDisable()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            GameManager.Instance.Player.OnPlayerDied -= PlayerDie;
        }
    }
  
    private void Update()
    {
    }
    public void PlayerDie()
    {
        diePopUP.SetActive(true);
    }
    public void OnClickRestartButton()
    {
        diePopUP.SetActive(false);
        GameManager.Instance.RestartWave();
    }
}
