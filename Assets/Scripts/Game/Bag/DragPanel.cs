using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragPanel : BasePanel
{
    public void Init()
    {
    }

    internal void Init(Item itemInfo)
    {
        FindComponent<Image>("Icon").sprite = ResMgr.GetInstance().Load<Sprite>("UI/Item/" + itemInfo.icon);
    }
}
