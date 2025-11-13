/// <summary>
/// 事件中心触发枚举分类
/// </summary>
public enum E_EventType
{
    LoadSceneChange, //场景加载进度事件
    
    Horizontal, //水平热键
    Vertical, //垂直热键
    
    PlayerDownCtrl, //玩家按下奔跑键
    PlayerJump, //玩家跳跃
    PlayerMouse0Atk, //玩家左键攻击
    PlayerMouse1Atk, //玩家右键攻击
    PlayerFAtk, //玩家F键攻击
}