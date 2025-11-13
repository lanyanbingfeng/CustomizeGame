

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneMgr : SingletonManager<SceneMgr>
{
    private SceneMgr() { }

    public void LoadScene(string sceneName,UnityAction callback = null)
    {
        SceneManager.LoadScene(sceneName);
        callback?.Invoke();
    }

    public void LoadSceneAsync(string sceneName, UnityAction callback = null)
    {
        MonoManager.Instance.StartCoroutine(IE_LoadSceneAsync(sceneName, callback));
    }

    private IEnumerator IE_LoadSceneAsync(string sceneName, UnityAction callback = null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        while (!ao.isDone)
        {
            EventCenterManager.Instance.EventTrigger<float>(E_EventType.LoadSceneChange, ao.progress);
            yield return null;
        }
        EventCenterManager.Instance.EventTrigger<float>(E_EventType.LoadSceneChange, 1);
        callback?.Invoke();
    }
}
