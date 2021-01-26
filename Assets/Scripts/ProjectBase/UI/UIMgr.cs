using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 面板层级，框架中为四级依次为
/// 1.底层(最底部）
/// 2.中层
/// 3.顶层
/// 4.系统级
/// </summary>
public enum PanelLayer
{
    bot,
    mid,
    top,
    system
}
/// <summary>
/// UI管理器
/// 1.管理所有显示的面板
/// 2.提供给外部显示/隐藏面板的接口
/// </summary>

public class UIMgr : BaseManager<UIMgr>
{
    Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    public GameObject canvasObj;
    public GameObject eventSystemObj;
    /// UI路径
    string canvasPath = "UI/Canvas";
    string eventPath = "UI/EventSystem";


    /// <summary>
    /// 构造函数
    /// 创建canvas和EventSystem对象（从预设中加载），并且使其在转场时不被销毁

    /// </summary>
    public UIMgr()
    {
        canvasObj = ResMgr.GetInstance().Load<GameObject>(canvasPath);
        eventSystemObj = ResMgr.GetInstance().Load<GameObject>(eventPath);
        GameObject.DontDestroyOnLoad(canvasObj);
        GameObject.DontDestroyOnLoad(eventSystemObj);
    }
    /// <summary>
    /// 显示面板接口
    /// </summary>
    /// <typeparam name="T">面板脚本类型</typeparam>
    /// <param name="name">面板依附对象名称</param>
    /// <param name="layer">面板所处层级</param>
    /// <param name="callBack">面板回调函数</param>
    public void ShowPanel<T>(string name, PanelLayer layer = PanelLayer.mid, UnityAction<T> callBack = null) where T : BasePanel
    {
        //面板已经被创建显示出来了
        if (panelDic.ContainsKey(name))
        {
            //执行BasePanel中的ShowMe方法
            panelDic[name].ShowMe();
            callBack?.Invoke(panelDic[name] as T);
            return;
        }
        else
        {
            ResMgr.GetInstance().LoadAsyn<GameObject>("UI/Panel/" + name, (panelObj) =>
              {
                  //调整其在canvas层级中的位置设置
                  Transform parent = SelectParent(layer);
                  panelObj.transform.SetParent(parent);
                  panelObj.transform.localPosition = Vector3.zero;
                  panelObj.transform.localScale = Vector3.one;
                  (panelObj.transform as RectTransform).offsetMax = Vector2.zero;
                  (panelObj.transform as RectTransform).offsetMin = Vector2.zero;
                  //其他对面板对象的操作
                  T panel = panelObj.GetComponent<T>();
                  //执行BasePanel中的ShowMe方法
                  panel.ShowMe();
                  //对panel脚本组件的设置
                  callBack?.Invoke(panel);
                  //收入字典
                  if (!panelDic.ContainsKey(name))
                      panelDic.Add(name, panel);
              });
        }
    }

    private Transform SelectParent(PanelLayer layer)
    {
        Transform parent = null;
        switch (layer)
        {
            case PanelLayer.bot:
                parent = canvasObj.transform.Find("Bot");
                break;
            case PanelLayer.mid:
                parent = canvasObj.transform.Find("Mid");
                break;
            case PanelLayer.top:
                parent = canvasObj.transform.Find("Top");
                break;
            case PanelLayer.system:
                parent = canvasObj.transform.Find("Sys");
                break;
            default:
                Debug.LogError("寻找面板层级出错！");
                break;
        }

        return parent;
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="name">面板名</param>
    public void HidePanel(string name)
    {
        if (panelDic.ContainsKey(name))
        {
            //执行BasePanel中的HideMe方法
            panelDic[name].HideMe();
            GameObject.Destroy(panelDic[name].gameObject);
            panelDic.Remove(name);
        }
    }
}
