using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemCell;
/// <summary>
/// 格子事件处理类
/// 1.处理拖拽存放
/// 2.处理鼠标事件
/// </summary>
public class ItemCellMgr : BaseManager<ItemCellMgr>
{
    //dragPanel，正在拖拽的物品
    DragPanel dragPanel;
    //拖拽出去的物品信息
    private ItemCell OutCell;
    //拖拽出去的物品的格子信息
    private BagItem bagItem;
    //bagPanel对应的当前的list
    private List<BagItem> bagPanel_curList;
    //拖拽进去物品的格子
    private ItemCell InCell;
    //拖拽动作是否正在执行的标志位
    bool isDraging = false;

    public void Init()
    {
        //监听鼠标事件
        EventCenter.GetInstance().AddEventListener<ItemCell>("鼠标进入", pointerEnter);
        EventCenter.GetInstance().AddEventListener<ItemCell>("鼠标移出", pointerExit);
        EventCenter.GetInstance().AddEventListener<ItemCell>("鼠标开始拖拽", pointerBeginDrag);
        EventCenter.GetInstance().AddEventListener<BaseEventData>("鼠标正在拖拽", pointerDrag);
        EventCenter.GetInstance().AddEventListener<ItemCell>("鼠标结束拖拽", pointerEndDrag);


    }

    private void pointerEndDrag(ItemCell arg0)
    {
        //获取当前背包的curList
        bagPanel_curList = BagPanel.curList;

        //销毁拖动面板
        UIMgr.GetInstance().HidePanel("DragPanel");
        dragPanel = null;
        Debug.Log("pointerEndDragCallback");
        //重置标志位
        isDraging = false;
        //记录拖入和拖出的物品/格子信息
        BagItem itemIn = null;
        BagItem itemOut = null;
        //如果拖入的格子中有物品，那么就得到该物品信息
        if (InCell != null && InCell.item.id != 0)
            itemIn = InCell.item;
        if (OutCell != null && OutCell.item.id != 0)
            itemOut = OutCell.item;
        //现在只给予一种装备物品和一种脱下物品的方式
        //1.装备物品：将物品拖动到正确的装备位置
        //2.脱下物品：将物品从装备栏中拖出

        //装备物品逻辑
        if (OutCell != null && InCell != null && InCell.e_Equip != (int)E_equip.notEquip && OutCell.e_Equip != (int)E_equip.notEquip && InCell.e_Equip == OutCell.e_Equip && InCell.e_GridType == E_gridType.equip && OutCell.e_GridType == E_gridType.bag)
        {
            EquipItem();
            EventCenter.GetInstance().EventTrigger("背包物品更新");
            EventCenter.GetInstance().EventTrigger("装备物品更新");
        }
        //脱下物品逻辑
        if (InCell == null && OutCell != null && OutCell.e_GridType == E_gridType.equip)
        {
            RemoveEquip();
            EventCenter.GetInstance().EventTrigger("背包物品更新");
            EventCenter.GetInstance().EventTrigger("装备物品更新");

        }
        //丢弃物品逻辑
        if (InCell == null && OutCell.e_GridType == E_gridType.bag)
        {
            EventCenter.GetInstance().EventTrigger("丢弃物品", OutCell);
            EventCenter.GetInstance().EventTrigger("背包物品更新");

        }





        //最后两个格子都置空
        InCell = null;
        OutCell = null;


    }

    private void RemoveEquip()
    {
        GameDataMgr.GetInstance().RemoveEquip(OutCell.e_Equip);
    }

    /// <summary>
    /// 装备物品
    /// </summary>
    /// <param name="o">物品信息</param>
    /// <param name="e_Equip">装备类型</param>
    private void EquipItem()
    {
        //更新数据
        //装备物品

        GameDataMgr.GetInstance().AddEquip(OutCell.item.id);

    }

    private void pointerDrag(BaseEventData arg0)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                UIMgr.GetInstance().canvasObj.transform as RectTransform,
                (arg0 as PointerEventData).position,
                (arg0 as PointerEventData).pressEventCamera,
                out pos
                );
        if (dragPanel != null)
            dragPanel.transform.localPosition = pos;
        //Debug.Log(pos);
        Debug.Log("pointerDragCallback");

    }

    private void pointerBeginDrag(ItemCell arg0)
    {
        UIMgr.GetInstance().ShowPanel<DragPanel>("DragPanel", PanelLayer.system, (o) =>
        {
            dragPanel = o;
            o.transform.localScale = new Vector3(1, 1, 1);
            o.Init(arg0.itemInfo);
        });
        OutCell = arg0;
        isDraging = true;
        Debug.Log("pointerBeginDragCallback");
    }

    private void pointerExit(ItemCell arg0)
    {
        if (isDraging)
            InCell = null;
        UIMgr.GetInstance().HidePanel("ItemPanel");
        Debug.Log("pointerExitCallback");
    }

    private void pointerEnter(ItemCell arg0)
    {
        if (isDraging)
        {
            InCell = arg0;
            Debug.Log("pointerEnterCallback");
            return;
        }
        //用此判断条件提点arg0!=null是因为item的两个属性是public的所以被初始化了不为空
        if (arg0.item.id != 0)
            UIMgr.GetInstance().ShowPanel<ItemPanel>("ItemPanel", PanelLayer.system, (p) =>
            {
                p.InitInfo(arg0.item);
                p.transform.position = arg0.FindComponent<Image>("ImgBK").transform.position;
            });
    }
}
