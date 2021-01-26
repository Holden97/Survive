using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 资源加载模块
/// 涉及到的知识点
/// 1.异步加载
/// 2.lambda表达式与委托
/// 3.协程
/// 4.泛型
/// 
/// 提供两种接口
/// 1.同步加载资源
/// 2.异步加载资源
/// 
/// 封装GameObject类型资源的实例化操作
/// 其他类型的资源不做额外处理
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    /// <summary>
    /// 同步加载接口
    /// </summary>
    /// <param name="path">加载资源的路径</param>

    public T Load<T>(string path) where T : Object
    {
        T res = Resources.Load<T>(path);
        if (res is GameObject)
            return GameObject.Instantiate(res);
        return res;
    }

    /// <summary>
    /// 异步加载接口
    /// </summary>
    /// <param name="path">加载资源的路径</param>
    /// <param name="fun">加载完成后执行的操作</param>
    public void LoadAsyn<T>(string path, UnityAction<T> fun) where T : Object
    {
        MonoMgr.GetInstance().StartCoroutine(LoadAsynC(path, fun));
    }

    /// <summary>
    /// 封装的异步加载操作协程函数
    /// </summary>
    private IEnumerator LoadAsynC<T>(string path, UnityAction<T> fun) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync(path);
        yield return r;
        if (r.asset is GameObject)
            fun(GameObject.Instantiate(r.asset) as T);
        else
            fun(r.asset as T);
    }
}
