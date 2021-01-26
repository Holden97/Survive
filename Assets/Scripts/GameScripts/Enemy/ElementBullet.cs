using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBullet : MonoBehaviour
{
    public float bulletSpeed;//子弹速度
    public float attackDamage;//子弹伤害
    public float suppressAbility;//移动速度的减缓buff大小
    public float suppressTime;//移动debuff持续时间
    public int ElementID;//元素弹编号 1.冰 2.火
    Vector3 attackDirection;
    GameObject player;
    GameObject parentGameObject;
    // Start is called before the first frame update
    void Start()
    {
        parentGameObject = transform.parent.gameObject;
        transform.parent = transform.parent.parent;
        player = GameObject.FindGameObjectWithTag("Player");
        attackDirection = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
        transform.up = attackDirection;
        Destroy(gameObject, 7f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * bulletSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != parentGameObject && !collision.CompareTag("EarthEnemy") && !collision.CompareTag("FireEnemy") && !collision.CompareTag("GoldEnemy"))
            Destroy(gameObject);

        if (collision.gameObject.tag.Contains("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.curHealth -= attackDamage;
            playerController.moveSpeed = playerController.isFrozen ? playerController.moveSpeed : playerController.moveSpeed - suppressAbility;
            switch (ElementID)
            {
                case 1:
                    playerController.isFrozen = true;
                    playerController.isBurning = false;
                    player.GetComponent<SpriteRenderer>().color = playerController.spriteColor[1];
                    break;
                case 2:
                    playerController.isFrozen = false;
                    playerController.isBurning = true;
                    break;

            }
            playerController.debuffDuration = suppressTime;
            playerController.lastElementBallBeatTime = Time.time;
            AudioManager.voiceSource.clip = AudioManager.Instance.hurtClip;
            AudioManager.voiceSource.Play();
        }
        if (collision.gameObject.tag.Contains("FireEnemy") && collision.gameObject != parentGameObject)
        {
            collision.gameObject.GetComponent<BugGunner>().curHealth -= 20;
        }
        if (collision.gameObject.tag.Contains("WoodEnemy"))
        {
            collision.gameObject.GetComponent<Hound>().curHealth -= 20;
        }
    }
}
