using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 4.18
/// A星算法的管理类，使用单例模式
/// 包含字段
/// openList<AStar>开放列表
/// closeList<AStar>关闭列表
/// 整张地图的尺寸
/// mapL地图长度
/// mapW地图宽度
/// 以及
/// 二维网格数组管理所有的AStar网格
/// AStarGrid[,] map;
/// 
/// 包含两种方法
/// 初始化管理类
/// InitMgr()
/// 对外部提供的接口
/// SearchPath()
/// 
/// </summary>
public class AStarMgr
{
    //单例模式，之后优化成自定义基类
    private static AStarMgr instance;
    public static AStarMgr Instance
    {
        get
        {
            if (instance == null)
                instance = new AStarMgr();
            return instance;
        }
    }

    BinaryHeap openList;
    List<AStarGrid> closeList;

    //寻路次数标记
    public static int findIndex;

    int mapL;
    int mapW;
    public static AStarGrid[,] map;

    //方法
    //初始化管理类
    //根据宽高，创建格子
    /// <summary>
    /// ----------------mapLength---------------------
    /// |
    /// |
    /// |
    /// |
    /// mapWidth
    /// |
    /// |
    /// |
    /// |
    /// |
    /// |
    /// 
    /// </summary>
    /// <param name="mapLength"></param>
    /// <param name="mapWidth"></param>
    /// <param name="mapGrid">已经转换好的通-堵数组</param>
    public void InitMgr(int mapLength, int mapWidth, GridType[,] mapGrid)
    {
        instance = this;
        mapL = mapLength;
        mapW = mapWidth;
        map = new AStarGrid[mapL, mapW];
        //将通堵信息录入
        for (int row = 0; row < mapGrid.GetLength(0); row++)
        {
            for (int col = 0; col < mapGrid.GetLength(1); col++)
            {
                map[row, col] = new AStarGrid(row, col, mapGrid[row, col]);
            }
        }
        findIndex = 0;

    }
    //提供给外部的寻路方法
    //4.21 从递归改成循环
    //递归书写简单，代码逻辑书面上更清晰，但有额外的栈开销
    //循环反之
    /// <summary>
    /// 外部调用时，应自行实现坐标轴到以(0,0)为起始点的自然数二维数组的转换
    /// 这里的startPoint与endPoint已经是整数形式，内部使用(int)强转类型
    /// </summary>
    /// <param name="startPoint">以(0,0)为起始点的自然数二维数组的二维坐标下的起始点，point.x对应横坐标，point.y对应纵坐标</param>
    /// <param name="endPoint">以(0,0)为起始点的自然数二维数组的二维坐标下的终点，point.x对应横坐标，point.y对应纵坐标</param>
    /// <returns></returns>
    public List<AStarGrid> SearchPath(Vector2 startPoint, Vector2 endPoint)
    {
        //检查合法性
        //1.不越界
        //2.不阻塞
        //根据公式寻路
        //返回结果


        //具体过程
        //检查合法性
        AStarGrid startGrid = map[Mathf.RoundToInt(startPoint.y), Mathf.RoundToInt(startPoint.x)];
        AStarGrid endGrid = map[Mathf.RoundToInt(endPoint.y), Mathf.RoundToInt(endPoint.x)];
        if (Mathf.RoundToInt(startPoint.x) < 0 || Mathf.RoundToInt(startPoint.x) > mapL || Mathf.RoundToInt(startPoint.y) < 0 || Mathf.RoundToInt(startPoint.y) > mapW || Mathf.RoundToInt(endPoint.x) < 0 || Mathf.RoundToInt(endPoint.x) > mapL || Mathf.RoundToInt(endPoint.y) < 0 || Mathf.RoundToInt(endPoint.y) > mapW || startGrid.Type == GridType.blocked || endGrid.Type == GridType.blocked)
        {
            Debug.Log("寻路起点或终点越界！");
            IncreaseMark();
            return null;

        }
        else
        {
            //对路径上的当前点CheckSurroundNodes()------
            //1.选择该点周围的不在开放/关闭列表中的通点作为新的合法点并放入开放列表，计算出每个新点的f值，并取最小的一个放入关闭列表
            List<AStarGrid> result = new List<AStarGrid>();
            //当前寻路的“中心点”
            AStarGrid curCenter = startGrid;

            //并未初始化H，因为起点一定会被放进closeList，且H值后面的节点不需要使用
            //4.21 需初始化H（即清空），因为寻路方法可能被多次调用，此次寻路的起点在上次寻路中可能已经存了不为0的H值，对寻路结果无影响，但会妨碍检查BUG
            startGrid.F = 0;
            startGrid.G = 0;
            startGrid.H = 0;
            startGrid.Parent = null;
            //清空开启与关闭列表
            openList = new BinaryHeap();
            closeList = new List<AStarGrid>();
            //openList.Clear();
            //closeList.Clear();
            //关闭列表加入起始点
            startGrid.CloseMark = findIndex;
            closeList.Add(startGrid);

            while (true)
            {
                //检查周围的四个点是否合法
                //计算合法的，且不在开放/关闭列表中的点的f值
                //排序开放列表中f值最小的结点，取出并放到关闭列表中
                //判断思路的条件不是当前点的周围4个点都不合法
                //而是openlist中无结点
                CheckValid(curCenter.Col - 1, curCenter.Row, new Vector2(curCenter.Col, curCenter.Row), endPoint);
                CheckValid(curCenter.Col, curCenter.Row - 1, new Vector2(curCenter.Col, curCenter.Row), endPoint);
                CheckValid(curCenter.Col + 1, curCenter.Row, new Vector2(curCenter.Col, curCenter.Row), endPoint);
                CheckValid(curCenter.Col, curCenter.Row + 1, new Vector2(curCenter.Col, curCenter.Row), endPoint);
                //排序，死路返回空
                if (openList.Heap.Count != 0)
                {
                    AStarGrid item = openList.Pop();
                    item.OpenMark = -1;
                    item.CloseMark = findIndex;
                    closeList.Add(item);
                }
                else
                {
                    IncreaseMark();
                    Debug.Log("该物体遇到了死路！");
                    return null;
                }

                //找到终点
                if (closeList[closeList.Count - 1] == endGrid)
                {
                    AStarGrid curGrid = closeList[closeList.Count - 1];
                    while (curGrid.Parent != null)
                    {
                        result.Add(curGrid);
                        curGrid = curGrid.Parent;
                    }
                    result.Reverse();
                    //更新寻路次数标记
                    IncreaseMark();
                    return result;
                }
                else
                {
                    curCenter = closeList[closeList.Count - 1];
                }
            }

        }

    }

    private static void IncreaseMark()
    {
        if (findIndex == int.MaxValue)
        {
            findIndex = 0;
        }
        else
        {
            findIndex++;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="v1">横坐标</param>
    /// <param name="v2">纵坐标</param>
    /// <param name="centerPoint"></param>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    private bool CheckValid(int v1, int v2, Vector2 centerPoint, Vector2 endPoint)
    {
        if (v1 >= 0 && v1 < mapL && v2 >= 0 && v2 < mapW)
        {
            AStarGrid aStarGrid = map[(int)v2, (int)v1];
            if (aStarGrid.Type == GridType.clear && aStarGrid.OpenMark != findIndex && aStarGrid.CloseMark != findIndex)
            {
                //写入parent值
                //计算对应的f值
                AStarGrid curGrid = map[(int)v2, (int)v1];
                curGrid.Parent = map[(int)centerPoint.y, (int)centerPoint.x];
                //F=G+H
                //G是离起点的距离
                //H是离终点的距离
                //G=父节点的G+自身距离父节点的距离
                //H=离终点的曼哈顿距离
                curGrid.G = curGrid.Parent.G + Mathf.Sqrt(Mathf.Pow(curGrid.Row - curGrid.Parent.Row, 2) + Mathf.Pow(curGrid.Col - curGrid.Parent.Col, 2));
                curGrid.H = Mathf.Abs(endPoint.y - curGrid.Row) + Mathf.Abs(endPoint.x - curGrid.Col);
                curGrid.F = curGrid.G + curGrid.H;
                curGrid.OpenMark = findIndex;
                openList.Push(curGrid);
                return true;
            }
        }
        return false;

    }
}
