
using System;
using UnityEditor;
using UnityEngine;

public class GiveChildObject : EditorWindow
{
    private static GiveChildObject window;
    private Transform[] SelectionObjects;
    private Transform[] ChildObjects;
    
    private bool isFocus; // 打开窗口的焦点
    
    [MenuItem("自定义窗口/赋予子对象")]
    private static void ShowWindow()
    {
        if (Selection.activeGameObject == null)
        {
            EditorUtility.DisplayDialog("提示", "请先选择一个对象", "确定");
            return;
        }
        window = GetWindow<GiveChildObject>("赋予子对象");
        window.position = new Rect(750,240,300,300);
        window.Show();
    }
    
    private void OnEnable()
    {
        SelectionObjects = Selection.transforms;
        isFocus = true;
    }
    
    private int currentChildNum;
    
    
    private void OnGUI()
    {
        GUI.SetNextControlName("IntNum");
        currentChildNum = EditorGUILayout.IntField("子对象列表",currentChildNum);
        if (isFocus)
        {
            EditorGUI.FocusTextInControl("IntNum");
            isFocus = false;
        }
        EditorGUI.indentLevel++;
        if (currentChildNum != (ChildObjects?.Length ?? 0)) Array.Resize(ref ChildObjects,currentChildNum);
        for (int i = 0; i < currentChildNum; i++)
        {
            ChildObjects[i] = EditorGUILayout.ObjectField(new GUIContent("对象" + i),
                ChildObjects[i], 
                typeof(Transform),
                false) as Transform;
        }
        if (GUILayout.Button("确认", GUILayout.Height(30)))
        {
            foreach (var selection in SelectionObjects)
            {
                foreach (var child in ChildObjects)
                {
                    if (child != null) Instantiate(child, selection);
                }
            }
            window.Close();
        }
    }
}
