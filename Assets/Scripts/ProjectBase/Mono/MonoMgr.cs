using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

/// <summary>
/// 封装场景中的MonoController
/// 使用单例模式
/// </summary>
public class MonoMgr : BaseManager<MonoMgr>
{
    public MonoController monoController;

    public MonoMgr()
    {
        GameObject ctrlObj = new GameObject("MonoController");
        monoController = ctrlObj.AddComponent<MonoController>();
    }

    public void AddUpdateListener(UnityAction fun)
    {
        monoController.AddUpdateListener(fun);
    }

    public void RemoveUpdateListener(UnityAction fun)
    {
        monoController.RemoveUpdateListener(fun);

    }


    public Coroutine StartCoroutine(string methodName)
    {
        return monoController.StartCoroutine(methodName);
    }
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return monoController.StartCoroutine(routine);
    }
    public Coroutine StartCoroutine(string methodName, [DefaultValue("null")]object value)
    {
        return monoController.StartCoroutine(methodName, value);
    }
    public Coroutine StartCoroutine_Auto(IEnumerator routine)
    {
        return monoController.StartCoroutine(routine);
    }
}
