using UnityEngine;

public class FloatingTextPopUpManager : Singleton<FloatingTextPopUpManager>
{
    [SerializeField] private GameObject floatingTextPrefab;

    public void Show(float value, Transform targetParent, TextType type)
    {
        GameObject obj = SpawnPopup(targetParent);
        if (obj != null && obj.TryGetComponent(out FloatingTextPopUp ft))
        {
            ft.Setup(value, type);
        }
    }

    private GameObject SpawnPopup(Transform targetParent)
    {
        if (floatingTextPrefab == null || targetParent == null)
        {
            return null;
        }

        GameObject obj = PoolManager.Instance.Spawn(floatingTextPrefab, targetParent.position, Quaternion.identity);
        if (obj == null)
        {
            return null;
        }

        obj.transform.SetParent(targetParent);
        return obj;
    }
}
