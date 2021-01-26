using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IWeapon
{

    //位置参数
    Vector3 mousePosition;
    Vector2 rotationVector2;
    //武器参数
    public GameObject bulletPrefab;
    public float shootInterval;
    public float damage;
    public int clipAmmo;
    public int maxAmmo;
    public int curAmmo;
    GameObject firePoint;
    public int weaponID;
    //武器的两种音效
    public AudioClip shootClip;
    public AudioClip reloadClip;


    //霰弹枪每发的子弹个数
    static int Saiga_12_Count = 5;



    public float ShootInterval { get => shootInterval; set => shootInterval = value; }
    public float Damage { get => damage; set => damage = value; }
    public GameObject BulletPrefab => bulletPrefab;


    private void Awake()
    {
        if (weaponID != 4)
        {
            firePoint = transform.Find("FirePoint").gameObject;
        }
        WeaponRotation();
    }

    // Update is called once per frame
    void Update()
    {
        WeaponRotation();


    }

    private void WeaponRotation()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rotationVector2 = mousePosition - transform.position;
        if (mousePosition.x - transform.position.x < 0)
        {
            transform.right = -rotationVector2;
        }
        else
        {
            float rotationAngle = Mathf.Rad2Deg * Mathf.Atan2(rotationVector2.y, rotationVector2.x);
            transform.rotation = Quaternion.Euler(0, 180, -rotationAngle);
        }
    }

    public void Shoot()
    {
        if (weaponID == 1 || weaponID == 2)
        {
            Instantiate(bulletPrefab, firePoint.transform.position, transform.rotation);
        }

        if (weaponID == 3)
        {
            for (int i = 0; i < Saiga_12_Count; i++)
            {
                Instantiate(bulletPrefab, firePoint.transform.position, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + 5 * i));
            }
        }

        if (weaponID == 4)
        {
            Animator swordAnim = gameObject.GetComponent<Animator>();
            swordAnim.SetBool("leftButton", true);
            swordAnim.SetBool("rightButton", false);
        }

        AudioManager.PlayClip(shootClip);


    }

    private void Attack()
    {
        throw new NotImplementedException();
    }


    public void SetAnimPara()
    {
        Animator swordAnim = gameObject.GetComponent<Animator>();
        swordAnim.SetBool("leftButton", false);
        swordAnim.SetBool("rightButton", false);

    }
}
