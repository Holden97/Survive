using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{


    /// <summary>
    /// 更新UI显示（旧）
    /// 关卡管理
    /// 地图信息转换
    /// </summary>
    /// 


    public Texture texture;//准心图案
    public GameObject ammoBagPrefab;
    public Text curHealth;
    public Text curLevel;
    public Text curWeaponAmmo;
    public Text coinCount;
    public static GameObject player;
    public static Vector2 playerPosition;
    public static List<GameObject> enemies;
    private int level;
    GameObject ammoPoint;
    //public GameObject mapPanel;
    bool curLevelGenerationFinish;//当前关卡自动出怪全部结束
    int curEnemyOutput;
    public int coin;//收集到的金币数
    //游戏计时，积分和击杀
    public float surviveTime;
    public float surviveStart;
    public int totalScore;
    public int takedowns;
    public Text surviveTimeText;
    public Text totalScoreText;
    public Text takedownsText;
    //每一关的基础出怪数
    static int basicAddtionalBugOutput = 10;
    int BugOutput = basicAddtionalBugOutput;
    int BugHasOutput = 0;
    static GameManager instance;
    public GameObject[] enemyPrefabs;
    bool pauseIsOpen;
    public bool curGameIsOver;
    public GameObject PausePanel;
    public GameObject SummaryPanel;
    //计时器
    float bugTimer;
    float gunnerTimer;
    float bugBuilderTimer;
    public float ammoBagTimer;
    //各个怪物出现间隔
    float bugInterval = 4f;
    float gunnerInterval = 10f;
    float bugBuilderInterval = 60f;
    float ammoBagInterval = 10f;



    public int Level { get => level; }
    public static GameManager Instance { get => instance; set => instance = value; }
    public bool PauseIsOpen { get => pauseIsOpen; }


    public enum enemy
    {
        bug,
        gunner,
        builder,
        hound
    }

    /// <summary>    
    /// /// 绘制准心    
    /// /// </summary>    
    /// 
    void OnGUI()
    {
        //绘制准心
        Cursor.visible = false;//隐藏鼠标
        Rect rect = new Rect(Input.mousePosition.x - 30,
        Screen.height - Input.mousePosition.y - 30,
        60, 60);
        GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);

    }

    // Start is called before the first frame update
    void Awake()
    {
        ammoPoint = GameObject.FindGameObjectWithTag("AmmoBagPoint");
        instance = this;
        Instance.takedowns = 0;
        surviveStart = Time.time;
        level = 0;
        coin = 0;
        player = GameObject.FindGameObjectWithTag("Player");
        playerPosition = new Vector2(Mathf.Round(player.transform.position.x), Mathf.Round(player.transform.position.y));
        totalScore = 0;
        bugTimer = Time.time;
        curGameIsOver = false;
        SummaryPanel.SetActive(false);
        curLevelGenerationFinish = true;
        enemies = new List<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        if (curGameIsOver)
        {
            //mapPanel.SetActive(false);
            SummaryPanel.SetActive(true);
            return;
        }

        //更新UI
        UpdateUI();
        //关卡检测
        //更新关卡数
        CheckLevelUp();
        //生成弹药包
        GenerateAmmo();
        //按键检测
        KeyInputListener();
    }

    private void GenerateAmmo()
    {
        bool full = true;
        if (Time.time > ammoBagTimer + ammoBagInterval)
        {
            int index = 0;
            foreach (Transform item in ammoPoint.transform)
            {
                if (item.transform.childCount == 0)
                {
                    full = false;
                    break;
                }

            }
            if (full)
                return;
            do
            {
                index = UnityEngine.Random.Range(0, ammoPoint.transform.childCount);
            } while (ammoPoint.transform.GetChild(index).childCount != 0);
            Instantiate(ammoBagPrefab, ammoPoint.transform.GetChild(index)).transform.SetParent(ammoPoint.transform.GetChild(index));
            ammoBagTimer = Time.time;
        }
    }

    void CheckLevelUp()
    {
        if (GameObject.Find("EnemyManager").transform.childCount == 0 && instance.curLevelGenerationFinish)
        {
            level++;
            curLevel.text = Level.ToString();
            BugOutput += basicAddtionalBugOutput * level;
            curLevelGenerationFinish = false;
        }

        //完成当前关卡后，开始生成下一关的敌人
        if (!curLevelGenerationFinish)
        {
            GameObject curE;

            //生成敌人
            if (Time.time > bugTimer + bugInterval)
            {
                curE = GenerateEnemy(enemy.bug);
                curE.transform.SetParent(GameObject.Find("EnemyManager").transform);
                enemies.Add(curE);
                BugHasOutput++;
                curE = GenerateEnemy(enemy.hound);
                curE.transform.SetParent(GameObject.Find("EnemyManager").transform);
                enemies.Add(curE);
                BugHasOutput++;
                bugTimer = Time.time;

            }

            if (Time.time > gunnerTimer + gunnerInterval)
            {
                curE = GenerateEnemy(enemy.gunner);
                curE.transform.SetParent(GameObject.Find("EnemyManager").transform);
                enemies.Add(curE);
                gunnerTimer = Time.time;
                BugHasOutput++;
            }

            if (Time.time > bugBuilderTimer + bugBuilderInterval)
            {
                curE = GenerateEnemy(enemy.builder);
                curE.transform.SetParent(GameObject.Find("EnemyManager").transform);
                enemies.Add(curE);
                bugBuilderTimer = Time.time;
                BugHasOutput++;
            }
            if (BugHasOutput >= BugOutput)
                curLevelGenerationFinish = true;
        }
    }

    GameObject GenerateEnemy(enemy enemyIndex)
    {
        Vector2[] generatePosition = new Vector2[4];
        Vector2 position;
        AStarGrid curGrid;
        do
        {
            generatePosition[0] = new Vector2(40, UnityEngine.Random.Range(-40, 40));
            generatePosition[1] = new Vector2(-40, UnityEngine.Random.Range(-40, 40));
            generatePosition[2] = new Vector2(UnityEngine.Random.Range(-40, 40), 40);
            generatePosition[3] = new Vector2(UnityEngine.Random.Range(-40, 40), -40);
            position = generatePosition[UnityEngine.Random.Range(0, 4)];
            curGrid = AStarMgr.map[Mathf.RoundToInt(position.y) + MapManager.offsety, Mathf.RoundToInt(position.x) + MapManager.offsetx];
        }
        while (curGrid.Type == GridType.blocked);
        return Instantiate(enemyPrefabs[(int)enemyIndex], position, transform.rotation);

    }

    public void ResumeGame()
    {
        PausePanel.SetActive(false);
        pauseIsOpen = false;
        Time.timeScale = 1;
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        Cursor.visible = true;
        SceneManager.LoadScene(0);
    }



    void KeyInputListener()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanel.SetActive(pauseIsOpen = !pauseIsOpen);
            if (pauseIsOpen)
            {
                Time.timeScale = 0;
                //mapPanel.SetActive(false);
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    bool curActive = mapPanel.activeSelf;
        //    mapPanel.SetActive(!curActive);
        //}


    }


    public static string UpdateTime(float time)
    {
        return ((int)time / 60).ToString("00") + ":" + ((int)time % 60).ToString("00");
    }


    public void UpdateUI()
    {
        //血量显示更新
        if (player.GetComponent<PlayerController>().curHealth >= 0)
            curHealth.text = player.GetComponent<PlayerController>().curHealth.ToString();
        else
            curHealth.text = "0";
        //时间显示
        surviveTime = Time.time - surviveStart;
        //转化成分钟数
        instance.surviveTimeText.text = UpdateTime(surviveTime);
        //积分更新
        GameObject.Find("Canvas/ScoreBar/Score").GetComponent<Text>().text = totalScore.ToString();
        totalScoreText.text = totalScore.ToString();
        //击杀数更新
        takedownsText.text = takedowns.ToString();
        int weaponIndex = PlayerController.Instance.CurWeapon.GetComponent<Weapon>().weaponID - 1;
        curWeaponAmmo.text = PlayerController.Instance.curAmmo[weaponIndex] + "/" + PlayerController.Instance.maxAmmo[weaponIndex];
        coinCount.text = coin.ToString();
    }
}
