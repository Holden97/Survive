using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 解决List<T>无法使用UnityUtility序列化/反序列化的问题
/// 思路：unityutility可以解析包含了List<T>的对象，根据这一点封装List<T>
/// </summary>
/// <typeparam name="T">列表类型</typeparam>
[System.Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> info;
    public List<T> ToList() { return info; }
    public Serialization(List<T> target)
    {
        this.info = target;
    }
}

/// <summary>
/// 解决Dictionary<TKey, TValue>无法使用UnityUtility序列化/反序列化的问题
/// 思路：字典分解成两个list，之后与List<T>处理一致
/// </summary>
/// <typeparam name="TKey">字典key类型</typeparam>
/// <typeparam name="TValue">字典value类型</typeparam>
[System.Serializable]
public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    List<TKey> keys;
    [SerializeField]
    List<TValue> values;
    //被封装的字典
    Dictionary<TKey, TValue> info;
    public Dictionary<TKey, TValue> ToDictionary() { return info; }

    public Serialization(Dictionary<TKey, TValue> target)
    {
        this.info = target;
    }

    /// <summary>
    /// 序列化之前，将字典分解成两个list
    /// </summary>
    public void OnBeforeSerialize()
    {
        keys = new List<TKey>(info.Keys);
        values = new List<TValue>(info.Values);
    }

    /// <summary>
    /// 反序列化之后，将得到的两个List组装成Dictionary
    /// </summary>
    public void OnAfterDeserialize()
    {
        //保证之后的循环操作不会出错
        var count = Math.Min(keys.Count, values.Count);
        info = new Dictionary<TKey, TValue>(count);
        for (var i = 0; i < count; ++i)
        {
            info.Add(keys[i], values[i]);
        }

    }


    //// List<T> -> Json ( 例 : List<Enemy> )
    //string str = JsonUtility.ToJson(new Serialization<Enemy>(enemies)); // 输出 : {"target":[{"name":"怪物1","skills":["攻击"]},{"name":"怪物2","skills":["攻击","恢复"]}]}
    //// Json-> List<T>
    //List<Enemy> enemies = JsonUtility.FromJson<Serialization<Enemy>>(str).ToList();

    //// Dictionary<TKey,TValue> -> Json( 例 : Dictionary<int, Enemy> )
    //string str = JsonUtility.ToJson(new Serialization<int, Enemy>(enemies)); // 输出 : {"keys":[1000,2000],"values":[{"name":"怪物1","skills":["攻击"]},{"name":"怪物2","skills":["攻击","恢复"]}]}
    //// Json -> Dictionary<TKey,TValue>
    //Dictionary<int, Enemy> enemies = JsonUtility.FromJson<Serialization<int, Enemy>>(str).ToDictionary();

}
