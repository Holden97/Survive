using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建筑者是近战单位
/// 移速中等
/// 血量高
/// 可以建筑生产土匪的建筑单位
/// 在建筑3个建筑后会主动玩家
/// 受伤后反击
/// </summary>
public class BugBuilder : MonoBehaviour
{
    public enum State
    {
        attack,
        idle,
        building,
        follow
    }


    float moveSpeed;
    float attackInterval;
    float actionInterval;
    float lastConfirmActionTime;//最后确认状态的时间
    float lastAttackTime;//最后攻击的时间
    public float lastBeAttackedTime = -4f;
    public State bugState;//0：停止行动，1：攻击，2：闲逛，3：建造，4：跟随
    GameObject player;//玩家
    int Direction;//0：上，1：下，2：左，3：右
    float attackDamage;
    public float curHealth;
    public float maxHealth;
    public bool isAlive;
    public GameObject elementPrefab;
    public static int score = 30;
    //建筑相关
    float buildCostTime;
    float buildInterval;
    float buildTimer;
    bool isBuilding;
    bool canBuild;
    float lastBuildTime;
    int buildCount = 0;
    public GameObject bugBasePrefab;
    GameObject curBase;
    //受击相关
    static float removeHateTime = 3f;
    //动画相关
    public Animator bugAnimator;
    //路径
    public List<AStarGrid> path;
    [Header("音效")]
    public AudioClip hurtClip;
    AudioSource audioSource;
    [Header("建筑颜色")]
    public Color buildingColor;
    public Color finishColor;



    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 4f;//速度
        attackInterval = 0.5f;//攻击间隔
        bugState = State.idle;//人物状态
        player = GameManager.player;//玩家
        attackDamage = 5f;//攻击伤害
        actionInterval = 0.2f;//行动间隔
        maxHealth = 150f;//最大生命值
        curHealth = maxHealth;//当前生命值
        lastConfirmActionTime = Time.time;//上一次确认状态的时间
        isAlive = true;//是否存活
        isBuilding = false;//是否正在建造
        canBuild = false;//能否建造
        audioSource = GetComponent<AudioSource>();//自身音源
        lastBuildTime = Time.time;//上一次建造的时间
        buildCostTime = 3f;//建造持续时间
        path = MapManager.SearchPath(transform.position, player.transform.position);
    }
    public void PlayHurtClip()
    {
        audioSource.clip = hurtClip;
        audioSource.Play();
    }
    // Update is called once per frame
    void Update()
    {

        if (GameManager.Instance.curGameIsOver)
            return;
        //死亡处理
        if (curHealth <= 0)
        {
            GameManager.Instance.takedowns++;
            isAlive = false;
            Instantiate(elementPrefab, transform.position, transform.rotation);
            AudioManager.PlayClip(AudioManager.Instance.bugBuilderDeathClip);
            GameManager.enemies.Remove(gameObject);
            Destroy(gameObject);
            GameManager.Instance.totalScore += score;
            return;
        }
        if (lastConfirmActionTime + actionInterval <= Time.time)
        {
            //状态切换
            ConfirmState();
            //计时器
            lastConfirmActionTime = Time.time;
        }
        //行动
        Action();
        SetAnimatorParameters(SwitchDirection(Direction), bugState);

    }

    private void SetAnimatorParameters(Vector2 directionVector, State bugState)
    {
        if (directionVector.x != 0 || directionVector.y != 0)
        {
            bugAnimator.SetFloat("vertical", directionVector.x);
            bugAnimator.SetFloat("horizontal", directionVector.y);
        }
        switch (bugState)
        {
            case State.follow:
                bugAnimator.SetFloat("speed", 1);
                break;
            case State.attack:
            case State.building:
                bugAnimator.SetFloat("speed", 0);
                break;
        }
    }

    public void ConfirmState()
    {
        if (bugState == State.building)
        {
            //当前时间>上次建筑时间+建筑消耗时间
            //建筑完成
            if (Time.time > lastBuildTime + buildCostTime)
            {
                bugState = State.idle;
                lastBuildTime = Time.time;
                buildCount++;
                Color curColor = curBase.GetComponent<SpriteRenderer>().color;
                curBase.GetComponent<SpriteRenderer>().color = finishColor;
                foreach (Transform item in curBase.transform)
                {
                    item.gameObject.GetComponent<SpriteRenderer>().color = finishColor;
                }
                curBase.GetComponent<Turret>().isBuilding = false;
            }
            else
            {
                Build();
            }
            return;
        }
        if (canBuild)
        {
            if (lastBeAttackedTime + removeHateTime >= Time.time)
            {
                bugState = State.follow;
                //确定follow的移动方向
                List<int> moveDirectionList = new List<int>();
                ConfirmFollowDirection(moveDirectionList);
                Direction = moveDirectionList[UnityEngine.Random.Range(0, 2)];
            }

            else
            {
                bugState = State.idle;
                Direction = UnityEngine.Random.Range(0, 4);
            }
        }
        else
        {
            bugState = State.follow;
            //确定follow的移动方向
            List<int> moveDirectionList = new List<int>();
            ConfirmFollowDirection(moveDirectionList);
            Direction = moveDirectionList[UnityEngine.Random.Range(0, 2)];
        }


        if (!canBuild && buildCount < 3)
        {
            buildInterval = UnityEngine.Random.Range(15, 20);
            canBuild = true;
        }

        if (canBuild && Time.time > lastBuildTime + buildInterval)
        {
            bugState = State.building;
            lastBuildTime = Time.time;
            curBase = Instantiate(bugBasePrefab, transform.position, transform.rotation);
            Color curColor = curBase.GetComponent<SpriteRenderer>().color;
            curBase.GetComponent<SpriteRenderer>().color = buildingColor;
            foreach (Transform item in curBase.transform)
            {
                item.gameObject.GetComponent<SpriteRenderer>().color = finishColor;
            }

        }


    }

    private void ConfirmFollowDirection(List<int> moveDirectionList)
    {
        if (player.transform.position.y - transform.position.y > 0)
            moveDirectionList.Add(0);
        else
            moveDirectionList.Add(1);
        if (player.transform.position.x - transform.position.x > 0)
            moveDirectionList.Add(3);
        else
            moveDirectionList.Add(2);
    }

    public void Action()
    {
        switch (bugState)
        {
            case State.follow:
                if (path == null)
                {
                    return;
                }
                //这里写成if (path != null)即为错误(PS.初始化与未初始化的区别4.28)
                if (path.Count != 0)
                    //判断进入格子是以碰撞体的中心与格子中心基本吻合作为条件，而不是GameObject的中心与格子中心重合
                    if (Mathf.Abs(path[0].Col - 50 - transform.position.x) >= 0.05 || Mathf.Abs(path[0].Row - 50 - (transform.position.y - 0.5f)) >= 0.05)
                    {
                        Vector3 normalizedDirection = new Vector3(Mathf.Abs(path[0].Col - 50 - transform.position.x) >= 0.05 ? (path[0].Col - 50 - transform.position.x) : 0, Mathf.Abs(path[0].Row - 50 - (transform.position.y - 0.5f)) >= 0.05 ? (path[0].Row - 50 - (transform.position.y - 0.5f)) : 0).normalized;
                        SetAnimatorParameters(normalizedDirection);
                        transform.position += Time.deltaTime * moveSpeed * normalizedDirection;
                    }
                    else
                    {
                        path.RemoveAt(0);
                    }
                break;
            case State.idle:
                transform.position += SwitchDirection(Direction) * Time.deltaTime * moveSpeed;
                break;
            case State.attack:
                {
                    if (Time.time >= lastAttackTime + attackInterval)
                    {
                        Attack();
                        lastAttackTime = Time.time;
                    }
                    break;
                }
            case State.building:
                {
                    float x = transform.position.x;
                    float y = transform.position.y;
                    if (x < 40 && x > -40 && y < 40 && y > -40)
                    {
                        Build();
                    }
                    canBuild = false;

                    break;
                }

        }

    }
    public void SetAnimatorParameters(Vector3 directionVector)
    {
        if (directionVector.x != 0 || directionVector.y != 0)
        {
            bugAnimator.SetFloat("vertical", directionVector.x);
            bugAnimator.SetFloat("horizontal", directionVector.y);
        }
    }

    public void Attack()
    {
        player.GetComponent<PlayerController>().isBeating = true;
        player.GetComponent<PlayerController>().lastBeatTime = Time.time;
        player.GetComponent<PlayerController>().curBeatDirection = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
        player.GetComponent<PlayerController>().curHealth -= attackDamage;

    }

    public void Build()
    {

    }


    public static Vector3 SwitchDirection(int mark)
    {
        switch (mark)
        {
            case 0: return new Vector2(0, 1);//上
            case 1: return new Vector2(0, -1);//下
            case 2: return new Vector2(-1, 0);//左
            case 3: return new Vector2(1, 0);//右
            default: return new Vector2(0, 0);
        }
    }
}
