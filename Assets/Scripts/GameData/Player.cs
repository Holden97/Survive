using System.Collections.Generic;
using static ItemCell;
/// <summary>
/// 玩家信息类，负责对外提供修改Player类对象数据的接口
/// </summary>
public class Player
{
    //成员数据
    //玩家姓名
    public string name;
    //玩家等级
    public int lev;
    //玩家拥有货币
    public Dictionary<string, Currency> prop = new Dictionary<string, Currency>();
    //玩家背包
    public Dictionary<string, List<BagItem>> bag = new Dictionary<string, List<BagItem>>();

    /// <summary>
    ///玩家当前身上的装备
    ///<装备类型，装备物品信息>
    ///0:非装备类型
    ///1：头部装备
    ///2：手部装备
    ///3：脚部装备
    /// </summary>
    public Dictionary<E_equip, BagItem> equip;


    //此静态变量日后应删除，权当测试使用
    public static Player player;

    //成员方法
    /// <summary>
    /// 购买并修改当前Player中的货币以及背包中的数据
    /// </summary>
    /// <param name="o">购买物品</param>
    /// <returns></returns>
    public bool BuyItem(Item o)
    {
        foreach (var item in prop)
        {
            if (item.Key == o.moneyType && ModifyCurrency(-o.price, item.Value))
            {
                Player.player.AddItem(o.id, 1);
                return true;
            }
        }
        return false;

    }



    /// <summary>
    /// 修改货币值，并存入文件
    /// </summary>
    /// <param name="m">修改量，正数为加，负数为减</param>
    /// <param name="currency">货币对象</param>
    /// <returns>返回此次购买是否成功</returns>
    public bool ModifyCurrency(int m, Currency currency)
    {
        bool result = currency.ModifyCurrency(m);
        GameDataMgr.ObjectToFile(this, GameDataMgr.playerInfo_url);
        return result;
    }


    /// <summary>
    /// 向背包添加物品
    /// 7.16
    /// 时间复杂度为n平方，应修改
    /// </summary>
    /// <param name="o">物品</param>
    /// <param name="m">数量，增加背包物品数量</param>
    /// <returns></returns>
    public void AddItem(int id, int m)
    {
        List<BagItem> curList = null;
        Item o = GameDataMgr.GetInstance().GetItem(id);
        //遍历已有的单元格，选好curList，优先将物品填充到不满最大存储数的单元格
        foreach (var tag in bag)
        {
            if (tag.Key == o.type)
            {
                curList = tag.Value;
                foreach (var bagItem in tag.Value)
                {
                    if (bagItem.id == o.id && bagItem.num < bagItem.maxUnitStorage)
                    {

                        //当前单元格可存储量
                        int curStorage = bagItem.maxUnitStorage - bagItem.num > m ? m : bagItem.maxUnitStorage - bagItem.num;
                        //剩余需存储量
                        m -= curStorage;
                        //当前单元格增加存储量
                        bagItem.num += curStorage;

                        if (m == 0)
                            break;
                    }
                }
            }

        }
        //遍历完成后若m仍有剩余，则开辟新空间直至存储完毕
        while (m > 0)
        {
            int curStorage = m > o.maxUnitStorage ? o.maxUnitStorage : m;
            m -= curStorage;
            //对应List<BagItem>添加新项
            curList?.Add(new BagItem(o.id, curStorage));
        }

    }
    /// <summary>
    /// 移除背包物品
    /// </summary>
    /// <param name="id">物品id</param>
    /// <param name="m">移除数量</param>
    public bool RemoveItem(int id, int m)
    {
        List<BagItem> curList = null;
        //物品已拥有数量
        int itemNum = 0;
        Item o = GameDataMgr.GetInstance().GetItem(id);
        //遍历已有的单元格
        foreach (var tag in bag)
        {
            if (tag.Key == o.type)
            {
                //选好物品类型对应的背包物品列表
                curList = tag.Value;

                //如果列表中的数量大于等于m，则操作成功，否则为失败

                //遍历取得物品数量
                foreach (var bagItem in tag.Value)
                {
                    if (bagItem.id == o.id)
                        itemNum += bagItem.num;
                }
                if (itemNum < m)
                    return false;
                else
                {
                    for (int i = 0; i < tag.Value.Count; i++)
                    {
                        BagItem bagItem = tag.Value[i];
                        if (bagItem.id == o.id)
                        {
                            if (bagItem.num <= m)
                            {
                                m -= bagItem.num;
                                //这个操作在使用foreach时无法实现，可用于比较二者差异
                                tag.Value.RemoveAt(i);
                            }
                            //一格的物品数量大于剩余需要减去的数量
                            else
                            {
                                bagItem.num -= m;
                                m = 0;
                            }
                        }
                    }
                    return true;
                }

            }
        }
        return false;
    }
}
