using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Datas
    private float moveSpeed;
    private float damage;

    private void FixedUpdate()
    {
        Move(new Vector3 (0, 0, 1));
    }

    private void Move(Vector3 movePosition)
    {
        transform.Translate(movePosition * moveSpeed * Time.deltaTime);
    }

    public void Init(float bulletSpeed, float bulletDamage)
    {
        moveSpeed = bulletSpeed;
        damage = bulletDamage;
    }

    public float Damage()
    {
        return damage;
    }
}
