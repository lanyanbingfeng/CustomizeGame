

using UnityEngine.Events;

public class TimeObj:IPoolObject
{
    public int ID;
    ///结束调用
    public UnityAction OverCallback;
    ///间隔一段事件调用
    public UnityAction IntervalCallback;

    ///总时间
    public int RunTime;
    private int Time;

    ///间隔时间
    public int RunIntervalTime;
    ///用于重置间隔时间的变量
    public int IntervalTime;

    public bool isRuning;

    /// <summary>
    /// 初始化计时器
    /// </summary>
    /// <param name="id"></param>
    /// <param name="time">总时长</param>
    /// <param name="OverCallback">结束回调</param>
    /// <param name="intervalTime">间隔时间</param>
    /// <param name="OutCallback">间隔回调</param>
    public void Init(int id,int time,UnityAction overCallback,int intervalTime = 0,UnityAction intervalCallback = null)
    {
        ID = id;
        RunTime = Time = time;
        RunIntervalTime = IntervalTime = intervalTime;
        OverCallback = overCallback;
        IntervalCallback = intervalCallback;
        isRuning = true;
    }

    public void ResetTime()
    {
        RunTime = Time;
        RunIntervalTime = IntervalTime;
        isRuning = true;
    }

    public void ResetData()
    {
        OverCallback = null;
        IntervalCallback = null;
    }
}
