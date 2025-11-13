using System;
using System.Data;
using System.IO;
using System.Text;
using Excel;
using UnityEditor;
using UnityEngine;

public class ExcelDateLoad
{
    // Excel 存储位置
    private static string Excel_Path = Application.dataPath + "/Artres/Excel/";
    // 数据结构类存储位置
    private static string Data_Class_Path = Application.dataPath + "/Scripts/ExcelData/DataClass/"; 
    // 容器类存储位置
    private static string Container_Path = Application.dataPath + "/Scripts/ExcelData/Container/";
    // 删除数据文件路径
    private static string Delect_Path = Application.dataPath + "/Scripts/";

    [MenuItem("Generate/生成Excel存储位置")]
    private static void GenerateExcelPath()
    {
        if (!Directory.Exists(Excel_Path)) Directory.CreateDirectory(Excel_Path);
        else Debug.LogWarning("文件存储位置已存在");
        
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Generate/GenerateExcelData")]
    private static void GenerateExcelData()
    {
        // 如果文件路径存在就返回 文件夹信息
        DirectoryInfo dInfo = Directory.CreateDirectory(Excel_Path);
        // 得到所有文件信息
        FileInfo[] files = dInfo.GetFiles("*.xlsx");
        if (files.Length == 0)
        {
            Debug.LogWarning(Excel_Path + " 路径下没有Excel表格数据");
            return;
        }
        // 声明一个 Excel 的所有表数据
        DataTableCollection tables;
        foreach (FileInfo file in files)
        {
            //打开单个 Excel
            using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                tables = excelReader.AsDataSet().Tables;
                fs.Close();
            }
            
            foreach (DataTable table in tables)
            {
                Debug.Log(table.TableName);
                
                //生成数据结构类
                GenerateExcelDataClass(table);
                //生成容器类
                GenerateExcelContainer(table);
                //生成二进制文件
                GenerateExcelBinary(table);
            }
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Generate/DelectExcelData")]
    private static void DelectExcelData()
    {
        if (File.Exists(Delect_Path + "/ExcelData.meta"))
        {
            File.Delete(Delect_Path + "/ExcelData.meta");
            Directory.Delete(Delect_Path + "/ExcelData", true);
        }
        else Debug.LogWarning(Delect_Path + "/ExcelDate 路径文件已删除");
        if (File.Exists(Application.dataPath + "/StreamingAssets.meta"))
        {
            File.Delete(Application.dataPath + "/StreamingAssets.meta");
            Directory.Delete(Application.dataPath + "/StreamingAssets", true);
        }
        else Debug.LogWarning(Application.streamingAssetsPath + " 路径文件已删除");
        AssetDatabase.Refresh();
    }
    
    //生成数据结构类方法
    private static void GenerateExcelDataClass(DataTable table)
    {
        // 得到字段名
        DataRow veriableNameList = GetVeriableNameRow(table);
        // 得到字段类型
        DataRow veriableTypeList = GetVeriableTypeRow(table);
        // 如果数据结构类文件夹不存在就创建
        if (!Directory.Exists(Data_Class_Path)) Directory.CreateDirectory(Data_Class_Path);
        string dataText = "public class " + table.TableName + "\n{\n";
        for (int i = 0; i < table.Columns.Count; i++)
        {
            dataText += "   public " + veriableTypeList[i] + " " + veriableNameList[i] + ";\n";
        }
        dataText += "}";
        
        File.WriteAllText(Data_Class_Path + table.TableName + ".cs", dataText);
    }
    //生成容器类方法
    private static void GenerateExcelContainer(DataTable table)
    {
        //得到主键索引
        int keyIndex = GetKeyIndex(table);
        //得到字段类型行
        DataRow typerow = GetVeriableTypeRow(table);
        //如果存储文件夹不存在就创建
        if (!Directory.Exists(Container_Path)) Directory.CreateDirectory(Container_Path);
        string dataText = "using System.Collections.Generic;\n";
        dataText += "public class " + table.TableName + "Container\n{\n";
        dataText += "   public Dictionary<" + typerow[keyIndex] + "," + table.TableName + "> ";
        dataText += "dictionary = new Dictionary<" + typerow[keyIndex] + "," + table.TableName + ">();\n";
        dataText += "}";
        File.WriteAllText(Container_Path + table.TableName + "Container.cs", dataText);
    }
    //生成二进制文件方法
    private static void GenerateExcelBinary(DataTable table)
    {
        //如果存储位置不存在就创建
        if (!Directory.Exists(BinaryDateMgr.Binary_Data_Path)) Directory.CreateDirectory(BinaryDateMgr.Binary_Data_Path);
        using (FileStream fs = new FileStream( BinaryDateMgr.Binary_Data_Path + table.TableName + ".bfdat", FileMode.OpenOrCreate, FileAccess.Write))
        {
            //先存表格有多少行,不包括无用的配置信息
            fs.Write(BitConverter.GetBytes(table.Rows.Count - 4), 0, 4);
            //再存主键字段名
            string keyName = GetVeriableNameRow(table)[GetKeyIndex(table)].ToString();
            byte[] keyNameBytes = Encoding.UTF8.GetBytes(keyName);
            fs.Write(BitConverter.GetBytes(keyNameBytes.Length),0,4);
            fs.Write(keyNameBytes, 0, keyNameBytes.Length);
            //最后开始存表格数据
            DataRow dataType = GetVeriableTypeRow(table);
            for (int i = 4; i < table.Rows.Count; i++)
            {
                DataRow dataRow = table.Rows[i];
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    switch (dataType[j].ToString())
                    {
                        case "int":
                            fs.Write(BitConverter.GetBytes(int.Parse(dataRow[j].ToString())), 0, 4);
                            break;
                        case "float":
                            fs.Write(BitConverter.GetBytes(float.Parse(dataRow[j].ToString())), 0, 4);
                            break;
                        case "bool":
                            fs.Write(BitConverter.GetBytes(bool.Parse(dataRow[j].ToString())), 0, 1);
                            break;
                        case "string":
                            byte[] bytes = Encoding.UTF8.GetBytes(dataRow[j].ToString());
                            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                        default:
                            Debug.Log(table.TableName + "表格中的第 " + (j + 1) + " 列类型不匹配");
                            break;
                    }
                }
            }
        }
    }
    //得到字段名方法
    private static DataRow GetVeriableNameRow(DataTable table)
    {
        return table.Rows[0];
    }
    //得到字段类型方法
    private static DataRow GetVeriableTypeRow(DataTable table)
    {
        return table.Rows[1];
    }
    //得到主键索引的方法
    private static int GetKeyIndex(DataTable table)
    {
        DataRow row = table.Rows[2];
        for (int i = 0; i < table.Columns.Count; i++)
        {
            if ((string)row[i] == "key")
            {
                return i;
            }
        }
        return 0;
    } 
}
