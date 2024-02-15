using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{

    //Movement Data's
    private float moveSpeed;
    private Rigidbody rb;
    private Vector3 movePosition = new Vector3(0,1,0);

    //Shoot Data's
    private float bulletSpeed;
    private float bulletDamage;
    private GameObject bullet;
    private Vector3 offset;
    private float fireSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //Make the characters automaticaly shoot
        StartCoroutine(Shoot());
    }

    private void FixedUpdate()
    {
        Move();
    }

    #region Movement
    public void Move()
    {
        rb.velocity = movePosition * moveSpeed;
    }

    public void ChangeMove(Vector3 newMove)
    {
        movePosition = newMove;
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

    #region Initialization
    public void Init(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }
    #endregion
}
