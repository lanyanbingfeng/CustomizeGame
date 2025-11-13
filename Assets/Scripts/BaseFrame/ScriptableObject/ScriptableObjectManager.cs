using UnityEngine;

public class ScriptableObjectManager<T> : ScriptableObject where T : ScriptableObject
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null) instance = Resources.Load<T>("GameData/" + typeof(T).Name);
            if (instance == null) instance = CreateInstance<T>(); 
            return instance;
        }
    }
}
