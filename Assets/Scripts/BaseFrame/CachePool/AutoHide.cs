using UnityEngine;

public class AutoHide : MonoBehaviour
{
    public string res;
    public float time = 1;
    void OnEnable()
    {
        Invoke(nameof(Hide),time);
    }

    private void Hide()
    {
        CachePoolManager.Instance.HideObj(res,gameObject);
    }
}
