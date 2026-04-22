using NUnit.Framework.Internal;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject UpgradeUIManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            UpgradeUIManager.SetActive(true);
        }
    }

}
