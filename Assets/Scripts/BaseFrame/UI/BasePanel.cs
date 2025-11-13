using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UI面板基类
public abstract class BasePanel : MonoBehaviour
{
    //存储当前面板的控件
    private Dictionary<string, UIBehaviour> uiDic = new();

    //默认不处理的控件名称
    private static List<string> defaultNameList = new()
    {
        "Image", "Text (TMP)", "RawImage", "Background", "Checkmark", "Label",
        "Text (Legacy)", "Arrow", "Placeholder", "Fill", "Handle", "Viewport", "Scrollbar Horizontal",
        "Scrollbar Vertical",
    };

    protected virtual void Awake()
    {
        FindchildrenControl<Button>();
        FindchildrenControl<Toggle>();
        FindchildrenControl<Slider>();
        FindchildrenControl<InputField>();
        FindchildrenControl<ScrollRect>();
        FindchildrenControl<Dropdown>();
        FindchildrenControl<Text>();
        FindchildrenControl<TextMeshProUGUI>();
        FindchildrenControl<Image>();
    }

    public abstract void Show();
    
    public abstract void Hide();

    /// <summary>
    /// 得到指定控件
    /// </summary>
    /// <param name="uiName">UI名称</param>
    /// <typeparam name="T">UI类型</typeparam>
    /// <returns></returns>
    protected T GetUI<T>(string uiName) where T : UIBehaviour
    {
        if (uiDic.TryGetValue(uiName, out var value))
        {
            T ui = value as T;
            if (ui == null) Debug.LogWarning($"不存在{typeof(T).Name}类型的{uiName}组件");
            return ui;
        }
        Debug.LogWarning($"不存在名字为{uiName}组件");
        return null;
    }
    
    //按钮事件
    protected virtual void ChildButtonEvent(string buttonName){}
    //多选框事件
    protected virtual void ChildToggleEvent(string buttonName, bool state){}
    //滑动条事件
    protected virtual void ChildSilderEvent(string buttonName,float value){}

    /// <summary>
    /// 查找所有子对象的控件
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    private void FindchildrenControl<T>() where T : UIBehaviour
    {
        T[] children = gameObject.GetComponentsInChildren<T>();
        for (int i = 0; i < children.Length; i++)
        {
            string childName = children[i].gameObject.name;
            if (!uiDic.ContainsKey(childName))
            {
                if (!defaultNameList.Contains(childName))
                {
                    uiDic.Add(childName, children[i]);

                    if (children[i] is Button)
                    {
                        (children[i] as Button)?.onClick.AddListener(() =>
                        {
                            ChildButtonEvent(childName);
                        });
                    }

                    if (children[i] is Toggle)
                    {
                        (children[i] as Toggle)?.onValueChanged.AddListener(isopen =>
                        {
                            ChildToggleEvent(childName, isopen);
                        });
                    }
                    if (children[i] is Slider)
                    {
                        (children[i] as Slider)?.onValueChanged.AddListener(value =>
                        {
                            ChildSilderEvent(childName, value);
                        });
                    }
                }
            }
        }
    }
}
