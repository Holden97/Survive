using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制飞虫的移动与攻击
/// 猎犬是近战单位
/// </summary>
public class Hound : MonoBehaviour
{
    public enum State
    {
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
    public List<AStarGrid> path;//寻路后的结果

    public State bugState;//，1：攻击，2：闲逛，3：防御，4：追踪
    GameObject player;
    int Direction;//0：上，1：下，2：左，3：右
    float attackDamage;
    public float curHealth;
    public float maxHealth;
    public bool isAlive;
    public GameObject elementPrefab;
    public static int score = 20;
    //动画控制器
    Animator bugAnimator;

    [Header("音效")]
    public AudioClip hurtClip;
    public AudioClip[] barkClip;

    AudioSource audioSource;



    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 8f;
        attackInterval = 1f;
        bugState = State.idle;
        player = GameManager.player;
        attackDamage = 8f;
        actionInterval = 0.5f;
        maxHealth = 40f;
        curHealth = maxHealth;
        lastConfirmActionTime = Time.time;
        isAlive = true;
        bugAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
            if (transform.parent != GameObject.Find("EnemyManager").transform)
                transform.parent.gameObject.GetComponent<BugBase>().bugAmount--;
            isAlive = false;
            Instantiate(elementPrefab, transform.position, transform.rotation);
            AudioManager.PlayClip(AudioManager.Instance.houndDeathClip);
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
        //确定移动还是不移动，如果移动，确定移动方向
        if ((Mathf.Sqrt(Mathf.Pow((player.transform.position.x - transform.position.x), 2) + Mathf.Pow((player.transform.position.y - transform.position.y), 2))) >= 1)
        {
            bugState = State.follow;
            bugAnimator.SetBool("isAttacking", false);
        }
        else
        {
            bugState = State.attack;
            bugAnimator.SetBool("isAttacking", true);

        }

    }


    public static Vector3 SwitchDirection(int mark)
    {
        switch (mark)
        {
            case 0:
                return new Vector2(0, 1);
            case 1:
                return new Vector2(0, -1);
            case 2:
                return new Vector2(-1, 0);
            case 3:
                return new Vector2(1, 0);
            default: return new Vector2(0, 0);
        }
    }


    public void SetAnimatorParameters(Vector3 directionVector)
    {
        if (directionVector.x != 0 || directionVector.y != 0)
        {
            bugAnimator.SetFloat("horizontal", directionVector.x);
            bugAnimator.SetFloat("vertical", directionVector.y);
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
                        Attack();
                        lastAttackTime = Time.time;
                    }
                    break;
                }

        }

    }

    public void Attack()
    {
        player.GetComponent<PlayerController>().isBeating = true;
        player.GetComponent<PlayerController>().lastBeatTime = Time.time;
        player.GetComponent<PlayerController>().curBeatDirection = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
        player.GetComponent<PlayerController>().curHealth -= attackDamage;
        AudioManager.voiceSource.clip = AudioManager.Instance.hurtClip;
        AudioManager.voiceSource.Play();

    }


    public void Bark()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = barkClip[Random.Range(0, 2)];
            audioSource.Play();
        }
    }


}
