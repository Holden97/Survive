using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 提示面板
/// 1.初始化提示面板中的信息
/// </summary>
public class ItemPanel : BasePanel
{
    private Item itemInfo;
    /// <summary>
    /// 初始化信息方法
    /// </summary>
    /// <param name="info">传入的简略信息</param>
    public void InitInfo(BagItem info)
    {
        itemInfo = GameDataMgr.GetInstance().GetItem(info.id);

        FindComponent<Text>("ItemName").text = itemInfo.name;
        FindComponent<Text>("ItemNum").text = info.num.ToString();
        FindComponent<Text>("ItemDes").text = itemInfo.tips;
        FindComponent<Image>("TipIcon").sprite = ResMgr.GetInstance().Load<Sprite>(GameDataMgr.icon_url_pre + info.id);
    }

}
