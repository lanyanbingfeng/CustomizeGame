using UnityEngine.Events;

/// <summary>
/// 集中管理 帧更新
/// </summary>
public class MonoManager : SingletonAutoMono<MonoManager>
{
    private UnityAction _updateEvent;
    private UnityAction _fixedUpdateEvent;
    private UnityAction _lateUpdateEvent;

    /// <summary>
    /// 添加帧更新事件
    /// </summary>
    /// <param name="updateEvent"></param>
    public void AddUpdateEvent(UnityAction updateEvent)
    {
        _updateEvent += updateEvent;
    }
    
    /// <summary>
    /// 删除帧更新事件
    /// </summary>
    /// <param name="updateEvent"></param>
    public void RemoveUpdateEvent(UnityAction updateEvent)
    {
        _updateEvent -= updateEvent;
    }
    /// <summary>
    /// 添加物理帧更新事件
    /// </summary>
    /// <param name="fixedUpdateEvent"></param>
    public void AddFixedUpdateEvent(UnityAction fixedUpdateEvent)
    {
        _fixedUpdateEvent += fixedUpdateEvent;
    }
    /// <summary>
    /// 删除物理帧更新事件
    /// </summary>
    /// <param name="fixedUpdateEvent"></param>
    public void RemoveFixedUpdateEvent(UnityAction fixedUpdateEvent)
    {
        _fixedUpdateEvent -= fixedUpdateEvent;
    }
    /// <summary>
    /// 添加帧更新之后的帧更新事件
    /// </summary>
    /// <param name="lateUpdateEvent"></param>
    public void AddLateUpdateEvent(UnityAction lateUpdateEvent)
    {
        _lateUpdateEvent += lateUpdateEvent;
    }
    /// <summary>
    /// 删除帧更新之后的帧更新事件
    /// </summary>
    /// <param name="lateUpdateEvent"></param>
    public void RemoveLateUpdateEvent(UnityAction lateUpdateEvent)
    {
        _lateUpdateEvent -= lateUpdateEvent;
    }
    private void Update()
    {
        _updateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        _fixedUpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        _lateUpdateEvent?.Invoke();
    }
}
