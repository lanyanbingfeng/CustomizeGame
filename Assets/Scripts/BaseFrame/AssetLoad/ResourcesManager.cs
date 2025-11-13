
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AssetBase
{
    //引用计数
    public int refCount;
}

public class AssetInfo<T> : AssetBase
{
    //资源
    public T asset;
    //回调函数
    public UnityAction<T> callBack;
    //协程
    public Coroutine coroutine;
    //是否卸载
    public bool isDel;
    
    public void AddRefCount(){refCount++;}
    
    public void SubRefCount(){refCount--;}
}

public class ResourcesManager : SingletonManager<ResourcesManager>
{
    private ResourcesManager() { }

    private Dictionary<string,AssetBase> assetDic = new();
    
    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <typeparam name="T"></typeparam>
    public T Load<T>(string path) where T : Object
    {
        string resName = path + "_" + typeof(T).Name;
        AssetInfo<T> assetInfo = new();
        if (!assetDic.ContainsKey(resName))
        {
            T res = Resources.Load<T>(path);
            assetInfo.asset = res;
            assetInfo.AddRefCount();
            assetDic.Add(resName,assetInfo);
            return assetInfo.asset;
        }

        assetInfo = assetDic[resName] as AssetInfo<T>;
        if (assetInfo.asset == null)
        {
            MonoManager.Instance.StopCoroutine(assetInfo.coroutine);
            T res = Resources.Load<T>(path);
            assetInfo.asset = res;
            assetInfo.AddRefCount();
            assetInfo.callBack?.Invoke(assetInfo.asset);
            assetInfo.callBack = null;
            assetInfo.coroutine = null;
            return assetInfo.asset;
        }

        return assetInfo.asset;
    }
    
    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="callBack">回调函数</param>
    /// <typeparam name="T"></typeparam>
    public void AsyncLoad<T>(string path,UnityAction<T> callBack) where T : Object
    {
        string resName = path + "_" + typeof(T).Name;
        AssetInfo<T> assetInfo;
        if (!assetDic.ContainsKey(resName))
        {
            assetInfo = new();
            assetInfo.AddRefCount();
            assetDic.Add(resName, assetInfo);
            assetInfo.callBack += callBack;
            assetInfo.coroutine = MonoManager.Instance.StartCoroutine(AsyncLoadAsset<T>(path));
        }
        else
        {
            assetInfo = assetDic[resName] as AssetInfo<T>;
            assetInfo.AddRefCount();
            //没有加载完毕
            if (assetInfo.asset == null) assetInfo.callBack += callBack;
            //加载完毕
            else callBack?.Invoke(assetInfo.asset);
        }
    }
    private IEnumerator AsyncLoadAsset<T>(string path) where T : Object
    {
        ResourceRequest rr = Resources.LoadAsync<T>(path);
        yield return rr;
        string resName = path + "_" + typeof(T).Name;
        if (assetDic.ContainsKey(resName))
        {
            AssetInfo<T> assetInfo = assetDic[resName] as AssetInfo<T>;
            assetInfo.asset = rr.asset as T;
            if (assetInfo.refCount == 0) UnLoadAsset<T>(path,assetInfo.isDel,null,false);
            else
            {
                assetInfo.callBack?.Invoke(assetInfo.asset);
                assetInfo.callBack = null;
                assetInfo.coroutine = null;
            }
        }
    }

    /// <summary>
    /// 卸载指定资源
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="isDel">是否删除对象</param>
    /// <param name="callBack">回调</param>
    /// <param name="isredSub"></param>
    /// <typeparam name="T"></typeparam>
    public void UnLoadAsset<T>(string path,bool isDel = false,UnityAction<T> callBack = null,bool isredSub = true) where T : Object
    {
        string resName = path + "_" + typeof(T).Name;
        if (assetDic.ContainsKey(resName))
        {
            AssetInfo<T> assetInfo = assetDic[resName] as AssetInfo<T>;
            if(isredSub)assetInfo.SubRefCount();
            assetInfo.isDel = isDel;
            if (assetInfo.asset != null && assetInfo.refCount == 0 && assetInfo.isDel)
            {
                assetDic.Remove(resName);
                Resources.UnloadAsset(assetInfo.asset);
            }
            else if (assetInfo.asset == null) assetInfo.callBack -= callBack;
        }
    }

    /// <summary>
    /// 异步卸载所有未使用的资源
    /// </summary>
    /// <param name="callBack">回调函数</param>
    public void UnloadUnusedAsset(UnityAction callBack)
    {
        MonoManager.Instance.StartCoroutine(UnloadUnusedAssets(callBack));
    }
    private IEnumerator UnloadUnusedAssets(UnityAction callBack)
    {
        List<string> list = new List<string>();
        foreach (string path in assetDic.Keys) if (assetDic[path].refCount == 0) list.Add(path);
        foreach (string path in list) assetDic.Remove(path);
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        callBack?.Invoke();
    }
}
