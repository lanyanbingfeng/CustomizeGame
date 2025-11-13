using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class PlayerPrefsDateManager : SingletonManager<PlayerPrefsDateManager>
{
    private PlayerPrefsDateManager() { }
    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="date">数据对象</param>
    /// <param name="keyName">保存名字</param>
    public void SaveData(object date , string keyName)
    {
        Type datetype = date.GetType();
        FieldInfo[] fields = datetype.GetFields();
        
        string newKeyName;
        for (int i = 0; i < fields.Length; i++)
        {
            newKeyName = keyName + "_" + datetype.Name + "_" + fields[i].FieldType.Name + "_" + fields[i].Name;
            SaveValue(fields[i].GetValue(date),newKeyName);
        }
    }
    private void SaveValue(object date, string keyName)
    {
        Type datetype = date.GetType();
        if (datetype == typeof(int))
        {
            PlayerPrefs.SetInt(keyName, (int)date);
        }
        else if (datetype == typeof(float))
        {
            PlayerPrefs.SetFloat(keyName, (float)date);
        }
        else if (datetype == typeof(string))
        {
            PlayerPrefs.SetString(keyName, date.ToString());
        }
        else if (datetype == typeof(bool))
        {
            PlayerPrefs.SetInt(keyName, (bool)date ? 1 : 0);
        }
        else if (typeof(IList).IsAssignableFrom(datetype))
        {
            IList list = date as IList;
            
            PlayerPrefs.SetInt(keyName, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                SaveValue(list[i], keyName + "_" + i);
            }
        }
        else if (typeof(IDictionary).IsAssignableFrom(datetype))
        {
            IDictionary list = date as IDictionary;
            
            PlayerPrefs.SetInt(keyName, list.Count);
            int Index = 0;
            foreach (object obj in list.Keys)
            {
                SaveValue(obj,keyName + "_key_" + Index);
                SaveValue(list[obj],keyName + "_value_" + Index);
                Index++;
            }
        } 
        else SaveData(date,keyName);
    }
    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="datetype">数据类型</param>
    /// <param name="keyName">保存名字</param>
    /// <returns></returns>
    public object LoadData(Type datetype, string keyName)
    {
        object date = Activator.CreateInstance(datetype);
        FieldInfo[] fields = datetype.GetFields();
        string newKeyName = "";
        for (int i = 0; i < fields.Length; i++)
        {
            newKeyName = keyName + "_" + datetype.Name + "_" + fields[i].FieldType.Name + "_" + fields[i].Name;
            fields[i].SetValue(date,LoadValue(fields[i].FieldType,newKeyName));
        }
        return date;
    }
    private object LoadValue(Type datetype, string keyName)
    {
        if (datetype == typeof(int))
        {
            return PlayerPrefs.GetInt(keyName,0);
        }
        else if (datetype == typeof(float))
        {
            return PlayerPrefs.GetFloat(keyName,0);
        }
        else if (datetype == typeof(string))
        {
            return PlayerPrefs.GetString(keyName,"");
        }
        else if (datetype == typeof(bool))
        {
            return PlayerPrefs.GetInt(keyName,0) == 1 ? true : false;
        }
        else if (typeof(IList).IsAssignableFrom(datetype))
        {
            IList list = Activator.CreateInstance(datetype) as IList;
            
            int ListLen = PlayerPrefs.GetInt(keyName,0);
            for (int i = 0; i < ListLen; i++)
            {
                list.Add(LoadValue(datetype.GetGenericArguments()[0],keyName + "_" + i));
            }
            return list;
        }
        else if (typeof(IDictionary).IsAssignableFrom(datetype))
        {
            IDictionary Dic = Activator.CreateInstance(datetype) as IDictionary;
            
            int DicLen = PlayerPrefs.GetInt(keyName,0);
            for (int i = 0; i < DicLen; i++)
            {
                Dic.Add(
                    LoadValue(datetype.GetGenericArguments()[0],keyName + "_key_" + i),
                    LoadValue(datetype.GetGenericArguments()[1],keyName + "_value_" + i)
                    );
            }
            return Dic;
        }
        else
        {
            return LoadData(datetype,keyName);
        }
    }
}

