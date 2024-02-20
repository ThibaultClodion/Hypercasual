using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{

    //Movement Data's
    private Rigidbody rb;
    private Vector3 movePosition;
    private float moveSpeed;

    //Shoot Data's
    private float bulletSpeed;
    private float bulletDamage;
    private GameObject bullet;
    private Vector3 offset;
    private float fireSpeed;

    private void Start()
    {
        //Make the characters automaticaly shoot
        StartCoroutine(Shoot());

        //Get the rigibody
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if((movePosition - transform.position).magnitude > 0.5f)
        {
            Vector3 direction = (movePosition - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    #region Movement
    public void ChangeMove(Vector3 newMove)
    {
        movePosition = newMove;
    }

    public void ChangeMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;     
    }

    #endregion

    #region Shoot
    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(fireSpeed);

        GameObject newBullet = Instantiate(bullet, transform.position + offset, Quaternion.identity);
        newBullet.GetComponent<Bullet>().Init(bulletSpeed, bulletDamage);

        StartCoroutine(Shoot());
    }

    public void ChangeShoot(GameObject bullet, float bulletSpeed, float bulletDamage, float fireSpeed, Vector3 offset)
    {
        this.bullet = bullet;
        this.bulletSpeed = bulletSpeed;
        this.bulletDamage = bulletDamage;
        this.fireSpeed = fireSpeed;
        this.offset = offset;
    }
    #endregion
}
