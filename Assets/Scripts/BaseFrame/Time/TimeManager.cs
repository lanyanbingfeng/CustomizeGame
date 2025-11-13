
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : SingletonManager<TimeManager>
{
    private TimeManager() { StartTimeManager(); }
    //计时器字典
    private Dictionary<int, TimeObj> InfluencetimeDic = new();
    //不受时间影响的计时器字典
    private Dictionary<int, TimeObj> notInfluencetimeDic = new();
    //待移除列表
    private readonly List<TimeObj> _timeDelList = new();
    private Coroutine InfluencetimeDicCoroutine;
    private Coroutine notInfluencetimeDicCoroutine;
    //计算间隔时间
    private const float IntervalTime = 0.1f;
    //创建的计时器唯一ID
    private int _timeID;
    private readonly WaitForSeconds _waitTime = new(IntervalTime);
    private readonly WaitForSecondsRealtime _waitTimeRealtime = new(IntervalTime);
    private IEnumerator StartTime(bool isInfluenceTime,Dictionary<int, TimeObj> timeDic)
    {
        while (true)
        {
            if (isInfluenceTime) yield return _waitTime;
            else yield return _waitTimeRealtime;
            foreach (TimeObj obj in timeDic.Values)
            {
                if(!obj.isRuning) continue;

                if (obj.IntervalCallback != null)
                {
                    obj.RunIntervalTime -= (int)(IntervalTime * 1000);
                    if (obj.RunIntervalTime <= 0)
                    {
                        obj.IntervalCallback?.Invoke();
                        obj.RunIntervalTime = obj.IntervalTime;
                    }
                }
                obj.RunTime -= (int)(IntervalTime * 1000);
                if (obj.RunTime <= 0)
                {
                    obj.OverCallback?.Invoke();
                    _timeDelList.Add(obj);
                }
            }

            for (int i = 0; i < _timeDelList.Count; i++)
            {
                timeDic.Remove(_timeDelList[i].ID);
                CachePoolManager.Instance.HideObj(_timeDelList[i]);
            }
            _timeDelList.Clear();
        }
    }
    //开启计时器管理
    public void StartTimeManager()
    {
        InfluencetimeDicCoroutine = MonoManager.Instance.StartCoroutine(StartTime(true, InfluencetimeDic));
        notInfluencetimeDicCoroutine = MonoManager.Instance.StartCoroutine(StartTime(false, notInfluencetimeDic));
    }
    //关闭计时器管理
    public void StopTimeManager()
    {
        MonoManager.Instance.StopCoroutine(InfluencetimeDicCoroutine);
        MonoManager.Instance.StopCoroutine(notInfluencetimeDicCoroutine);
    }
    /// <summary>
    /// 创建单个计时器
    /// </summary>
    /// <param name="isInfluenceTime">是否受到时间停止影响</param>
    /// <param name="time">总时长</param>
    /// <param name="OverCallback">结束回调</param>
    /// <param name="intervalTime">间隔时间</param>
    /// <param name="OutCallback">间隔回调</param>
    public int CreateTimeObj(bool isInfluenceTime,int time,UnityAction overCallback,int intervalTime = 0,UnityAction intervalCallback = null)
    {
        int timeID = ++_timeID;
        time *= 1000;
        intervalTime *= 1000;
        TimeObj obj = CachePoolManager.Instance.InstantiateObj<TimeObj>();
        obj.Init(timeID,time, overCallback, intervalTime, intervalCallback);
        if (isInfluenceTime) InfluencetimeDic.Add(timeID, obj);
        else notInfluencetimeDic.Add(timeID, obj);
        return timeID;
    }
    /// <summary>
    /// 删除单个计时器
    /// </summary>
    /// <param name="timeID"></param>
    public void RemoveTimeObj(int timeID)
    {
        if (InfluencetimeDic.TryGetValue(timeID, out var value1))
        {
            CachePoolManager.Instance.HideObj(value1);
            InfluencetimeDic.Remove(timeID);
        }
        if (notInfluencetimeDic.TryGetValue(timeID, out var value2))
        {
            CachePoolManager.Instance.HideObj(value2);
            notInfluencetimeDic.Remove(timeID);
        }
    }
    /// <summary>
    /// 重置计时器时间
    /// </summary>
    /// <param name="timeID"></param>
    public void ResetTimeObj(int timeID)
    {
        if (InfluencetimeDic.TryGetValue(timeID, out var value1)) value1.ResetTime();
        if (notInfluencetimeDic.TryGetValue(timeID, out var value2)) value2.ResetTime();
    }
    /// <summary>
    /// 暂停或继续计时器
    /// </summary>
    /// <param name="timeID"></param>
    /// <param name="openOrClose"></param>
    public void StartOrStopTimeObj(int timeID,bool openOrClose)
    {
        if (InfluencetimeDic.TryGetValue(timeID, out var value1)) value1.isRuning = openOrClose;
        if (notInfluencetimeDic.TryGetValue(timeID, out var value2)) value2.isRuning = openOrClose;
    }
}
