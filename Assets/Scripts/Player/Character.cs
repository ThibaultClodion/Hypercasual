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
    private Vector3 velocity;

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
        /*if(velocity != Vector3.zero) 
        {
            rb.velocity = Vector3.Lerp(rb.velocity, velocity, Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, velocity, Time.fixedDeltaTime * 3);
        }*/

        //rb.MovePosition(transform.position + velocity * Time.deltaTime

        // Get the delta position  
        Vector3 dir = velocity - rb.position;
        // Get the velocity required to reach the target in the next frame
        dir /= Time.fixedDeltaTime;
        // Clamp that to the max speed
        dir = Vector3.ClampMagnitude(dir, moveSpeed);
        // Apply that to the rigidbody
        rb.velocity = dir;
    }

    #region Movement

    public void ChangeMove(Vector3 newVelocity) 
    {
        velocity = newVelocity;
        //velocity = newVelocity * moveSpeed * 200 * Time.fixedDeltaTime;
    }

    public void ChangeMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;     
    }

    /*public void ChangeMoveRayCast(Vector3 newMove)
    {
        movePosition = newMove;

        //In the fixed Update
         * if((movePosition - transform.position).magnitude > 0.5f)
        {
            Vector3 direction = (movePosition - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.5f);
        }
    }

    public void DontMoveRaycast()
    {
        movePosition = transform.position;
    }
    
     */

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
