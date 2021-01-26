using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagPanel : BasePanel
{
    public Toggle[] toggles;
    public static List<BagItem> curList;
    public GameObject content;
    private void Start()
    {
        //背包script在角色面板被复用一部分，close按钮这一部分并未被复用
        if (FindComponent<Button>("btnClose") != null)
            FindComponent<Button>("btnClose").onClick.AddListener(() =>
            {
                UIMgr.GetInstance().HidePanel("BagPanel");
            });
        //查找到所有目录toggle
        toggles = GetComponentsInChildren<Toggle>();
        //为toggle[]添加事件监听
        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener(onToggleChange);
        }
        //添加事件监听
        EventCenter.GetInstance().AddEventListener("背包物品更新", Refresh);
        //为什么这里不能通过DiscardItem的参数类型提示泛型T的类型呢？
        EventCenter.GetInstance().AddEventListener<ItemCell>("丢弃物品", DiscardItem);


    }

    private void DiscardItem(ItemCell o)
    {
        Player.player.RemoveItem(o.itemInfo.id, o.item.num);
        EventCenter.GetInstance().EventTrigger("保存玩家数据");
    }

    public override void ShowMe()
    {
        base.ShowMe();
        FindComponent<Toggle>("props").isOn = true;
        curList = Player.player.bag["props"];
        //初始化填充
        if (content.transform.childCount == 0 && curList.Count != 0)
            CreateNewList(curList, content);
    }


    /// <summary>
    /// Toggle Group的回调
    /// </summary>
    /// <param name="isOn"></param>
    private void onToggleChange(bool isOn)
    {
        if (isOn)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        //删除旧List信息
        foreach (Transform item in content.transform)
        {
            Destroy(item.gameObject);
        }
        //切换当前List信息
        curList = Player.player.bag[GetCurToggleName()];
        //添加新的ItemCell信息
        CreateNewList(curList, content);
    }

    /// <summary>
    /// 获取当前Toggle名称
    /// </summary>
    /// <returns></returns>
    public string GetCurToggleName()
    {
        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
                return toggle.name;
        }
        return null;
    }
    /// <summary>
    /// 用curList的信息填充背包
    /// </summary>
    /// <param name="curList">填充时使用的List</param>
    /// <param name="parent">物体的父对象</param>

    public void CreateNewList(List<BagItem> curList, GameObject parent)
    {
        foreach (BagItem item in curList)
        {
            GameObject itemCell = ResMgr.GetInstance().Load<GameObject>("UI/Cell/ItemCell");
            //每个单元格填充信息
            itemCell.transform.SetParent(parent.transform);
            itemCell.GetComponent<ItemCell>().InitInfo(item);
        }
    }

    public override void HideMe()
    {
        base.HideMe();
        EventCenter.GetInstance().RemoveEventListener("背包物品更新", Refresh);
        EventCenter.GetInstance().RemoveEventListener<ItemCell>("丢弃物品", DiscardItem);
    }

}
