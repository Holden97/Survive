using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 面板基类
/// 1.寻找自身面板下的UI控件的接口
/// 2.显示/隐藏自己的虚函数
/// 
/// 在awake中去查找所需控件并存入字典容器
/// </summary>
public class BasePanel : MonoBehaviour
{
    /// <summary>
    /// UI字典
    /// string 是控件对应的GameObject名称
    /// List<UIBehaviour> 是UI列表
    /// </summary>
    Dictionary<string, List<UIBehaviour>> UIDic = new Dictionary<string, List<UIBehaviour>>();

    void Awake()
    {
        FindAllComponents<Button>();
        FindAllComponents<Image>();
        FindAllComponents<Text>();
        FindAllComponents<Toggle>();
        FindAllComponents<Slider>();
        FindAllComponents<ScrollRect>();

    }

    private void FindAllComponents<T>() where T : UIBehaviour
    {
        T[] components = GetComponentsInChildren<T>();
        foreach (T component in components)
        {
            string name = component.gameObject.name;
            if (UIDic.ContainsKey(name))
                UIDic[name].Add(component);
            else
                UIDic.Add(name, new List<UIBehaviour>() { component });

        }
    }
    /// <summary>
    /// 寻找指定名称的GameObject上的控件的公共接口
    /// </summary>
    /// <typeparam name="T">寻找的控件类型</typeparam>
    /// <param name="name">游戏物体名</param>
    /// <returns></returns>
    public T FindComponent<T>(string name) where T : UIBehaviour
    {
        if (UIDic.ContainsKey(name))
        {
            foreach (UIBehaviour item in UIDic[name])
            {
                if (item is T)
                    return item as T;
            }
        }
        return null;

    }
    /// <summary>
    /// 显示面板时执行的操作
    /// </summary>
    public virtual void ShowMe()
    {

    }
    /// <summary>
    /// 隐藏面板时执行的操作
    /// </summary>
    public virtual void HideMe()
    {

    }
}
