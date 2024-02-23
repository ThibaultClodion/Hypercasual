using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    //Prefab of how the bullet are shoot
    public GameObject bulletsGo;

    //Datas of the bullets
    public float fireRate;
    public float bulletSpeed;
    public float bulletDamage;
}
