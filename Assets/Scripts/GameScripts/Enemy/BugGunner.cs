using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 火元素是远程攻击单位
/// 拥有燃烧大地的技能
/// 会喷射火球
/// 不能够近战
/// </summary>
public class BugGunner : MonoBehaviour
{
    public enum State
    {
        stop,
        attack,
        idle,
        defense,
        follow
    }


    float moveSpeed;
    float attackInterval;
    float actionInterval;
    float lastConfirmActionTime;
    float lastAttackTime;
    public State bugState;//0：停止行动，1：攻击，2：闲逛，3：防御，4：追踪
    GameObject player;
    int Direction;//0：上，1：下，2：左，3：右
    public float curHealth;
    public float maxHealth;
    public bool isAlive;
    public GameObject elementPrefab;
    public GameObject fireBallPrefab;
    public static int score = 50;
    public Animator bugAnimator;
    [Header("音效")]
    public AudioClip hurtClip;
    public AudioClip attackClip;
    AudioSource audioSource;
    //路径
    public List<AStarGrid> path;



    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 2f;
        attackInterval = 0.2f;
        bugState = State.idle;
        player = GameObject.Find("Player").gameObject;
        actionInterval = 1.5f;
        maxHealth = 200f;
        curHealth = maxHealth;
        lastConfirmActionTime = Time.time;
        isAlive = true;
        audioSource = GetComponent<AudioSource>();
        path = MapManager.SearchPath(transform.position, player.transform.position);
    }

    public void PlayHurtClip()
    {
        audioSource.clip = hurtClip;
        audioSource.Play();
    }

    public void PlayAttackClip()
    {
        audioSource.PlayOneShot(attackClip);
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
            AudioManager.PlayClip(AudioManager.Instance.bugGunnerDeathClip);
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

    }



    public void ConfirmState()
    {
        if ((Mathf.Sqrt(Mathf.Pow((player.transform.position.x - transform.position.x), 2) + Mathf.Pow((player.transform.position.y - transform.position.y), 2))) <= 8)
        {
            bugState = State.attack;
        }
        else
        {
            bugState = State.follow;
            //确定follow的移动方向
            List<int> moveDirectionList = new List<int>();
            if (player.transform.position.y - transform.position.y > 0)
                moveDirectionList.Add(0);
            else
                moveDirectionList.Add(1);
            if (player.transform.position.x - transform.position.x > 0)
                moveDirectionList.Add(3);
            else
                moveDirectionList.Add(2);

            Direction = moveDirectionList[UnityEngine.Random.Range(0, 2)];

        }



    }

    /// <summary>
    /// mark是0-3的数字，转化的向量依次对应上下左右
    /// </summary>
    /// <param name="mark"></param>
    /// <returns></returns>
    public static Vector3 SwitchDirection(int mark)
    {
        switch (mark)
        {
            case 0: return new Vector2(0, 1);
            case 1: return new Vector2(0, -1);
            case 2: return new Vector2(-1, 0);
            case 3: return new Vector2(1, 0);
            default: return new Vector2(0, 0);
        }
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
                SetAnimatorParameters(SwitchDirection(Direction));
                break;
            case State.attack:
                {
                    if (Time.time >= lastAttackTime + attackInterval)
                    {
                        PlayAttackClip();
                        Attack(Direction);
                        lastAttackTime = Time.time;
                    }
                    break;
                }

        }

    }

    public void SetAnimatorParameters(Vector2 directionVector)
    {
        if (directionVector.x != 0 || directionVector.y != 0)
        {
            bugAnimator.SetFloat("vertical", directionVector.x);
            bugAnimator.SetFloat("horizontal", directionVector.y);
        }
    }

    public void Attack(int direction)
    {
        switch (direction)
        {
            case 0:
                Instantiate(fireBallPrefab, transform.Find("FirePointU").position, transform.rotation).transform.SetParent(gameObject.transform);

                break;
            case 1:
                Instantiate(fireBallPrefab, transform.Find("FirePointD").position, transform.rotation).transform.SetParent(gameObject.transform);
                break;
            case 2:
                Instantiate(fireBallPrefab, transform.Find("FirePointL").position, transform.rotation).transform.SetParent(gameObject.transform);
                break;
            case 3:
                Instantiate(fireBallPrefab, transform.Find("FirePointR").position, transform.rotation).transform.SetParent(gameObject.transform);
                break;

        }

    }

}
