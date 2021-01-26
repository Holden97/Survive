using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{

    private void Start()
    {
        //添加自定义事件监听
        EventCenter.GetInstance().AddEventListener<Item>("购买物品", callBack_handle_buy);
        EventCenter.GetInstance().AddEventListener("保存玩家数据", saveData);
        //添加按钮事件监听
        FindComponent<Button>("bag").onClick.AddListener(() =>
        {
            UIMgr.GetInstance().ShowPanel<BagPanel>("BagPanel", PanelLayer.top);
        });
        FindComponent<Button>("shop").onClick.AddListener(() =>
        {
            UIMgr.GetInstance().ShowPanel<ShopPanel>("shopPanel", PanelLayer.top);
        });
        FindComponent<Button>("role").onClick.AddListener(() =>
        {
            UIMgr.GetInstance().ShowPanel<RolePanel>("RolePanel", PanelLayer.top);
        });
    }

    private void saveData()
    {
        GameDataMgr.ObjectToFile(Player.player, GameDataMgr.playerInfo_url);
    }

    private void callBack_handle_buy(Item o)
    {
        if (Player.player.BuyItem(o))
        {
            ConfirmPanel.ShowTips("购买成功");
            EventCenter.GetInstance().EventTrigger("背包物品更新");
        }
        else
            ConfirmPanel.ShowTips("购买失败，余额不足！");
        Refresh();
    }


    public override void ShowMe()
    {
        base.ShowMe();
        Refresh();
    }
    /// <summary>
    /// 刷新面板信息
    /// </summary>
    private void Refresh()
    {
        FindComponent<Text>("TextNAME").text = Player.player.name;
        FindComponent<Text>("TextLEVEL").text = Player.player.lev.ToString();
        FindComponent<Text>("textGold").text = Player.player.prop["gold"].quantity.ToString();
        FindComponent<Text>("textDia").text = Player.player.prop["dia"].quantity.ToString();
        FindComponent<Text>("textMine").text = Player.player.prop["mine"].quantity.ToString();
    }

    public void AddGold()
    {
        Player.player.prop["gold"].ModifyCurrency(1000);
        GameDataMgr.ObjectToFile<Player>(Player.player, GameDataMgr.playerInfo_url);
        Refresh();
    }

    public void AddDia()
    {
        Player.player.prop["dia"].ModifyCurrency(1000);
        GameDataMgr.ObjectToFile<Player>(Player.player, GameDataMgr.playerInfo_url);
        Refresh();
    }

    public void AddMine()
    {
        Player.player.prop["mine"].ModifyCurrency(1000);
        GameDataMgr.ObjectToFile<Player>(Player.player, GameDataMgr.playerInfo_url);
        Refresh();
    }

    public override void HideMe()
    {
        base.HideMe();
        EventCenter.GetInstance().RemoveEventListener<Item>("购买物品", callBack_handle_buy);
        EventCenter.GetInstance().RemoveEventListener("保存玩家数据", saveData);

    }

}
