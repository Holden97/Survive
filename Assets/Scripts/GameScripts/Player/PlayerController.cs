using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const int totalWeaponsCount = 4;
    static PlayerController instance;
    public float moveSpeed;
    public float defaultMoveSpeed = 6f;
    public GameObject bulletPrefab;
    public Vector3 offsetPosition;
    [Header("武器参数相关")]
    public bool shootingIsReady;
    public float shootingCoolDownTime;
    public float lastShotTime;
    public float debuffDuration;
    public int[] curAmmo = new int[totalWeaponsCount];
    public int[] maxAmmo = new int[totalWeaponsCount];
    public int[] clipAmmo = new int[totalWeaponsCount];
    GameObject curWeapon;
    [Header("生命值相关")]
    public float maxHealth;
    public float curHealth;
    public float reGenInterval;
    public float lastReGenTime;
    public float reGenRate;
    public bool reGening;
    [Header("受击参数")]
    public bool isBeating;
    float repulseSpeed;
    public Vector2 curBeatDirection;
    public float lastBeatTime;
    //远程的冰弹攻击
    public float lastElementBallBeatTime;
    float beatInterval = 0.2f;
    [Header("状态监测")]
    public bool moving;
    public bool isAlive;
    public bool isFrozen = false;
    public bool isBurning = false;
    [Header("动画控制")]
    Animator playerAnimator;//人物动画
    Animator swordAnimator;//砍刀动画
    [Header("武器预制体")]
    //枪支预制体
    //2020.3.28
    //三种枪分别是
    //QSG92手枪，AK-47步枪，Saiga-12霰弹枪
    //4.30
    //添加了第四种武器，砍刀
    public GameObject[] weaponList = new GameObject[4];
    [Header("自身音效")]
    public AudioClip noBulletClip;
    public AudioClip hurt;
    [Header("形象管理")]
    //用于管理形象的透明度
    //0.正常
    //1.冰冻减缓
    public Color[] spriteColor;


    public GameObject CurWeapon { get => curWeapon; }
    public static PlayerController Instance { get => instance; set => instance = value; }

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < totalWeaponsCount; i++)
        {
            maxAmmo[i] = weaponList[i].GetComponent<Weapon>().maxAmmo;
            curAmmo[i] = weaponList[i].GetComponent<Weapon>().maxAmmo;
            clipAmmo[i] = weaponList[i].GetComponent<Weapon>().clipAmmo;
        }
        Instance = this;
        shootingIsReady = true;
        maxHealth = 100;
        curHealth = maxHealth;
        isBeating = false;
        repulseSpeed = 6f;
        isAlive = true;
        playerAnimator = GetComponent<Animator>();
        curWeapon = GameObject.FindGameObjectWithTag("Weapon");
        reGenInterval = 1f;
        reGenRate = 2f;

    }

    private void Start()
    {
        shootingCoolDownTime = curWeapon.GetComponent<Weapon>().ShootInterval;
    }



    private void FixedUpdate()
    {
        if (curHealth <= 0)
            GameManager.Instance.curGameIsOver = true;
        if (curHealth <= 0 || GameManager.Instance.PauseIsOpen == true)
            return;
        if (isBeating)
        {
            transform.position += new Vector3(curBeatDirection.normalized.x, curBeatDirection.normalized.y, 0) * Time.deltaTime * repulseSpeed;
        }
        PlayerRotate();
        SwitchWeapons();
        Timer();
        PlayerAct();
        PlayerMove();

    }



    public void PlayerMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        transform.position += new Vector3(x, y).normalized * moveSpeed * Time.deltaTime;



        playerAnimator.SetFloat("moveSpeed", new Vector3(x, y).normalized.magnitude);

    }

    public void PlayerRotate()
    {
        Vector3 mouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mouseVector.x - transform.position.x > 0)
            playerAnimator.SetFloat("lookX", 1);
        else
            playerAnimator.SetFloat("lookX", -1);

        if (mouseVector.y - transform.position.y > 0)
            playerAnimator.SetFloat("lookY", 1);
        else
            playerAnimator.SetFloat("lookY", -1);
    }

    public void PlayerAct()
    {
        if (Input.GetMouseButton(0))
        {
            if (shootingIsReady && curAmmo[curWeapon.GetComponent<Weapon>().weaponID - 1] == -1)
            {
                shootingCoolDownTime = curWeapon.GetComponent<Weapon>().ShootInterval;
                curWeapon.GetComponent<Weapon>().Shoot();
                lastShotTime = Time.time;
                shootingIsReady = false;
            }
            if (shootingIsReady && curAmmo[curWeapon.GetComponent<Weapon>().weaponID - 1] > 0)
            {
                shootingCoolDownTime = curWeapon.GetComponent<Weapon>().ShootInterval;
                curWeapon.GetComponent<Weapon>().Shoot();
                curAmmo[curWeapon.GetComponent<Weapon>().weaponID - 1]--;
                lastShotTime = Time.time;
                shootingIsReady = false;
            }
            if (curAmmo[curWeapon.GetComponent<Weapon>().weaponID - 1] == 0)
            {
                AudioManager.PlayEClip(noBulletClip);
                UIManager.Instance.AddHint(TextManager.noAmmo(curWeapon.GetComponent<Weapon>().weaponID - 1));
            }
        }
        if (curWeapon.GetComponent<Weapon>().weaponID == 4)
        {
            swordAnimator = curWeapon.GetComponent<Animator>();
            if (Input.GetMouseButton(1))
            {
                swordAnimator.SetBool("rightButton", true);
            }
            else
            {
                swordAnimator.SetBool("rightButton", false);
            }
        }
    }

    public void Timer()
    {
        if (shootingCoolDownTime + lastShotTime <= Time.time)
        {
            shootingIsReady = true;
        }
        if (Time.time >= lastBeatTime + beatInterval)
        {
            isBeating = false;
        }
        if (Time.time >= lastElementBallBeatTime + debuffDuration)
        {
            moveSpeed = defaultMoveSpeed;
            isFrozen = false;
            isBurning = false;
            GetComponent<SpriteRenderer>().color = spriteColor[0];
        }

        //间隔结束后自动执行的操作：回血等等
        if (Time.time >= lastReGenTime + reGenInterval)
        {
            curHealth = maxHealth < curHealth + reGenRate ? maxHealth : curHealth + reGenRate;
            lastReGenTime = Time.time;
        }

    }

    public void SwitchWeapons()
    {
        Vector3 weaponPos = GameObject.FindGameObjectWithTag("Weapon").transform.position;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            foreach (Transform child in GameObject.Find("Player").transform)
            {
                if (child.tag == "Weapon")
                    Destroy(child.gameObject);
            }
            curWeapon = Instantiate(weaponList[0], weaponPos, transform.rotation);
            curWeapon.transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            foreach (Transform child in GameObject.Find("Player").transform)
            {
                if (child.tag == "Weapon")
                    Destroy(child.gameObject);
            }

            curWeapon = Instantiate(weaponList[1], weaponPos, transform.rotation);
            curWeapon.transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            foreach (Transform child in GameObject.Find("Player").transform)
            {
                if (child.tag == "Weapon")
                    Destroy(child.gameObject);
            }
            curWeapon = Instantiate(weaponList[2], weaponPos, transform.rotation);
            curWeapon.transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform);
        }

        if (Input.GetKey(KeyCode.Alpha4))
        {
            foreach (Transform child in GameObject.Find("Player").transform)
            {
                if (child.tag == "Weapon")
                    Destroy(child.gameObject);
            }
            curWeapon = Instantiate(weaponList[3], weaponPos, transform.rotation);
            curWeapon.transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform);
        }
    }

}
