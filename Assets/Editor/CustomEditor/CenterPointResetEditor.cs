
using UnityEditor;
using UnityEngine;

public class CenterPointResetEditor : EditorWindow
{
    private static CenterPointResetEditor window;
    private GameObject obj;
    private GameObject newPoint;
    
    [MenuItem("自定义窗口/中心点重置")]
    private static void CenterPointReset()
    {
        if (Selection.activeGameObject == null)
        {
            EditorUtility.DisplayDialog("提示", "请先选择一个对象", "确定");
            return;
        }
        window = GetWindow<CenterPointResetEditor>("选择位置");
        window.position = new Rect(750,240,200,300);
        window.Show();
    }

    

    private void OnEnable()
    {
        obj = Selection.activeGameObject;
        newPoint = new GameObject();
        newPoint.transform.position = obj.transform.position;
        Selection.activeGameObject = newPoint;
    }

    private void OnDestroy()
    {
        if (newPoint == null) return;
        if (newPoint.transform.childCount == 0) DestroyImmediate(newPoint);
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("放置父对象（新点）的位置");
        EditorGUILayout.Space(10);
        if (GUILayout.Button("确认", GUILayout.Height(30)))
        {
            obj.transform.parent = newPoint.transform;
            window.Close();
        }
    }
}
