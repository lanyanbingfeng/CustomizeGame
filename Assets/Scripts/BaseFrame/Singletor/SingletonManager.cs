using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 不继承 MonoBehaviours 的单例基类
/// ！注意！
/// 继承之后必须要有私有构造函数
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonManager<T> where T : class
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type type = typeof(T);
                //找到私有构造函数
                ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                    null, 
                    Type.EmptyTypes, 
                    null);
                if (info != null) instance = info.Invoke(null) as T;
                else Debug.LogWarning("单例类 " + type.Name + " 没有私有构造函数");
            }
            return instance;
        }
    }
}
