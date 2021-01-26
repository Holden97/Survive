using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 缓存池模块
/// 使用一个Dic存放所有数据
/// 对外提供存取接口
/// 自定义类PoolData用于梳理对象池中的对象层次
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    //存放的list
    Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    //在场景中缓存池的根节点
    private GameObject poolObj;
    /// <summary>
    /// 存方法
    /// </summary>
    /// <param name="name">物品名称</param>
    /// <param name="res">当前存的物品</param>
    public void Push(string name, GameObject res)
    {
        res.SetActive(false);
        //明确父子关系
        if (poolObj == null)
            poolObj = new GameObject("Pool");
        if (!poolDic.ContainsKey(name))
        {
            poolDic.Add(name, new PoolData(poolObj, res));
        }
        else
        {
            poolDic[name].Push(res);
        }
    }

    /// <summary>
    /// 取方法
    /// </summary>
    /// <param name="path">所取物品名称（在Resources文件夹下的路径）</param>
    /// <returns></returns>
    public void Pop(string path, UnityAction<GameObject> callBack)
    {
        if (poolDic.ContainsKey(path) && poolDic[path].objList.Count > 0)
        {
            callBack(poolDic[path].Pop());
        }
        else
        {
            ResMgr.GetInstance().LoadAsyn<GameObject>(path, (o) => { o.name = path; callBack(o); });

        }

    }

    /// <summary>
    /// 清空缓存池引用，置空场景缓存池对象
    /// 主要用于场景切换时
    /// </summary>
    public void Clear()
    {
        poolDic.Clear();
        poolObj = null;
    }


}
/// <summary>
/// 对象池的数据类
/// objName 预设对象名
/// objList 对象列表
/// 提供分装的存取方法
/// </summary>
public class PoolData
{
    public List<GameObject> objList;
    GameObject PoolNode;//在hierarchy面板中对应的一类物体的父节点
    public GameObject Pop()
    {
        //从列表中取出
        GameObject res = objList[0];
        objList.RemoveAt(0);
        //激活
        res.SetActive(true);
        //从场景中的缓存池中取出，脱离与父节点的父子关系
        res.transform.SetParent(null);
        return res;

    }

    public void Push(GameObject res)
    {
        objList.Add(res);
        res.transform.SetParent(PoolNode.transform);
    }

    public PoolData(GameObject poolObj, GameObject res)
    {
        objList = new List<GameObject>();
        Push(res);
        PoolNode = new GameObject(res.name);
        PoolNode.transform.SetParent(poolObj.transform);
    }


}
