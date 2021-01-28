using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameDataMgr.GetInstance().Init();
        ItemCellMgr.GetInstance().Init();
        //UIMgr.GetInstance().ShowPanel<MainPanel>("MainPanel", PanelLayer.bot); 
    }

}
