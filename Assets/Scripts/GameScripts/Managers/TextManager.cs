using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 4.16
/// 用于管理与规范提示信息
/// 目前写在这个文件中的事件有
/// 1.捡起了XX弹药
/// 2.XX武器没有弹药了
/// </summary>
public class TextManager : MonoBehaviour
{
    public static  string BagKey(int index)
    {
        string result="";
        switch (index)
        {
            case 0:
                result = "手枪弹药";
                break;
            case 1:
                result = "步枪弹药";
                break;
            case 2:
                result = "霰弹枪弹药";
                break;
            default:
                break;
        }
        return result;
    }

    public static string PickUpBag(int index)
    {
        return "你捡起了" + BagKey(index) + "！";
    }

    public static string noAmmo(int index)
    {
        return "没有" + BagKey(index) + "了！";
    }
}
