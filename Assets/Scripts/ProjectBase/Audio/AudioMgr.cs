using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 音效管理模块
/// 音效资源采用异步加载的方式
/// 对外提供
/// 1.添加背景音效接口
/// 2.暂停背景音效接口
/// 3.停止背景音效接口
/// 4.添加音效接口
/// 5.停止所有音效接口
/// 6.停止单个音效接口
/// 7.调整背景音乐音量接口
/// 8.调整音效音量接口
/// </summary>
public class AudioMgr : BaseManager<AudioMgr>
{
    GameObject clipsMgrObj = new GameObject("ClipsMgrObj");//音效管理Obj
    GameObject bgmMgrObj = new GameObject("BgmMgrObj");//BGM管理Obj
    AudioSource bgmSource = null;//BGM音源
    List<AudioSource> clipsSources = new List<AudioSource>();//音效音源列表
    float bgmVolume = 1;//背景音量
    float clipsVolume = 1;//音效音量
    /// <summary>
    /// 帧更新，自动删去音效列表中那些已经播放完成的音效
    /// </summary>
    public AudioMgr()
    {
        MonoMgr.GetInstance().AddUpdateListener(Update);
    }
    void Update()
    {
        foreach (AudioSource cur in clipsSources)
        {
            if (!cur.isPlaying)
            {
                clipsSources.Remove(cur);
                Object.Destroy(cur);
            }
        }
    }
    /// <summary>
    /// 添加BGM
    /// BGM路径为 "Resource/Audio/BGM/" + name
    /// </summary>
    /// <param name="name">BGM名称</param>
    public void AddBGM(string name)
    {
        bgmSource = bgmMgrObj.AddComponent<AudioSource>();
        ResMgr.GetInstance().LoadAsyn<AudioClip>("Audio/BGM/" + name, (o) =>
        {
            bgmSource.clip = o;
            bgmSource.loop = true;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();

        });


    }
    /// <summary>
    /// 暂停BGM
    /// </summary>
    public void PauseBGM()
    {
        if (bgmSource != null)
            bgmSource.Pause();
    }
    /// <summary>
    /// 停止BGM
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }
    /// <summary>
    /// 添加音效
    /// 音效路径为  "Resource/Audio/Clips/" + name
    /// </summary>
    /// <param name="name">音效名称</param>
    public void AddClip(string name, bool isLoop, UnityAction<AudioSource> callBack = null)
    {
        ResMgr.GetInstance().LoadAsyn<AudioClip>("Audio/Clips/" + name, (o) =>
          {
              //异步加载结束后添加音源
              AudioSource curSource = clipsMgrObj.AddComponent<AudioSource>();

              curSource.clip = o;
              curSource.volume = clipsVolume;
              curSource.loop = isLoop;
              curSource.Play();

              clipsSources.Add(curSource);

              //如果外部需要对当前音效进行操作，用回调对其操作
              callBack?.Invoke(curSource);
          });
    }


    /// <summary>
    /// 停止所有音效
    /// </summary>
    public void StopAllClips()
    {
        foreach (AudioSource cur in clipsSources)
        {
            cur.Stop();
        }
    }
    /// <summary>
    /// 停止单个音效
    /// 单个音效停止时立刻移除组件和列表中的相应的对象
    /// </summary>
    /// <param name="cur">音效对应的AudioSource组件</param>
    public void StopClip(AudioSource cur)
    {
        if (clipsSources.Contains(cur))
        {
            cur.Stop();
            clipsSources.Remove(cur);
            Object.Destroy(cur);
        }
    }
    /// <summary>
    /// 修改BGM音量
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBGMVolume(float v)
    {
        if (bgmSource != null)
            bgmSource.volume = v;
        bgmVolume = v;
    }
    /// <summary>
    /// 修改音效音量
    /// </summary>
    /// <param name="v"></param>
    public void ChangeClipsVolume(float v)
    {
        foreach (AudioSource cur in clipsSources)
        {
            cur.volume = v;
        }
        clipsVolume = v;
    }
}
