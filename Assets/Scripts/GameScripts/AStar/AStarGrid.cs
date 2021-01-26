using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A*算法依据f=g+h;来计算损耗
/// f是总损耗
/// g是距离起点的损耗
/// h是距离终点的损耗
/// 
/// parent记录父节点
/// 
/// x与y记录其在网格中的位置
/// x是行
/// y是列
/// 
/// type记录该网格的类型
/// clear表示通畅
/// blocked表示堵塞
/// </summary>

public enum GridType
{
    clear,
    blocked
}
public class AStarGrid : IComparable
{
    //属性
    //公式用值
    float f;
    float g;
    float h;
    //父节点
    AStarGrid parent;
    //位置信息
    int row;//行
    int col;//列
    //类型
    GridType type;
    //是否在开/关列表中的标记
    int openMark;
    int closeMark;

    //方法
    //初始化方法
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row">行</param>
    /// <param name="col">列</param>
    /// <param name="gType">网格通堵信息</param>
    public AStarGrid(int row, int col, GridType gType)
    {
        this.row = row;
        this.col = col;
        this.type = gType;
        openMark = -1;
        closeMark = -1;
    }

    public GridType Type { get => type; }
    public float F { get => f; set => f = value; }
    public float G { get => g; set => g = value; }
    public float H { get => h; set => h = value; }
    public AStarGrid Parent { get => parent; set => parent = value; }
    public int Row { get => row; set => row = value; }
    public int Col { get => col; set => col = value; }
    public int OpenMark { get => openMark; set => openMark = value; }
    public int CloseMark { get => closeMark; set => closeMark = value; }

    public int CompareTo(object aStarGrid)
    {
        AStarGrid aStar;
        if (aStarGrid is AStarGrid)
        {
            aStar = aStarGrid as AStarGrid;
            return f.CompareTo(aStar.f);
        }
        else
        {
            throw new ArgumentException("Object is not a AStarGrid Object");
        }

    }
}
