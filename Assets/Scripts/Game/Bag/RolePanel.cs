using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RolePanel : BasePanel
{
    public BagPanel bagPanel;
    //玩家所有装备的父对象
    public GameObject parent;
    private void Start()
    {
        //添加关闭按钮事件监听
        FindComponent<Button>("btnClose").onClick.AddListener(() =>
        {
            UIMgr.GetInstance().HidePanel("RolePanel");
        });
        //初始化面板信息
        Refresh();
        EventCenter.GetInstance().AddEventListener("装备物品更新", Refresh);



        ////添加拖拽事件监听
        //EventTrigger trigger = this.gameObject.AddComponent<EventTrigger>();
        //EventTrigger.Entry drag = new EventTrigger.Entry();
        //drag.eventID = EventTriggerType.BeginDrag;
        //drag.callback.AddListener(callback_drag);
        //List<EventTrigger.Entry> entries = new List<EventTrigger.Entry>();
        //entries.Add(drag);
        //trigger.triggers = entries;

    }

    private void Refresh()
    {
        foreach (Transform item in parent.transform)
        {
            ItemCell cell = item.gameObject.GetComponent<ItemCell>();
            cell.InitEquipInfo(Player.player.equip[cell.e_Equip]);
        }

        //存入文件
        GameDataMgr.ObjectToFile(Player.player, GameDataMgr.playerInfo_url);
    }

    //private void callback_drag(BaseEventData arg0)
    //{
    //    Debug.Log("Begindrag!");
    //}

    public override void ShowMe()
    {
        base.ShowMe();
        bagPanel.ShowMe();
    }


    public override void HideMe()
    {
        base.HideMe();
        bagPanel.HideMe();
        EventCenter.GetInstance().RemoveEventListener("装备物品更新", Refresh);
    }


}
