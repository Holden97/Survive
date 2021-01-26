using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static ItemCell;
/// <summary>
/// 游戏数据管理类
/// 1.存放游戏数据
/// 2.存放静态路径
/// </summary>
public class GameDataMgr : BaseManager<GameDataMgr>
{
    /// <summary>
    /// 存储物品信息的字典，便于按索引查找
    /// </summary>
    Dictionary<int, Item> dic = new Dictionary<int, Item>();
    /// <summary>
    /// 存储商店物品信息的字典，便于按索引查找
    /// </summary>
    Dictionary<int, Item> shopDic = new Dictionary<int, Item>();
    /// <summary>
    /// 玩家信息的存储路径
    /// </summary>
    public static string playerInfo_url = Application.persistentDataPath + "playerInfo";
    /// <summary>
    /// 物品icon信息的存储前缀
    /// </summary>
    public static string icon_url_pre = "UI/Item/";
    /// <summary>
    /// ShopItem预设体的资源路径
    /// </summary>
    public static string shopItem_url = "UI/Cell/shopCell";
    /// <summary>
    /// ShopItem货币图案资源路径前缀
    /// </summary>
    public static string shopItem_moneyType_url_pre = "UI/moneyType/";
    /// <summary>
    /// 玩家信息
    /// </summary>



    /// <summary>
    /// 初始化游戏数据信息
    /// 2020.7.11
    /// 这块代码有待改进，违反开闭原则
    /// </summary>
    public void Init()
    {
        //物品文件
        //这里因为路径问题不能使用FileToObject接口，"JSON/ItemsInfo"无法指向正确文件路径
        //Items items = FileToObject<Items>("JSON/ItemsInfo");
        TextAsset textAsset = ResMgr.GetInstance().Load<TextAsset>("JSON/ItemsInfo");
        //物品信息列表
        Items items = JsonUtility.FromJson<Items>(textAsset.text);
        //Debug.Log(playerInfo_url); 
        //C:/Users/Smallball/AppData/LocalLow/DefaultCompany/MyBagGameplayerInfo

        foreach (var item in items.info)
        {
            dic.Add(item.id, item);
            if (item.moneyType != "notforsale")
                shopDic.Add(item.id, item);
        }

        //初始化玩家信息
        if (File.Exists(playerInfo_url))
        {
            FileToObject(playerInfo_url, out Player.player);
        }
        else
        {
            //创建初始玩家
            Player.player = new Player()
            {
                name = "锅兄",
                lev = 1,
            };
            Player.player.prop.Add("gold", new Currency(1, 1000));
            Player.player.prop.Add("dia", new Currency(2, 1000));
            Player.player.prop.Add("mine", new Currency(3, 200));
            //道具
            Player.player.bag.Add("props", new List<BagItem>());
            //装备
            Player.player.bag.Add("equip", new List<BagItem>());
            //宝藏
            Player.player.bag.Add("trea", new List<BagItem>());
            Player.player.AddItem(1, 2);
            Player.player.AddItem(2, 2);
            Player.player.AddItem(3, 2);
            Player.player.AddItem(4, 2);
            Player.player.AddItem(5, 2);
            Player.player.AddItem(6, 2);

            //初始化装备栏类型(包含了非装备类型)
            InitEquip(out Player.player.equip);
            //删除初始化时非装备一栏的字典信息
            Player.player.equip.Remove(E_equip.notEquip);

            //添加装备
            AddEquip(1);
            AddEquip(1);
            AddEquip(7);
            //存入文件
            ObjectToFile(Player.player, playerInfo_url);
        }

    }
    /// <summary>
    /// 初始化装备
    /// </summary>
    /// <typeparam name="T">装备使用的枚举类型</typeparam>
    /// <param name="equip">装备字典</param>
    private static void InitEquip<T>(out Dictionary<T, BagItem> equip)
    {
        equip = new Dictionary<T, BagItem>();
        foreach (T type in Enum.GetValues(typeof(T)))
        {
            equip.Add(type, null);
        }
    }

    /// <summary>
    /// 添加装备
    /// </summary>
    /// <param name="id">装备id</param>
    /// <returns>添加操作是否成功</returns>
    public bool AddEquip(int id)
    {
        Item o = GetInstance().GetItem(id);
        //equipType为0表示非装备，或者背包中数量不足亦操作失败
        if (o.equipType == 0 || !Player.player.RemoveItem(id, 1))
            return false;
        else
        {
            //装备前装备栏为空，直接放上去，背包中的物品数量-1
            if (Player.player.equip[(E_equip)o.equipType] == null)
            {
                Player.player.equip[(E_equip)o.equipType] = new BagItem(id, 1);
            }
            //装备前装备栏不为空，新物品背包数量-1，旧物品背包数量+1
            else
            {
                BagItem lastEquip = Player.player.equip[(E_equip)o.equipType];
                Player.player.AddItem(lastEquip.id, 1);
                Player.player.equip[(E_equip)o.equipType] = new BagItem(id, 1);
            }
        }
        return true;
    }
    /// <summary>
    /// 添加装备到特定部位
    /// </summary>
    /// <param name="e_Equip">装备部位</param>
    /// <param name="id">装备id</param>
    /// <returns>添加操作是否成功</returns>
    public bool AddEquip(E_equip e_Equip, int id)
    {
        Item o = GetInstance().GetItem(id);
        //equipType类型为非装备(==0)，或者背包中数量不足(不足-1)，或者装备类型与指定部位类型不同，均操作失败
        if (o.equipType == 0 || !Player.player.RemoveItem(id, 1) || (int)e_Equip != o.equipType)
            return false;
        else
        {
            //装备前装备栏为空，直接放上去
            if (Player.player.equip[(E_equip)o.equipType] == null)
            {
                Player.player.equip[(E_equip)o.equipType] = new BagItem(id, 1);
            }
            //装备前装备栏不为空，旧物品背包数量+1
            else
            {
                BagItem lastEquip = Player.player.equip[(E_equip)o.equipType];
                Player.player.AddItem(lastEquip.id, 1);
            }
            return true;

        }
    }
    /// <summary>
    /// 移除装备
    /// </summary>
    /// <param name="e_Equip">装备部位</param>
    /// <returns></returns>
    public bool RemoveEquip(E_equip e_Equip)
    {
        //装备栏为空，返回false
        if (Player.player.equip[e_Equip] == null)
        {
            return false;
        }
        //装备前装备栏不为空，旧物品背包数量+1
        else
        {
            BagItem lastEquip = Player.player.equip[e_Equip];
            Player.player.AddItem(lastEquip.id, 1);
            Player.player.equip[e_Equip] = null;
        }
        return true;
    }



    /// <summary>
    /// 从文件读取可序列化对象到指定对象上
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="path">文件路径</param>
    /// <param name="o">对象</param>
    public static void FileToObject<T>(string path, out T o) where T : class
    {
        //直接初始化
        byte[] bytes = File.ReadAllBytes(path);
        //字节数组转JSON
        string json = Encoding.UTF8.GetString(bytes);
        //JSON转Player类
        o = JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    /// 从文件读取可序列化对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="path">文件路径</param>
    public static T FileToObject<T>(string path)
    {
        //直接初始化
        byte[] bytes = File.ReadAllBytes(path);
        //字节数组转JSON
        string json = Encoding.UTF8.GetString(bytes);
        //JSON转Player类
        return JsonUtility.FromJson<T>(json);
    }


    /// <summary>
    ///  将可序列化对象存入文件
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="o">对象</param>
    /// <param name="path">文件路径</param>
    public static void ObjectToFile<T>(T o, string path)
    {
        //将玩家信息转化成JSON格式
        string json = JsonConvert.SerializeObject(o, Formatting.Indented);
        //Debug输出信息
        Debug.Log(json);
        //JSON转字节数组
        File.WriteAllBytes(path, Encoding.UTF8.GetBytes(json));
    }

    /// <summary>
    /// 获取对应id的物品信息
    /// </summary>
    /// <param name="id">对应id</param>
    /// <returns></returns>
    public Item GetItem(int id)
    {
        if (dic.ContainsKey(id))
            return dic[id];
        return null;
    }


    /// <summary>
    /// 返回所有商店物品信息
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, Item> GetShopItems()
    {
        return shopDic;
    }
}
/// <summary>
/// 存放物品信息类，用于接收JSON信息
/// </summary>
public class Items
{
    public List<Item> info;
}

/// <summary>
/// 物品信息类
/// </summary>
[System.Serializable]
public class Item
{
    //物品id
    public int id;
    //物品名称
    public string name;
    //物品图标
    public int icon;
    //物品类型（对应背包中的不同分类）
    public string type;
    //物品价格
    public int price;
    //物品描述
    public string tips;
    //物品购买所需货币名称
    public string moneyType;
    //单个最大物品存储数
    public int maxUnitStorage;
    //物品的装备类型(对应物品在装备界面不同的穿戴位置)
    public int equipType;
}


/// <summary>
/// 物品背包信息类
/// </summary>
[System.Serializable]
public class BagItem
{
    //物品id
    public int id;
    //物品数量
    public int num;
    //物品最大堆叠数
    public int maxUnitStorage;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="id">物品id</param>
    /// <param name="num">物品数量</param>
    public BagItem(int id, int num)
    {
        this.id = id;
        this.num = num;
        Item o = GameDataMgr.GetInstance().GetItem(id);
        maxUnitStorage = o.maxUnitStorage;
    }
}

