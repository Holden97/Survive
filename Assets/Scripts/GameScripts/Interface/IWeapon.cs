using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{


    GameObject BulletPrefab { get; }
    float ShootInterval { get; set; }
    float Damage { get; set; }




    void Shoot();

}
