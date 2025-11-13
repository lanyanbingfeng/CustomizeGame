
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

/// <summary>
/// 音乐管理器
/// 配合 Addressable 使用
/// </summary>
public class AudioManager : SingletonManager<AudioManager>
{
    //背景音乐对象
    private AudioSource backgroundMusicObj;
    private float backgroundMusicVolume = 0.5f;
    
    //判断是否播放结束的音效列表
    private List<AudioSource> soundList = new();
    //音效对象
    private GameObject soundObj;
    private float soundVolume = 0.5f;
    private bool isPlaying = true;

    private AudioManager() { MonoManager.Instance.AddFixedUpdateEvent(JudgeSoundIsEnd); }

    private void JudgeSoundIsEnd()
    {
        if (!isPlaying) return;
        for (int i = soundList.Count - 1; i >= 0 ; i--)
        {
            if (!soundList[i].isPlaying)
            {
                soundList[i].clip = null;
                CachePoolManager.Instance.HideObj(soundList[i].name, soundList[i].gameObject);
                soundList.RemoveAt(i);
            }
        }
    }
    /// <summary>
    /// 播放或切换背景音乐
    /// </summary>
    /// <param name="musicName">音频文件名，类型为 BackgroundMusic</param>
    public void PlayBackgroundMusic(string musicName)
    {
        if (backgroundMusicObj == null)
        {
            GameObject backgroundMusicObj = new GameObject("BackgroundMusic");
            Object.DontDestroyOnLoad(backgroundMusicObj);
            this.backgroundMusicObj = backgroundMusicObj.AddComponent<AudioSource>();
        }
        AddressableManager.Instance.LoadAssetAsync<AudioClip>(Addressables.MergeMode.Intersection, handle =>
        {
            backgroundMusicObj.clip = handle;
            backgroundMusicObj.loop = true;
            backgroundMusicObj.volume = backgroundMusicVolume;
            backgroundMusicObj.Play();
        },musicName,"BackgroundMusic");
    }
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBackgroundMusic()
    {
        if (backgroundMusicObj != null) backgroundMusicObj.Stop();
    }
    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBackgroundMusic()
    {
        if (backgroundMusicObj != null) backgroundMusicObj.Pause();
    }
    /// <summary>
    /// 改变背景音乐大小
    /// </summary>
    /// <param name="volume">音乐大小</param>
    public void ChangeMusicVolume(float volume)
    {
        backgroundMusicVolume = volume;
        if (backgroundMusicObj != null) backgroundMusicObj.volume = backgroundMusicVolume;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="soundName">音效文件名 类型为 Sound</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="callback">回调</param>
    public void PlaySound(string soundName,bool isLoop = false,UnityAction<AudioSource> callback = null)
    {
        AddressableManager.Instance.LoadAssetAsync<AudioClip>(Addressables.MergeMode.Intersection, handle =>
        {
            AudioSource audioSource = CachePoolManager.Instance.InstantiateSound(handle).GetComponent<AudioSource>();
            audioSource.Stop();
            audioSource.clip = handle;
            audioSource.volume = soundVolume;
            audioSource.loop = isLoop;
            audioSource.Play();
            if (!soundList.Contains(audioSource)) soundList.Add(audioSource);
            callback?.Invoke(audioSource);
        },soundName,"Sound");
    }
    /// <summary>
    /// 停止指定音效
    /// </summary>
    /// <param name="sound"></param>
    public void StopSound(AudioSource sound)
    {
        if (soundList.Contains(sound))
        {
            sound.clip = null;
            CachePoolManager.Instance.HideObj(sound.name,sound.gameObject);
            soundList.Remove(sound);
        }
    }
    /// <summary>
    /// 改变所有音效大小
    /// </summary>
    /// <param name="volume"></param>
    public void ChangeSoundVolume(float volume)
    {
        soundVolume = volume;
        foreach (AudioSource audioSource in soundList) audioSource.volume = soundVolume;
    }
    /// <summary>
    /// 暂停或继续所有音效
    /// </summary>
    /// <param name="isPause"></param>
    public void PauseSound(bool isPause)
    {
        if (isPause)
        {
            foreach (AudioSource audioSource in soundList)
            {
                audioSource.Pause();
                isPlaying = false;
            }
        }
        else
        {
            foreach (AudioSource audioSource in soundList)
            {
                audioSource.Play();
                isPlaying = true;
            }
        }
    }
    /// <summary>
    /// 停止所有音效
    /// </summary>
    public void StopAllSound()
    {
        foreach (AudioSource audioSource in soundList.ToList())
        {
            audioSource.Stop();
            audioSource.clip = null;
            CachePoolManager.Instance.HideObj(audioSource.name, audioSource.gameObject);
        }
        soundList.Clear();
    }
}
