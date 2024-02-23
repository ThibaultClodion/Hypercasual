using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Datas
    public float moveSpeed;
    private float damage;
    private float range;
    private Vector3 initialPosition;


    private void FixedUpdate()
    {
        if (Vector3.Distance(initialPosition, transform.position) > range)
        {
            Destroy(gameObject);
        }

        Move(new Vector3 (0, 0, 1));
    }

    private void Move(Vector3 movePosition)
    {
        transform.Translate(movePosition * moveSpeed * Time.deltaTime);
    }

    public void Init(float bulletSpeed, float bulletDamage, float bulletRange)
    {
        moveSpeed = bulletSpeed;
        damage = bulletDamage;
        range = bulletRange;

        initialPosition = transform.position;
    }

    public float Damage()
    {
        return damage;
    }
}
