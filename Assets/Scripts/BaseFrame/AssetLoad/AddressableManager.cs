
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : SingletonManager<AddressableManager>
{
    private AddressableManager() { }
    
    private Dictionary<string, IEnumerator> AssetDic = new();

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="name">资源名</param>
    /// <param name="callback">回调</param>
    /// <typeparam name="T"></typeparam>
    public void LoadAssetAsync<T>(string name,Action<AsyncOperationHandle<T>> callback)
    {
        string keyName = name + "_" + typeof(T).Name;
        AsyncOperationHandle<T> handle;
        //如果加载过了
        if (AssetDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<T>)AssetDic[keyName];
            //加载完成
            if (handle.IsDone) callback(handle);
            //没有加载完成
            else handle.Completed += obj =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded) callback(obj);
            };
            return;
        }
        //如果没有加载过
        handle = Addressables.LoadAssetAsync<T>(name);
        handle.Completed += obj =>
        {
            if (obj.Status == AsyncOperationStatus.Succeeded) callback(obj);
            else
            {
                Debug.LogWarning(keyName + "加载失败");
                if (AssetDic.ContainsKey(keyName)) AssetDic.Remove(keyName);
            }
        };
        AssetDic.Add(keyName, handle);
    }
    
    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    public void ReleaseAssets<T>(string name)
    {
        string keyName = name + "_" + typeof(T).Name;
        if (AssetDic.ContainsKey(keyName))
        {
            AsyncOperationHandle<T> handle = (AsyncOperationHandle<T>)AssetDic[keyName];
            Addressables.Release(handle);
            AssetDic.Remove(keyName);
        }
    }

    /// <summary>
    /// 异步加载多个资源
    /// 或 指定名称和标签资源
    /// </summary>
    /// <param name="mode">资源查找方式</param>
    /// <param name="callback">回调</param>
    /// <param name="names">查找关键词</param>
    /// <typeparam name="T"></typeparam>
    public void LoadAssetAsync<T>( Addressables.MergeMode mode,Action<T> callback,params string[] names)
    {
        List<string> lists = new List<string>(names);
        string keyName = "";
        foreach (string name in names) keyName += name + "_";
        keyName += "_" + typeof(T).Name;
        AsyncOperationHandle<IList<T>> handle;
        //如果存在资源
        if (AssetDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<IList<T>>)AssetDic[keyName];
            //如果加载完了
            if (handle.IsDone)
            {
                foreach (T asset in handle.Result) callback(asset);
            }
            else handle.Completed += obj =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded) foreach (T asset in handle.Result) callback(asset);
            };
            return;
        }
        //如果不存在资源
        handle = Addressables.LoadAssetsAsync<T>(lists,callback,mode);
        handle.Completed += obj =>
        {
            if (obj.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogWarning(keyName + "加载失败");
                if (AssetDic.ContainsKey(keyName)) AssetDic.Remove(keyName);
            }
        };
        AssetDic.Add(keyName, handle);
    }
    
    /// <summary>
    /// （可以控制查找到的资源）
    /// 异步加载多个资源
    /// 或 指定名称和标签资源 
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="callback"></param>
    /// <param name="names"></param>
    /// <typeparam name="T"></typeparam>
    public void LoadAssetAsync<T>( Addressables.MergeMode mode,Action<AsyncOperationHandle<IList<T>>> callback,params string[] names)
    {
        List<string> lists = new List<string>(names);
        string keyName = "";
        foreach (string name in names) keyName += name + "_";
        keyName += "_" + typeof(T).Name;
        AsyncOperationHandle<IList<T>> handle;
        //如果存在资源
        if (AssetDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<IList<T>>)AssetDic[keyName];
            //如果加载完了
            if (handle.IsDone) callback(handle);
            else handle.Completed += obj =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded) callback(handle);
            };
            return;
        }
        //如果不存在资源
        handle = Addressables.LoadAssetsAsync<T>(lists,obj=>{},mode);
        handle.Completed += obj =>
        {
            if (obj.Status == AsyncOperationStatus.Succeeded) callback(handle);
            else
            {
                Debug.LogWarning(keyName + "加载失败");
                if (AssetDic.ContainsKey(keyName)) AssetDic.Remove(keyName);
            }
        };
        AssetDic.Add(keyName, handle);
    }
    
    public void ReleaseAssets<T>(params string[] names)
    {
        List<string> lists = new List<string>(names);
        string keyName = "";
        foreach (string name in names) keyName += name + "_";
        keyName += "_" + typeof(T).Name;
        if (AssetDic.ContainsKey(keyName))
        {
            AsyncOperationHandle<IList<T>> handle = (AsyncOperationHandle<IList<T>>)AssetDic[keyName];
            Addressables.Release(handle);
            AssetDic.Remove(keyName);
        }
    }
}
