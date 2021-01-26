using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 用于为没有继承MonoBehaviour的类提供
/// 1.帧更新接口
/// 2.协程接口
/// </summary>
public class MonoController : MonoBehaviour
{
    private event UnityAction updateEvent;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateEvent != null)
            updateEvent();
    }

    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }

    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }

}
