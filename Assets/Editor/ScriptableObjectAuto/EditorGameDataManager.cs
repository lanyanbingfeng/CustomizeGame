using UnityEditor;
using UnityEngine;
using System.IO;

public class EditorGameDataManager : EditorWindow
{
    // 存储用户输入的脚本名称
    private string scriptName = "NewData";
    
    // 标记当前是否为生成基类脚本模式
    private bool isCreatingBaseScript = false;

    [MenuItem("生成游戏数据/生成数据文件夹")]
    private static void CreateGameData()
    {
        // 定义目标文件夹路径
        string gameDataFolder = "Assets/Resources/GameData";
    
        // 检查并创建文件夹（如果不存在）
        if (!Directory.Exists(gameDataFolder))
        {
            // 创建文件夹并获取创建结果
            Directory.CreateDirectory(gameDataFolder);
            Debug.Log($"创建了GameData文件夹: {gameDataFolder}");
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("生成游戏数据/生成数据脚本")]
    private static void CreateScript()
    {
        EditorGameDataManager window = GetWindow<EditorGameDataManager>("输入脚本名称");
        window.minSize = new Vector2(300, 120);
        window.isCreatingBaseScript = false; // 普通脚本模式
        window.Show();
    }
    
    // 新增基类脚本菜单，使用相同的窗口逻辑
    [MenuItem("生成游戏数据/生成数据脚本基类")]
    private static void CreateBaseDataScript()
    {
        EditorGameDataManager window = GetWindow<EditorGameDataManager>("输入基类脚本名称");
        window.minSize = new Vector2(300, 120);
        window.isCreatingBaseScript = true; // 基类脚本模式
        window.scriptName = "BaseItem"; // 默认基类名称
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(20);
    
        // 根据模式显示不同的提示文本
        string title = isCreatingBaseScript ? 
            "请输入数据脚本基类名称：" : 
            "请输入ScriptableObject脚本名称：";
        GUILayout.Label(title, EditorStyles.boldLabel);
    
        GUI.SetNextControlName("ScriptNameField");
        scriptName = EditorGUILayout.TextField("脚本名称", scriptName);
    
        GUILayout.Space(10);
    
        if (GUILayout.Button("生成脚本", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(scriptName))
            {
                EditorUtility.DisplayDialog("错误", "脚本名称不能为空！", "确定");
                GUI.FocusControl("ScriptNameField");
                return;
            }
        
            if (ContainsInvalidChars(scriptName))
            {
                EditorUtility.DisplayDialog("错误", "名称包含非法字符！", "确定");
                GUI.FocusControl("ScriptNameField");
                return;
            }
        
            // 根据模式生成不同的脚本
            if (isCreatingBaseScript) GenerateBaseDataScript(scriptName);
            else GenerateScriptableObjectScript(scriptName);
            Close();
        }
    
        if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
        {
            Event.current.Use();
        
            if (string.IsNullOrEmpty(scriptName))
            {
                EditorUtility.DisplayDialog("错误", "脚本名称不能为空！", "确定");
                GUI.FocusControl("ScriptNameField");
                return;
            }
        
            if (ContainsInvalidChars(scriptName))
            {
                EditorUtility.DisplayDialog("错误", "名称包含非法字符！", "确定");
                GUI.FocusControl("ScriptNameField");
                return;
            }
         
            if (isCreatingBaseScript) GenerateBaseDataScript(scriptName);
            else GenerateScriptableObjectScript(scriptName);
            Close();
        }
    
        if (Event.current.type == EventType.Layout && string.IsNullOrEmpty(GUI.GetNameOfFocusedControl()))
        {
            GUI.FocusControl("ScriptNameField");
        }
    }
    
    //生成数据脚本基类
    private void GenerateBaseDataScript(string className)
    {
        string folderPath = "Assets/Scripts/GameData/BaseData";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        string scriptPath = $"{folderPath}/{className}.cs";
        
        if (File.Exists(scriptPath))
        {
            if (EditorUtility.DisplayDialog("提示", $"基类脚本{className}.cs已存在，是否覆盖？", "是", "否"))
            {
                File.Delete(scriptPath);
            }
            else return;
        }
        
        // 基类脚本内容（抽象类继承ScriptableObject）
        string scriptContent = $@"using UnityEngine;
public abstract class {className} : ScriptableObject
{{
    
}}";
        
        File.WriteAllText(scriptPath, scriptContent);
        AssetDatabase.Refresh();
        
        Object newScript = AssetDatabase.LoadAssetAtPath<Object>(scriptPath);
        Selection.activeObject = newScript;
        
        Debug.Log($"成功生成基类脚本：{scriptPath}");
    }

    // 生成ScriptableObject脚本
    private void GenerateScriptableObjectScript(string className)
    {
        // 脚本保存路径（确保Scripts/GameData文件夹存在）
        string folderPath = "Assets/Scripts/GameData/Data";
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        
        // 脚本完整路径
        string scriptPath = $"{folderPath}/{className}.cs";
        
        // 检查脚本是否已存在
        if (File.Exists(scriptPath))
        {
            if (EditorUtility.DisplayDialog("提示", "脚本已存在，是否覆盖？", "是", "否"))
            {
                File.Delete(scriptPath);
            }
            else return;
        }
        
        // 脚本内容模板（添加特殊标记用于识别自动生成的脚本）
        string scriptContent = $@"using UnityEngine;

// AutoGeneratedByTool
[CreateAssetMenu(fileName = ""{className}"", menuName = ""ScriptableObjects/{className}"")]
public class {className} : ScriptableObjectManager<{className}>
{{
    // 在这里添加自定义数据字段
}}";
        
        // 写入文件
        File.WriteAllText(scriptPath, scriptContent);
        
        // 刷新资源数据库，让Unity识别新脚本
        AssetDatabase.Refresh();
        
        // 选中新创建的脚本，方便用户查看
        Object newScript = AssetDatabase.LoadAssetAtPath<Object>(scriptPath);
        Selection.activeObject = newScript;
        
        Debug.Log($"成功生成脚本：{scriptPath}");
    }

    // 检查名称是否包含非法字符
    private bool ContainsInvalidChars(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            if (name.Contains(c.ToString())) return true;
        }
        return false;
    }
}