
using UnityEngine;

public class InputInfo
{
    public enum E_InputType
    {
        Keyboard,
        Mouse,
    }
    public enum E_InputMode
    {
        Down,Up,LongDown
    }
    
    public E_InputType KeyOrMouse;
    public E_InputMode Mode;
    
    public KeyCode KeyCode;
    public int MouseID;

    /// <summary>
    /// 键盘输入初始化
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="keyCode"></param>
    public InputInfo(E_InputMode mode, KeyCode keyCode)
    {
        KeyOrMouse = E_InputType.Keyboard;
        Mode = mode;
        KeyCode = keyCode;
    }
    /// <summary>
    /// 鼠标输入初始化
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="mouseID"></param>
    public InputInfo( E_InputMode mode,int mouseID)
    {
        KeyOrMouse = E_InputType.Mouse;
        Mode = mode;
        MouseID = mouseID;
    }
}
