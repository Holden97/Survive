using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugBase : MonoBehaviour
{
    public GameObject bug;
    float generateInterval;
    float generateBasicInterval;
    float lastGenerateTime;
    bool underAttack;
    bool beSurrounded;
    public int bugAmount;
    float generateCoefficient;
    [Header("生命值管理")]
    public float maxHealth;
    public float curHealth;
    public bool isBuilding = true;

    public bool UnderAttack { get => underAttack; }
    public bool BeSurrounded { get => beSurrounded; }

    // Start is called before the first frame update
    void Start()
    {
        bugAmount = 0;
        maxHealth = 300;
        curHealth = maxHealth;
        generateInterval = 2f;
        generateBasicInterval = 2f;
        underAttack = false;
        beSurrounded = false;
        generateCoefficient = 10f;

    }

    // Update is called once per frame
    void Update()
    {

        if (curHealth <= 0)
            Destroy(gameObject);
        if (isBuilding)
            return;
        if (Time.time >= lastGenerateTime + generateInterval)
        {
            GameObject curBug = Instantiate(bug, transform.Find("spawn").position, transform.rotation, transform);
            GameManager.enemies.Add(curBug);
            curBug.transform.parent = GameObject.Find("EnemyManager").transform;
            bugAmount++;
            //4.15为了暂时平衡地图太大而玩家找巢穴很费事的情况，暂时定为生成一定数量后自动摧毁巢穴
            if (bugAmount >= 20)
                Destroy(gameObject);
            generateInterval = generateBasicInterval * (2 * bugAmount + generateCoefficient) /( bugAmount + generateCoefficient);
            lastGenerateTime = Time.time;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            beSurrounded = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            beSurrounded = false;
        }
    }
}
