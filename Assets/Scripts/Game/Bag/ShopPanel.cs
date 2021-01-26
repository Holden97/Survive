using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : BasePanel
{
    Dictionary<int, Item> shopDic;
    public GameObject content;
    // Start is called before the first frame update
    void Start()
    {
        FindComponent<Button>("btnClose").onClick.AddListener(() => { UIMgr.GetInstance().HidePanel("shopPanel"); });
    }

    public override void ShowMe()
    {
        base.ShowMe();
        shopDic = GameDataMgr.GetInstance().GetShopItems();
        foreach (var item in shopDic)
        {
            ResMgr.GetInstance().LoadAsyn<GameObject>(GameDataMgr.shopItem_url, (o) =>
             {
                 o.transform.SetParent(content.transform);
                 o.GetComponent<ShopItem>().Init(item.Value);
             });
        }

    }


}
