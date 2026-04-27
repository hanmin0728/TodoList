using System.Collections.Generic;
using UnityEngine;

public class FloatingTextPopUpManager : Singleton<FloatingTextPopUpManager>
{
    [SerializeField] private GameObject floatingTextPrefab;

    public void Show(string content, Transform targetParent, TextType type)
    {
        GameObject obj = PoolManager.Instance.Spawn(floatingTextPrefab, targetParent.position, Quaternion.identity);
        obj.transform.SetParent(targetParent);

        if (obj.TryGetComponent(out FloatingTextPopUp ft))
        {
            ft.Setup(content, type);
        }
    }
}
