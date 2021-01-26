using UnityEngine;
/// <summary>
/// 货币类
/// 提供对
/// 1.货币id和quantity的初始化接口
/// 2.货币quantity的修改接口
/// 3.购买物品处理接口
/// </summary>
[System.Serializable]
public class Currency
{
    //货币种类id
    public int id;
    //货币数量
    public int quantity;

    public Currency(int id, int quantity)
    {
        this.id = id;
        this.quantity = quantity;
    }

    /// <summary>
    /// 购买物品
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    virtual public bool BuyItem(Item o)
    {
        return false;
    }

    /// <summary>
    /// 修改货币数量
    /// </summary>
    /// <param name="m">修改量，正数为加，负数为减</param>
    /// <returns></returns>
    virtual public bool ModifyCurrency(int m)
    {
        if (quantity + m >= 0)
        {
            quantity += m;
            return true;
        }
        else
            return false;
    }
}


