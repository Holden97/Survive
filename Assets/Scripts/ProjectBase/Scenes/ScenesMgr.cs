using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
/// <summary>
/// 场景切换模块
/// 为外部提供切换场景的公共接口
/// </summary>
public class ScenesMgr : BaseManager<ScenesMgr>
{
    /// <summary>
    /// 同步切换场景
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="fun">切换场景后所执行的事件</param>
    public void LoadScene(string name, UnityAction fun)
    {
        //场景同步加载
        SceneManager.LoadScene(name);
        //执行fun
        fun();
    }

    public void LoadSceneAsyn(string name, UnityAction fun)
    {
        MonoMgr.GetInstance().StartCoroutine(ReallyLoadSceneAsyn(name, fun));

    }

    public IEnumerator ReallyLoadSceneAsyn(string name, UnityAction fun)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        while (!ao.isDone)
            EventCenter.GetInstance().EventTrigger("进度更新", ao.progress);
        yield return ao.progress;
        fun();
    }
}
