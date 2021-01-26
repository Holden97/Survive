using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BulletController : MonoBehaviour
{
    float bulletSpeed = 20;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += -transform.right * bulletSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Building" || collision.gameObject.tag == "Rock" || collision.gameObject.tag == "Tree")
            Destroy(gameObject);
        if (collision.gameObject.tag.Contains("Building"))
        {
            collision.gameObject.GetComponent<BugBase>().curHealth -= 20;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Enemy"))
            Destroy(gameObject);
        if (collision.gameObject.tag.Contains("FireEnemy"))
        {
            collision.gameObject.GetComponent<BugGunner>().curHealth -= 20;
            collision.gameObject.GetComponent<BugGunner>().PlayHurtClip();
        }
        if (collision.gameObject.tag.Contains("EarthEnemy"))
        {
            collision.gameObject.GetComponent<Bug>().curHealth -= 20;
            collision.gameObject.GetComponent<Bug>().PlayHurtClip();
        }

        if (collision.gameObject.tag.Contains("GoldEnemy"))
        {
            collision.gameObject.GetComponent<BugBuilder>().curHealth -= 20;
            collision.gameObject.GetComponent<BugBuilder>().lastBeAttackedTime = Time.time;
            collision.gameObject.GetComponent<BugBuilder>().PlayHurtClip();
        }
        if (collision.gameObject.tag.Contains("WaterEnemy"))
        {
            collision.gameObject.GetComponent<Turret>().curHealth -= 20;
        }
        if (collision.gameObject.tag.Contains("WoodEnemy"))
        {
            collision.gameObject.GetComponent<Hound>().curHealth -= 20;
        }
    }
}
