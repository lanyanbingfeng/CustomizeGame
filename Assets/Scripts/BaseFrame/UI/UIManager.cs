using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
// Canves层级划分
public enum E_UILayer { Top, Middle, Bottom, System }
/// <summary>
/// UI面板管理器 配合 Addressable 使用
/// </summary>
public class UIManager : SingletonManager<UIManager>
{
    private Camera UICamera;
    private Canvas UICanvas;
    private EventSystem UIEventSystem;
    private Transform BottomLayer;
    private Transform MiddleLayer;
    private Transform TopLayer;
    private Transform SystemLayer;
    private Dictionary<string, BasePanel> panelDic = new();
    private UIManager()
    {
        UICamera = Object.Instantiate(ResourcesManager.Instance.Load<GameObject>("UI/UICamera")).GetComponent<Camera>();
        Object.DontDestroyOnLoad(UICamera.gameObject);
        UICanvas = Object.Instantiate(ResourcesManager.Instance.Load<GameObject>("UI/Canvas")).GetComponent<Canvas>();
        UICanvas.worldCamera = UICamera;
        Object.DontDestroyOnLoad(UICanvas.gameObject);
        UIEventSystem = Object.Instantiate(ResourcesManager.Instance.Load<GameObject>("UI/EventSystem")).GetComponent<EventSystem>();
        Object.DontDestroyOnLoad(UIEventSystem.gameObject);
        
        BottomLayer = UICanvas.transform.Find("Bottom");
        MiddleLayer = UICanvas.transform.Find("Middle");
        TopLayer = UICanvas.transform.Find("Top");
        SystemLayer = UICanvas.transform.Find("System");
    }
    private Transform GetLayerFather(E_UILayer layer) => layer switch
    {
        E_UILayer.Top => TopLayer,
        E_UILayer.Middle => MiddleLayer,
        E_UILayer.Bottom => BottomLayer,
        E_UILayer.System => SystemLayer,
        _ => null
    };
    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="layer">层级</param>
    /// <param name="callback">回调</param>
    /// <typeparam name="T">面板脚本</typeparam>
    public void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle,UnityAction<T> callback = null) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].gameObject.SetActive(true);
            panelDic[panelName].Show();
            callback?.Invoke(panelDic[panelName] as T);
            return;
        }
        AddressableManager.Instance.LoadAssetAsync<GameObject>(typeof(T).Name, handle =>
        {
            if (panelDic.ContainsKey(panelName)) return;
            GameObject obj = Object.Instantiate(handle.Result, GetLayerFather(layer), false);
            T objPanel = obj.GetComponent<T>();
            objPanel.Show();
            panelDic.Add(panelName, objPanel);
            callback?.Invoke(objPanel);
        });
    }
    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="isDel">删除或隐藏</param>
    /// <typeparam name="T">面板脚本</typeparam>
    public void HidePanel<T>(bool isDel = true) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].Hide();
            if (isDel)
            {
                Object.Destroy(panelDic[panelName].gameObject);
                panelDic.Remove(panelName);
            }
            else panelDic[panelName].gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 得到面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetPanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.TryGetValue(panelName, out var value)) return value as T;
        return null;
    }
    /// <summary>
    /// 添加 UI事件接口 方法
    /// </summary>
    /// <param name="control">UI控件</param>
    /// <param name="eventTriggerType">事件类型</param>
    /// <param name="callback">回调</param>
    public static void AddCustomEvent(UIBehaviour control,EventTriggerType eventTriggerType,UnityAction<BaseEventData> callback)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null) trigger = control.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventTriggerType;
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }
}
