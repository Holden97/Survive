using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCell : BasePanel
{
    //dragPanel
    DragPanel dragPanel;
    //物品详细信息
    public Item itemInfo = null;
    //物品背包信息
    public BagItem item = null;
    //物品格子的格子类型，默认为背包格子
    public E_gridType e_GridType = E_gridType.bag;
    //物品格子的装备类型，默认为非装备类型
    public E_equip e_Equip = E_equip.notEquip;
    /// <summary>
    /// 初始化显示数量的itemCell当中的信息
    /// </summary>
    /// <param name="bagItem"></param>
    /// 
    public void InitInfo(BagItem bagItem)
    {
        Refresh(bagItem);
    }
    /// <summary>
    /// 初始化不显示数量的装备item信息
    /// </summary>
    /// <param name="o">item</param>
    public void InitEquipInfo(BagItem bagItem)
    {
        Refresh(bagItem);
    }

    /// <summary>
    /// 刷新UI显示
    /// </summary>
    /// <param name="bagItem">格子物品信息</param>
    public void Refresh(BagItem bagItem)
    {
        //如果格子是背包类型且格子中为空，则删除这个格子
        if (e_GridType == E_gridType.bag && bagItem == null)
        {
            Destroy(this.gameObject);
            return;
        }
        //默认icon不显示
        FindComponent<Image>("Icon").gameObject.SetActive(false);
        //更新UI
        if (bagItem != null && bagItem.id != 0)
        {
            //分发信息
            item = bagItem;
            itemInfo = GameDataMgr.GetInstance().GetItem(bagItem.id);
            e_Equip = (E_equip)itemInfo.equipType;
            //更新UI
            FindComponent<Image>("Icon").gameObject.SetActive(true);
            FindComponent<Image>("Icon").sprite = ResMgr.GetInstance().Load<Sprite>(GameDataMgr.icon_url_pre + bagItem.id);
            if (FindComponent<Text>("textNum") != null)
                FindComponent<Text>("textNum").text = bagItem.num.ToString();
        }
    }


    /// <summary>
    /// 添加EventTrigger组件，监听鼠标移入移出事件
    /// </summary>
    private void Start()
    {
        EventTrigger eventTrigger = FindComponent<Image>("ImgBK").gameObject.AddComponent<EventTrigger>();
        List<EventTrigger.Entry> entries = new List<EventTrigger.Entry>();

        //添加鼠标进入事件
        EventTrigger.Entry p_entry = new EventTrigger.Entry();
        //添加事件类型
        p_entry.eventID = EventTriggerType.PointerEnter;
        //添加事件监听
        p_entry.callback.AddListener(pointerEnterCallback);
        //添加此对象
        entries.Add(p_entry);

        //添加鼠标移出事件，逻辑同上
        EventTrigger.Entry p_out = new EventTrigger.Entry();
        p_out.eventID = EventTriggerType.PointerExit;
        p_out.callback.AddListener(pointerExitCallback);
        entries.Add(p_out);

        //添加鼠标开始拖拽事件
        EventTrigger.Entry p_beginDrag = new EventTrigger.Entry();
        p_beginDrag.eventID = EventTriggerType.BeginDrag;
        p_beginDrag.callback.AddListener(pointerBeginDragCallback);
        entries.Add(p_beginDrag);
        //添加鼠标结束拖拽事件
        EventTrigger.Entry p_endDrag = new EventTrigger.Entry();
        p_endDrag.eventID = EventTriggerType.EndDrag;
        p_endDrag.callback.AddListener(pointerEndDragCallback);
        entries.Add(p_endDrag);
        //添加鼠标拖拽中的事件
        EventTrigger.Entry p_drag = new EventTrigger.Entry();
        p_drag.eventID = EventTriggerType.Drag;
        p_drag.callback.AddListener(pointerDragCallback);
        entries.Add(p_drag);

        eventTrigger.triggers = entries;
    }

    private void pointerDragCallback(BaseEventData arg0)
    {
        EventCenter.GetInstance().EventTrigger("鼠标正在拖拽", arg0);
    }

    private void pointerEndDragCallback(BaseEventData arg0)
    {
        EventCenter.GetInstance().EventTrigger("鼠标结束拖拽", this);
    }

    private void pointerBeginDragCallback(BaseEventData arg0)
    {
        EventCenter.GetInstance().EventTrigger("鼠标开始拖拽", this);
    }

    /// <summary>
    /// 鼠标进入的回调
    /// </summary>
    /// <param name="baseEventData"></param>
    private void pointerEnterCallback(BaseEventData baseEventData)
    {
        EventCenter.GetInstance().EventTrigger("鼠标进入", this);

    }

    /// <summary>
    /// 鼠标移出的回调
    /// </summary>
    /// <param name="baseEventData"></param>
    private void pointerExitCallback(BaseEventData baseEventData)
    {
        EventCenter.GetInstance().EventTrigger("鼠标移出", this);


    }

    /// <summary>
    /// 装备的枚举类型
    /// </summary>
    public enum E_equip
    {
        notEquip,
        helmet,
        glove,
        shoe
    }

    public enum E_gridType
    {
        bag,
        equip
    }
}
