
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 缓存对象
/// </summary>
public class PoolData
{
    //未被使用容器
    private Stack<GameObject> StackData = new();
    //正在使用中容器
    private List<GameObject> UsedList = new();
    
    //抽屉父对象
    private GameObject StackObj;

    //抽屉最大存储数量
    private int maxNum;
    
    public int StackDataCount => StackData.Count;
    public int UsedListCount => UsedList.Count;

    public bool IsGenerate => UsedList.Count < maxNum;

    #region 构造函数 创建窗口布局父对象
    public PoolData(string objName, GameObject parent,GameObject stackObj)
    {
        if (CachePoolManager.isOpenLayout)
        {
            StackObj = new GameObject(objName);
            StackObj.transform.SetParent(parent.transform);
        }
        //创建抽屉时必有一个对象被生成，所以放入使用中容器
        PushUsedList(stackObj);
        
        maxNum = stackObj.GetComponent<CachePoolMaxNumManager>().maxNum;
    }
    #endregion
    
    /// <summary>
    /// 取出对象
    /// </summary>
    /// <returns></returns>
    public GameObject Pop()
    {
        GameObject obj;
        if (StackDataCount > 0)
        {
            obj = StackData.Pop();
            UsedList.Add(obj);
        }
        else
        {
            obj = UsedList[0];
            UsedList.RemoveAt(0);
            UsedList.Add(obj);
        }
        obj.SetActive(true);
        if (CachePoolManager.isOpenLayout) obj.transform.SetParent(null);
        return obj;
    }

    /// <summary>
    /// 存入对象
    /// </summary>
    /// <param name="obj"></param>
    public void PushStackData(GameObject obj)
    {
        obj.SetActive(false);
        if (CachePoolManager.isOpenLayout) obj.transform.SetParent(StackObj.transform);
        StackData.Push(obj);
        UsedList.Remove(obj);
    }

    public void PushUsedList(GameObject obj)
    {
        UsedList.Add(obj);
    }
}

public class PoolObjectBase{}

/// <summary>
/// 用于存储数据结构类对象
/// </summary>
/// <typeparam name="T"></typeparam>
public class PoolObject<T>:PoolObjectBase where T : class
{
    public Queue<T> PoolObjs = new();
}

/// <summary>
/// 数据结构类接口
/// 当存入缓存池时会调用
/// </summary>
public interface IPoolObject
{
    void ResetData(); //重置数据
}

public class CachePoolManager : SingletonManager<CachePoolManager>
{
    private CachePoolManager(){}
    
    //缓存池
    private Dictionary<string,PoolData> Pool = new();
    //存储不挂载的数据结构类对象
    private Dictionary<string,PoolObjectBase> PoolObjectDic = new();

    //窗口布局父对象
    private GameObject poolobj;

    //是否开启窗口布局功能
    public static bool isOpenLayout = false;
    
    /// <summary>
    /// 生成对象
    /// </summary>
    /// <param name="objRes">资源路径</param>
    /// <returns></returns>
    public GameObject InstantiateObj(string objRes)
    {
        GameObject obj;
        
        if (!Pool.ContainsKey(objRes) ||
            Pool[objRes].StackDataCount == 0 && Pool[objRes].IsGenerate)
        {
            obj = Object.Instantiate(Resources.Load<GameObject>(objRes));
            obj.name = objRes;

            if (!Pool.ContainsKey(objRes))
            {
                if (poolobj == null && isOpenLayout) poolobj = new GameObject("Pool");
                Pool.Add(objRes,new PoolData(objRes,poolobj,obj));
            }
            else Pool[objRes].PushUsedList(obj);
        }
        else obj = Pool[objRes].Pop();
        
        return obj;
    }

    // 生成音效
    public GameObject InstantiateSound(AudioClip clip)
    {
        GameObject obj;
        
        if (!Pool.ContainsKey(clip.name) ||
            Pool[clip.name].StackDataCount == 0 && Pool[clip.name].IsGenerate)
        {
            obj = new GameObject { name = clip.name };
            obj.AddComponent<AudioSource>().clip = clip;
            obj.AddComponent<CachePoolMaxNumManager>().maxNum = 10;

            if (!Pool.ContainsKey(clip.name))
            {
                if (poolobj == null && isOpenLayout) poolobj = new GameObject("Pool");
                Pool.Add(clip.name,new PoolData(clip.name,poolobj,obj));
            }
            else Pool[clip.name].PushUsedList(obj);
        }
        else obj = Pool[clip.name].Pop();
        
        return obj;
    }
    /// <summary>
    /// 获取数据结构类
    /// </summary>
    /// <typeparam name="T">类名</typeparam>
    /// <returns></returns>
    public T InstantiateObj<T>() where T: class,IPoolObject,new()
    {
        string name = typeof(T).Name;
        if (PoolObjectDic.ContainsKey(name))
        {
            PoolObject<T> poolobj = PoolObjectDic[name] as PoolObject<T>;
            if (poolobj.PoolObjs.Count > 0)
            {
                T obj = poolobj.PoolObjs.Dequeue();
                return obj;
            }
            else
            {
                T obj = new T();
                return obj;
            }
        }
        else
        {
            T obj = new T();
            return obj;
        }
    }
    /// <summary>
    /// 隐藏对象
    /// </summary>
    /// <param name="objRes">资源路径</param>
    /// <param name="obj">资源对象</param>
    public void HideObj(string objRes, GameObject obj) { Pool[objRes].PushStackData(obj); }
    /// <summary>
    /// 删除数据结构类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void HideObj<T>(T obj) where T : class,IPoolObject
    {
        string name = typeof(T).Name;
        PoolObject<T> poolobj;
        if (PoolObjectDic.TryGetValue(name, out var value)) poolobj = value as PoolObject<T>;
        else
        {
            poolobj = new();
            PoolObjectDic.Add(name, poolobj);
        }
        obj.ResetData(); 
        poolobj.PoolObjs.Enqueue(obj);
    }
    /// <summary>
    /// 缓存池清空
    /// </summary>
    public void PoolClear()
    {
        Pool.Clear();
        PoolObjectDic.Clear();
        poolobj = null;
    }
}
