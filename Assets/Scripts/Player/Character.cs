using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{

    //Movement Data's
    private Rigidbody rb;
    private float moveSpeed = 0f;
    private Vector3 startPosition = new Vector3(0, 1, 0);
    private Vector3 desirePosition = new Vector3(0,1,0);

    //Shoot Data's
    private GameObject bulletsGo;
    private float fireRate;
    private float bulletSpeed;
    private float bulletDamage;

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

    public void StartMove(Vector3 startPosition, Vector3 desirePosition)
    {
        this.startPosition = startPosition;
        this.desirePosition = desirePosition;
    }

    public Vector3 GetStartPosition()
    {
        return startPosition;
    }

    public Vector3 GetDesirePosition()
    {
        return desirePosition;
    }

    public void ChangeMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;     
    }

    #endregion

    #region Shoot
    IEnumerator Shoot()
    {
        if(bulletsGo != null)
        {
            yield return new WaitForSeconds(fireRate);

            GameObject newBullet = Instantiate(bulletsGo, transform.position + bulletsGo.transform.position, Quaternion.identity);

            foreach (Bullet bullet in newBullet.GetComponentsInChildren<Bullet>())
            {
                bullet.GetComponent<Bullet>().Init(bulletSpeed, bulletDamage);
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        StartCoroutine(Shoot());
    }

    public void ChangeShoot(GameObject bulletsGO, float fireRate, float bulletSpeed, float bulletDamage)
    {
        this.bulletsGo = bulletsGO;
        this.fireRate = fireRate;
        this.bulletSpeed = bulletSpeed;
        this.bulletDamage = bulletDamage;
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
