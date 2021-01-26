using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    GameObject firePoint;
    GameObject player;
    public GameObject shellPrefab;
    Vector2 rotationVector;
    public bool isBuilding = true;//正在修建
    public float maxHealth = 200;
    public float curHealth;
    TurretState state;
    float playerDistance;

    float attackInterval = 0.3f;
    float lastAttackTime;

    //state，1：闲置 2：攻击
    enum TurretState
    {
        idle,
        attack
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.player;
        firePoint = gameObject.transform.Find("FirePoint").gameObject;
        lastAttackTime = Time.time;
        curHealth = maxHealth;
        state = TurretState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (curHealth <= 0)
            Destroy(gameObject);
        if (isBuilding)
            return;
        ConfirmState();
        Action();
    }

    private void ConfirmState()
    {
        playerDistance = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2) + Mathf.Pow(player.transform.position.y - transform.position.y, 2));
        if (playerDistance >= 12)
            state = TurretState.idle;
        else
            state = TurretState.attack;
    }

    private void Action()
    {
        rotationVector = player.transform.position - transform.position;
        firePoint.transform.up = rotationVector;
        if (Time.time > lastAttackTime + attackInterval && state == TurretState.attack)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void Attack()
    {
        Instantiate(shellPrefab, transform.position, transform.rotation, transform);
    }
}
