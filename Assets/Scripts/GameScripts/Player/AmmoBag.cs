using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBag : MonoBehaviour
{
    public AudioClip pickClip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //为随机一种武器增加弹药
            int index = Random.Range(0, 3);
            UIManager.Instance.AddHint(TextManager.PickUpBag(index));
            PlayerController.Instance.curAmmo[index] = PlayerController.Instance.maxAmmo[index];
            AudioManager.PlayClip(pickClip);
            Destroy(gameObject);
            GameManager.Instance.ammoBagTimer = Time.time;
        }
    }
}
