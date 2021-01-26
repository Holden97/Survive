using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItem : BasePanel
{
    Item curInfo;

    public void Init(Item shopItem)
    {
        curInfo = shopItem;
        FindComponent<Image>("itemIcon").sprite = ResMgr.GetInstance().Load<Sprite>(GameDataMgr.icon_url_pre + shopItem.icon);
        FindComponent<Text>("itemDes").text = curInfo.tips;
        FindComponent<Text>("itemName").text = curInfo.name;
        FindComponent<Text>("itemPrice").text = curInfo.price.ToString();
        FindComponent<Image>("buyType").sprite = ResMgr.GetInstance().Load<Sprite>(GameDataMgr.shopItem_moneyType_url_pre + shopItem.moneyType);
        FindComponent<Button>("btnBuy").onClick.AddListener(() => EventCenter.GetInstance().EventTrigger("购买物品", curInfo));


    }
}
