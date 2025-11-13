
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

///按键事件管理器
public class InputManager : SingletonManager<InputManager>
{
    private bool IsOpenInput = true;
    private bool isOpenHotkey = false;
    private UnityAction<InputInfo> InputInfoCallback;
    private InputManager() { MonoManager.Instance.AddUpdateEvent(InputUpdate); }
    private Dictionary<E_EventType,InputInfo> InputDic = new();
    private InputInfo currentInputInfo;
    /// <summary>
    /// 改建方法
    /// 调用之后玩家再按下任意键时会修改事件触发按键
    /// </summary>
    /// <param name="callback">玩家按下任意键时调用回调</param>
    public void GetInputInfo(UnityAction<InputInfo> callback) { MonoManager.Instance.StartCoroutine(WaitInput(callback)); }
    private IEnumerator WaitInput(UnityAction<InputInfo> callback)
    {
        yield return null;
        InputInfoCallback = callback;
    }
    //按键事件触发器
    //当玩家按下对应键时，触发对应事件
    private void InputUpdate()
    {
        // 改建对应代码
        if (InputInfoCallback != null)
        {
            if (Input.anyKeyDown)
            {
                InputInfo info = null;
                Array keyCodes = Enum.GetValues(typeof(KeyCode));
                foreach (KeyCode item in keyCodes)
                {
                    if (Input.GetKeyDown(item)) info = new InputInfo(InputInfo.E_InputMode.Down, item);
                }
                for (int i = 0; i < 3; i++)
                {
                    if (Input.GetMouseButtonDown(i)) info = new InputInfo(InputInfo.E_InputMode.Down, i);
                }
                InputInfoCallback?.Invoke(info);
                InputInfoCallback = null;
            }
        }
        
        if (!IsOpenInput) return;
        
        foreach (E_EventType eventType in InputDic.Keys)
        {
            currentInputInfo = InputDic[eventType];
            //如果是键盘输入
            if (currentInputInfo.KeyOrMouse == InputInfo.E_InputType.Keyboard)
            {
                switch (currentInputInfo.Mode)
                {
                    case InputInfo.E_InputMode.Down:
                        if (Input.GetKeyDown(currentInputInfo.KeyCode)) EventCenterManager.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputMode.Up:
                        if (Input.GetKeyUp(currentInputInfo.KeyCode)) EventCenterManager.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputMode.LongDown:
                        if (Input.GetKey(currentInputInfo.KeyCode)) EventCenterManager.Instance.EventTrigger(eventType);
                        break;
                }
            }
            //如果是鼠标输入
            else if (currentInputInfo.KeyOrMouse == InputInfo.E_InputType.Mouse)
            {
                switch (currentInputInfo.Mode)
                {
                    case InputInfo.E_InputMode.Down:
                        if (Input.GetMouseButtonDown(currentInputInfo.MouseID)) EventCenterManager.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputMode.Up:
                        if (Input.GetMouseButtonUp(currentInputInfo.MouseID)) EventCenterManager.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputMode.LongDown:
                        if (Input.GetMouseButton(currentInputInfo.MouseID)) EventCenterManager.Instance.EventTrigger(eventType);
                        break;
                }
            }
        }

        if (isOpenHotkey)
        {
            EventCenterManager.Instance.EventTrigger<float>(E_EventType.Horizontal,Input.GetAxis("Horizontal"));
            EventCenterManager.Instance.EventTrigger<float>(E_EventType.Vertical,Input.GetAxis("Vertical"));
        }
    }
    /// <summary>
    /// 玩家是否可以控制
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenOrCloseInput(bool isOpen) { IsOpenInput = isOpen; }
    /// <summary>
    /// 启动或关闭热键
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenOrCloseHotkey(bool isOpen) { isOpenHotkey = isOpen; }
    /// <summary>
    /// 键盘改建
    /// </summary>
    /// <param name="eventType">事件枚举类型</param>
    /// <param name="keyCode">键盘按键</param>
    /// <param name="inputMode">按键模式 Down 或 Up</param>
    public void AddKeyboardInput(E_EventType eventType,KeyCode keyCode,InputInfo.E_InputMode inputMode)
    {
        //如果有按键事件 改建
        if (InputDic.ContainsKey(eventType))
        {
            InputDic[eventType].KeyOrMouse = InputInfo.E_InputType.Keyboard;
            InputDic[eventType].Mode = inputMode;
            InputDic[eventType].KeyCode = keyCode;
        }
        //添加一个按键事件
        else InputDic.Add(eventType,new InputInfo(inputMode,keyCode));
    }
    /// <summary>
    /// 鼠标改建
    /// </summary>
    /// <param name="eventType">事件枚举类型</param>
    /// <param name="mouseID">鼠标按键</param>
    /// <param name="inputMode">按键模式 Down 或 Up</param>
    public void AddMouseInput(E_EventType eventType,int mouseID,InputInfo.E_InputMode inputMode)
    {
        //如果有按键事件 改建
        if (InputDic.ContainsKey(eventType))
        {
            InputDic[eventType].KeyOrMouse = InputInfo.E_InputType.Mouse;
            InputDic[eventType].Mode = inputMode;
            InputDic[eventType].MouseID = mouseID;
        }
        //添加一个按键事件
        else InputDic.Add(eventType,new InputInfo(inputMode,mouseID));
    }
    /// <summary>
    /// 移除按键事件
    /// </summary>
    /// <param name="eventType">事件枚举类型</param>
    public void RemoveInput(E_EventType eventType)
    {
        if (InputDic.ContainsKey(eventType)) InputDic.Remove(eventType);
    }
}