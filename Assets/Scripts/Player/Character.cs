using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private float bulletSpeed;
    private float bulletDamage;

    public void Move(Vector3 movePosition)
    {
        transform.position = Vector3.Lerp(transform.position, movePosition, Time.deltaTime);
    }

    public void Fire(GameObject bullet, Vector3 offset)
    {
        GameObject newBullet = Instantiate(bullet, transform.position + offset, Quaternion.identity);
        newBullet.GetComponent<Bullet>().Init(bulletSpeed, bulletDamage);
    }

    public void Init(float bulletSpeed, float bulletDamage)
    {
        this.bulletSpeed = bulletSpeed;
        this.bulletDamage = bulletDamage;
    }
}
