using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{

    //Movement Data's
    private CharacterJoint joint;

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

        //Joint the PlayerController Rigidbody
        joint = GetComponent<CharacterJoint>();
        joint.connectedBody = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<Rigidbody>();
    }

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
