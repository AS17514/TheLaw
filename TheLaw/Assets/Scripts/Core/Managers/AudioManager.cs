using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum E_BGM
{
    StartMenu,
    LevelSelect,
    Story,
    Battle,
    GameOver,
    Ending,
    // test
    Donglang,
    Dongbeak,
}

public enum E_SFX
{

}
public class AudioManager : ManagerMonoBase<AudioManager>
{
    // 分别创建两种字典
    Dictionary<E_BGM, AudioClip> bgms = new Dictionary<E_BGM, AudioClip>();
    Dictionary<E_SFX, AudioClip> sfxs = new Dictionary<E_SFX, AudioClip>();
    // 创建组件引用
    GameObject bgmPlayer;
    GameObject sfxPlayer;
    AudioSource bgmComponent;
    // 初始音量
    public float BGMVolume = 0.5f;
    public float SFXVolume = 0.5f;

    // 初始化音频列表
    List<AudioSource> audios = new List<AudioSource>();

    // 用来给主入口调用来初始化
    public void Init() { }
    void Update()
    {
        // 音效播放完毕自动移除组件
        for (int i = audios.Count - 1; i >= 0; i--)
        {
            if (!audios[i].isPlaying)
            {
                // 可以直接移除组件
                GameObject.Destroy(audios[i]);
                audios.RemoveAt(i);
            }
        }
        // 更新音量
    }
    void Awake()
    {
        // 设置bgm播放组件
        bgmPlayer = new GameObject { name = "BGMPlayer" };
        bgmComponent = bgmPlayer.AddComponent<AudioSource>();
        // 加载所有背景音乐
        LoadAllBGM();
        // 设置音效播放组件
        sfxPlayer = new GameObject { name = "SFXPlayer" };
        // 加载所有音效
        LoadAllSFX();
        // 设置过场景不摧毁播放器
        GameObject.DontDestroyOnLoad(bgmPlayer);
        GameObject.DontDestroyOnLoad(sfxPlayer);

        // 监听事件
        // 音频播放
        EventCenter.Instance.AddEventListener(E_EventType.Audio_Play_BGM, (obj) =>
        {
            object[] objs = (object[])obj;
            PlayBGM((E_BGM)Enum.Parse(typeof(E_BGM), objs[0].ToString()), (bool)objs[1]);
        });
        EventCenter.Instance.AddEventListener(E_EventType.Audio_Play_SFX, (obj) =>
        {
            object[] objs = (object[])obj;
            PlaySFX((E_SFX)Enum.Parse(typeof(E_SFX), objs[0].ToString()), (bool)objs[1]);
        });
        // 音频音量改变
        EventCenter.Instance.AddEventListener(E_EventType.Audio_Update_BGMVolume, (obj) =>
        {
            SetBGMVolume((float)obj);
        });
        EventCenter.Instance.AddEventListener(E_EventType.Audio_Update_SFXVolume, (obj) =>
        {
            SetSFXVolume((float)obj);
        });
    }

    #region BGM
    /// <summary>
    /// 加载所有背景音乐并添加到字典
    /// </summary>
    void LoadAllBGM()
    {
        // 加载所有背景音乐
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audios/BGM");
        // 遍历数组，按名称添加进字典
        foreach (AudioClip clip in clips)
        {
            if (Enum.TryParse<E_BGM>(clip.name, out E_BGM clipEnum))
            {
                bgms.Add(clipEnum, clip);
            }
        }
    }
    /// <summary>
    /// 播放指定背景音乐，也能用来切歌
    /// </summary>
    /// <param name="bgm">背景音乐枚举</param>
    /// <param name="isLoop">是否循环</param>
    public void PlayBGM(E_BGM bgm, bool isLoop = true)
    {
        if (bgmComponent == null)
        {
            return;
        }
        bgmComponent.clip = bgms[bgm];
        bgmComponent.loop = isLoop;
        bgmComponent.Play();
    }
    /// <summary>
    /// 暂停播放，继续播放时从暂停点开始
    /// </summary>
    public void PauseBGM()
    {
        if (bgmComponent == null)
        {
            return;
        }
        bgmComponent.Pause();
    }
    /// <summary>
    /// 停止播放，继续播放时重新开始
    /// </summary>
    public void StopBGM()
    {
        if (bgmComponent == null)
        {
            return;
        }
        bgmComponent.Stop();
    }
    /// <summary>
    /// 设置bgm音量并实时更新音量大小
    /// </summary>
    /// <param name="volume">音量大小</param>
    public void SetBGMVolume(float volume)
    {
        BGMVolume = volume;
        bgmComponent.volume = volume;
    }
    #endregion

    #region GameAudio
    /// <summary>
    /// 加载所有音效并添加到字典
    /// </summary>
    private void LoadAllSFX()
    {
        // 加载所有音效
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audios/SFX");
        // 遍历数组，按名称添加进字典
        foreach (AudioClip clip in clips)
        {
            if (Enum.TryParse<E_SFX>(clip.name, out E_SFX clipEnum))
            {
                sfxs.Add(clipEnum, clip);
            }
        }
    }
    /// <summary>
    /// 添加对应音效组件，并添加到列表，最后播放音效
    /// </summary>
    /// <param name="sfx">音效枚举</param>
    /// <param name="isLoop">是否循环播放</param>
    public void PlaySFX(E_SFX sfx, bool isLoop)
    {
        AudioSource audio = sfxPlayer.AddComponent<AudioSource>();
        audios.Add(audio);
        audio.clip = sfxs[sfx];
        audio.loop = isLoop;
        audio.Play();
    }
    /// <summary>
    /// 停止所有音效，随后自动删除并移出列表
    /// </summary>
    public void StopAllSFX()
    {
        foreach (AudioSource audio in audios)
        {
            audio.Stop();
        }
    }
    /// <summary>
    /// 设置音效音量并实时更新所有音效组件的音量
    /// </summary>
    /// <param name="volume">音量大小</param>
    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
        foreach (AudioSource sfx in audios)
        {
            sfx.volume = volume;
        }
    }
    #endregion
}
