using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 4.17
/// 用于生成与管理地图地形
/// 目前所有的地形介绍：
/// 0.空气墙：地图的最外层，有碰撞体，阻隔玩家穿出地图
/// 1.土：平地，无碰撞体，射击物和投掷物均可穿过
/// 2.水：形成河流，有碰撞体，射击物和投掷物均可穿过，投掷物在此落下后会熄灭
/// 3.石：平地阻碍物，有碰撞体，射击物不可穿过，投掷物可穿过，投掷物在此落下后会炸掉此地形
/// 4.树：平地阻碍物，有碰撞体，射击物不可穿过，投掷物不可穿过，投掷物不会摧毁地形，燃烧物可以摧毁该地形
/// </summary>
public class MapManager : MonoBehaviour
{
    public const int MapLength = 100;//指包括空气墙的长度
    public const int MapWidth = 100;//指包括空气墙的宽度
    public const int offsetx = 50;
    public const int offsety = 50;
    static MapManager instance;
    public GameObject[] Terrains;
    int[,] map = new int[MapLength, MapWidth];//其中100*100为人物可活动区域，外围是空气墙


    //河流参数
    //走向
    //0.水平
    //1.垂直
    int direction;
    //长度
    int length;
    //宽度
    int width;
    //A*寻路相关
    public static AStarMgr aStarMgr = AStarMgr.Instance;//A星算法寻路管理器
    GridType[,] mapGrids;

    public int[,] Map { get => map; }
    public static MapManager Instance { get => instance; }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        InitializeMap();
        mapGrids = FormGridsType(map);
        aStarMgr.InitMgr(MapLength, MapWidth, mapGrids);
    }

    private void InitializeMap()
    {
        //生成基本的土地形
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                map[i, j] = 1;
            }
        }

        //生成河流
        //决定河流数量
        int riverCount = UnityEngine.Random.Range(5, 15);
        //决定每一条河流的形态
        for (int i = 0; i < riverCount; i++)
        {
            //确定好参数
            direction = UnityEngine.Random.Range(0, 2);
            length = UnityEngine.Random.Range(3, 15);
            width = UnityEngine.Random.Range(3, 10);
            //产生河流
            GenerateRiver(direction, length, width);
        }

        //在土地上随机生成石子
        int rockCount = UnityEngine.Random.Range(15, 200);
        //在土地上随机生成树木
        int treeCount = UnityEngine.Random.Range(10, 20);

        GenerateRock(rockCount);
        GenerateTree(treeCount);

        //地图数组处理完毕，生成对应的object;
        for (int i = 0; i < MapWidth; i++)
        {
            //空气墙
            Instantiate(Terrains[0], new Vector3(-51, -50 + i, 0), transform.rotation, GameObject.Find("MapManager").transform);
            Instantiate(Terrains[0], new Vector3(50, -50 + i, 0), transform.rotation, GameObject.Find("MapManager").transform);
            Instantiate(Terrains[0], new Vector3(-50 + i, -51, 0), transform.rotation, GameObject.Find("MapManager").transform);
            Instantiate(Terrains[0], new Vector3(-50 + i, 50, 0), transform.rotation, GameObject.Find("MapManager").transform);
            //地图内地形（坐标-50-49）
            for (int j = 0; j < MapLength; j++)
            {
                //生成基本土地
                Instantiate(Terrains[1], new Vector3(-50 + j, -50 + i, -10), transform.rotation, GameObject.Find("MapManager").transform);
                //生成特定地形
                switch (map[i, j])
                {
                    case 0:
                    case 1:
                        continue;
                    case 2:
                    case 3:
                        Instantiate(Terrains[map[i, j]], new Vector3(-50 + j, -50 + i, 0), transform.rotation, GameObject.Find("MapManager").transform);
                        break;
                    case 4:
                        Instantiate(Terrains[map[i, j]], new Vector3(-50 + j, -50 + i, 0), transform.rotation, GameObject.Find("MapManager").transform);
                        break;
                }

            }
        }
        //空气墙的四个角落
        Instantiate(Terrains[0], new Vector3(-51, -51), transform.rotation, GameObject.Find("MapManager").transform);
        Instantiate(Terrains[0], new Vector3(50, -51), transform.rotation, GameObject.Find("MapManager").transform);
        Instantiate(Terrains[0], new Vector3(-51, -51, 0), transform.rotation, GameObject.Find("MapManager").transform);
        Instantiate(Terrains[0], new Vector3(-51, 50, 0), transform.rotation, GameObject.Find("MapManager").transform);

    }

    private void GenerateRock(int rockCount)
    {
        for (int i = 0; i < rockCount; i++)
        {
            int x = 0;
            int y = 0;
            do
            {
                x = UnityEngine.Random.Range(0, 100);
                y = UnityEngine.Random.Range(0, 100);
            } while ((map[x, y] == 2) || (map[x, y] == 4));
            map[x, y] = 3;
            //选择石子产生地
        }

    }

    private void GenerateTree(int treeCount)
    {
        for (int i = 0; i < treeCount; i++)
        {
            int x = 0;
            int y = 0;
            do
            {
                x = UnityEngine.Random.Range(0, 100);
                y = UnityEngine.Random.Range(0, 100);
            } while ((map[x, y] == 2) || (map[x, y] == 3));
            map[x, y] = 4;
            //选择树木产生地
        }

    }

    private void GenerateRiver(int direction, int length, int width)
    {
        Vector2 startPoint = new Vector2();
        //寻找一个合适的起点
        switch (direction)
        {
            case 0:
                startPoint.x = UnityEngine.Random.Range(1, map.GetLength(0) - 1 - length);
                startPoint.y = UnityEngine.Random.Range(1, map.GetLength(0) - 1 - width);
                break;
            case 1:
                startPoint.x = UnityEngine.Random.Range(1, map.GetLength(0) - 1 - width);
                startPoint.y = UnityEngine.Random.Range(1, map.GetLength(0) - 1 - length);
                break;
        }
        //生成河流主体部分
        switch (direction)
        {
            //水平
            case 0:
                for (int i = 0; i < length; i++)
                {
                    for (int j = UnityEngine.Random.Range(0, 2); j < UnityEngine.Random.Range(width - 1, width + 1); j++)
                    {
                        map[j + (int)startPoint.y, i + (int)startPoint.x] = 2;
                    }
                }
                break;
            //垂直
            case 1:
                for (int i = 0; i < length; i++)
                {
                    for (int j = UnityEngine.Random.Range(0, 2); j < UnityEngine.Random.Range(width - 1, width + 1); j++)
                    {
                        map[i + (int)startPoint.y, j + (int)startPoint.x] = 2;
                    }
                }
                break;
        }
    }

    public static GridType[,] FormGridsType(int[,] map)
    {
        GridType[,] gridTypes = new GridType[map.GetLength(0), map.GetLength(1)];
        for (int row = 0; row < map.GetLength(0); row++)
        {
            for (int col = 0; col < map.GetLength(1); col++)
            {
                switch (map[row, col])
                {
                    case 1:
                        gridTypes[row, col] = GridType.clear;
                        break;
                    case 2:
                    case 3:
                    case 4:
                        gridTypes[row, col] = GridType.blocked;
                        break;
                }
            }
        }
        return gridTypes;

    }

    public static List<AStarGrid> SearchPath(Vector2 startPoint, Vector2 endPoint)
    {
        //起点偏移-0.5f是因为实际碰撞体的位置还要离中心偏移0.5f
        List<AStarGrid> originalPath = aStarMgr.SearchPath(new Vector2(startPoint.x + offsetx, startPoint.y - 0.5f + offsety), new Vector2(endPoint.x + offsetx, endPoint.y - 0.5f + offsety));
        return originalPath;
    }

    private void Update()
    {
        //更新敌人的路径
        //根据玩家位置信息更新A*寻路
        if (new Vector2(Mathf.Round(GameManager.player.transform.position.x), Mathf.Round(GameManager.player.transform.position.y)) != GameManager.playerPosition)
        {

            foreach (GameObject enemy in GameManager.enemies)
            {
                if (enemy.tag == "EarthEnemy")
                {
                    enemy.GetComponent<Bug>().path = SearchPath(enemy.transform.position, new Vector2(GameManager.player.transform.position.x, GameManager.player.transform.position.y));
                }
                if (enemy.tag == "WoodEnemy")
                {
                    enemy.GetComponent<Hound>().path = SearchPath(enemy.transform.position, new Vector2(GameManager.player.transform.position.x, GameManager.player.transform.position.y));
                }
                if (enemy.tag == "GoldEnemy")
                {
                    enemy.GetComponent<BugBuilder>().path = SearchPath(enemy.transform.position, new Vector2(GameManager.player.transform.position.x, GameManager.player.transform.position.y));

                }
                if (enemy.tag == "FireEnemy")
                {
                    enemy.GetComponent<BugGunner>().path = SearchPath(enemy.transform.position, new Vector2(GameManager.player.transform.position.x, GameManager.player.transform.position.y));

                }
            }
            GameManager.playerPosition = new Vector2(Mathf.Round(GameManager.player.transform.position.x), Mathf.Round(GameManager.player.transform.position.y));
        }
    }
}
