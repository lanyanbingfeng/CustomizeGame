using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class BinaryDateMgr : SingletonManager<BinaryDateMgr>
{
    /// 数据存储地址
    private static string SAVE_FILE_PATH = Application.persistentDataPath + "/Date/";
    /// 二进制文件存储位置
    public static string Binary_Data_Path = Application.streamingAssetsPath + "/BinaryData/";
    /// 存储所有表的字典
    private Dictionary<string,object> tableDic = new();
    
    private BinaryDateMgr(){ LoadAllData(); }

    /// <summary>
    /// 加载所有表 到字典中
    /// </summary>
    public void LoadAllData()
    {
        
    }
    /// <summary>
    /// 加载 Excel 表格的数据
    /// </summary>
    /// <typeparam name="T">容器类类名</typeparam>
    /// <typeparam name="K">数据结构类类名</typeparam>
    public void LoadExcelData<T,K>()
    {
        using (FileStream fs = File.Open(Application.streamingAssetsPath + "/BinaryData/" + typeof(K).Name + ".bfdat",
                   FileMode.Open,FileAccess.Read))
        {
            //得到二进制文件所有字节
            byte[] datas = new byte[fs.Length];
            fs.Read(datas, 0, datas.Length);
            fs.Close();
            
            //先读取有多少行
            int index = 0;
            int dataLength = BitConverter.ToInt32(datas, index);
            index += 4;
            //再读取主键字段名
            int keyNameLength = BitConverter.ToInt32(datas, index);
            index += 4;
            string keyName = Encoding.UTF8.GetString(datas, index, keyNameLength);
            index += keyNameLength;
            //通过反射实例化一个容器类
            Type containerType = typeof(T);
            object containerObj = Activator.CreateInstance(containerType);
            //通过反射获得数据结构类的类型
            Type dataClassType = typeof(K);
            //获得数据结构类的所有字段
            FieldInfo[] fields = dataClassType.GetFields();
            //然后开始读取所有数据 每循环一行数据 实例化一个数据结构类 并赋值
            for (int i = 0; i < dataLength; i++)
            {
                object dataClassObj = Activator.CreateInstance(dataClassType);
                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType == typeof(int))
                    {
                        field.SetValue(dataClassObj, BitConverter.ToInt32(datas, index));
                        index += 4;
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        field.SetValue(dataClassObj, BitConverter.ToSingle(datas, index));
                        index += 4;
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(dataClassObj, BitConverter.ToBoolean(datas, index));
                        index += 1;
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        int strlen = BitConverter.ToInt32(datas, index);
                        index += 4;
                        field.SetValue(dataClassObj, Encoding.UTF8.GetString(datas, index, strlen));
                        index += strlen;
                    }
                }
                
                //先得到 容器类中 的字典字段
                object dicObj = containerType.GetField("dictionary").GetValue(containerObj);
                //通过反射 获得 字典的 Add 方法
                MethodInfo dic_Add_Method = dicObj.GetType().GetMethod("Add");
                //获得 数据结构类 的主键的值
                object keyValue = dataClassType.GetField(keyName).GetValue(dataClassObj);
                //赋值完成 把数据结构类存入容器类
                dic_Add_Method.Invoke(dicObj, new object[] { keyValue, dataClassObj });
            }
            
            tableDic.Add(typeof(T).Name, containerObj);
        }
    }
    /// <summary>
    /// 获得指定表数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetExcelDataTable<T>() where T : class
    {
        string tableName = typeof(T).Name;
        if (tableDic.ContainsKey(tableName)) return tableDic[tableName] as T;
        return null;
    }
    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="date">数据对象</param>
    /// <param name="fileName">保存名称</param>
    public void SaveDate(object date,string fileName)
    {
        //如果没有数据文件夹就创建一个
        if (!Directory.Exists(SAVE_FILE_PATH)) Directory.CreateDirectory(SAVE_FILE_PATH);
        using (FileStream fs = new FileStream(SAVE_FILE_PATH + fileName + ".bfdat", FileMode.OpenOrCreate, FileAccess.Write))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, date);
            Debug.Log("数据存储成功，存储路径 " + SAVE_FILE_PATH + fileName + ".bfdat");
            fs.Close();
        }
    }
    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="fileName">保存名称</param>
    /// <typeparam name="T">数据类型</typeparam>
    /// <returns></returns>
    public T LoadDate<T>(string fileName) where T : class
    {
        T date; //声明一个泛型 T 
        if (!File.Exists(SAVE_FILE_PATH + fileName + ".bfdat"))
        {
            Debug.Log(SAVE_FILE_PATH + fileName + ".bfdat 不存在数据");
            return default(T);
        }
        using (FileStream fs = File.Open(SAVE_FILE_PATH + fileName + ".bfdat", FileMode.Open, FileAccess.Read))
        {
            BinaryFormatter bf = new BinaryFormatter();
            date = bf.Deserialize(fs) as T;
            Debug.Log("数据读取成功，数据位置 " + SAVE_FILE_PATH + fileName + ".bfdat");
            fs.Close();
        }
        
        return date;
    }
}
