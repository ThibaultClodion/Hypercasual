using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{

    //Movement Data's
    private bool canMove;
    private Rigidbody rb;
    private float moveSpeed;
    private Vector3 startPosition = new Vector3(0, 1, 0);
    private Vector3 desirePosition = new Vector3(0,1,0);

    //Shoot Data's
    private float bulletSpeed;
    private float bulletDamage;
    private GameObject bullet;
    private Vector3 offset;
    private float fireSpeed;

    //Die Data's
    private float explosionRadius = 50.0f;

    private void Start()
    {
        //Make the characters automaticaly shoot
        StartCoroutine(Shoot());

        //Get the rigibody
        rb = GetComponent<Rigidbody>();

    }

    private void FixedUpdate()
    {
        rb.velocity = ((desirePosition - transform.position + startPosition) * Time.deltaTime * moveSpeed);
    }

    #region Movement

    public void ChangeMove(Vector3 newPosition)
    {
        desirePosition = newPosition;
    }

    public void StartMove()
    {
        startPosition = transform.position;
        desirePosition = Vector3.zero;
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

    #region Collision
    private void OnCollisionEnter(Collision collision)
    {
        //If enter a collision with a player then destroy both
        if (collision.gameObject.tag == "Enemy")
        {
            //Delete the enemy
            Destroy(collision.gameObject);

            //Delete the character from the characters list of the PlayerController
            GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>().CharacterDestroy(this);

            //Destroy all entity on a radius
            Explosion();

            //Delete this gameObject
            Destroy(this.gameObject);
        }
    }

    private void Explosion()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider col in colliders)
        {
            if (col.tag == "Enemy")
            {
                Destroy(col.gameObject);
            }
        }
    }
    #endregion
}
