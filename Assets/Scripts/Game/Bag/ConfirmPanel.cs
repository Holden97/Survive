using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanel : BasePanel
{
    /// <summary>
    /// 初始化确认面板方法
    /// </summary>
    /// <param name="tip">面板上提示的内容</param>
    private void Init(string tip)
    {
        FindComponent<Text>("tips").text = tip;
        FindComponent<Button>("btnConfirm").onClick.AddListener(() =>
        {
            UIMgr.GetInstance().HidePanel("ConfirmPanel");
        });
        FindComponent<Button>("btnClose").onClick.AddListener(() =>
        {
            UIMgr.GetInstance().HidePanel("ConfirmPanel");
        });
    }

    public static void ShowTips(string tips)
    {
        UIMgr.GetInstance().ShowPanel<ConfirmPanel>("ConfirmPanel", PanelLayer.system, (o) =>
        {
            o.Init(tips);
        });
    }

    public static void HideTips()
    {
        UIMgr.GetInstance().HidePanel("ConfirmPanel");
    }

}
