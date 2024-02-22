using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Obstacle : MonoBehaviour
{
    //Datas
    private float moveSpeed;
    private float actualHP;
    private int gaugeIncrement;

    private void FixedUpdate()
    {
        Move(new Vector3(transform.position.x,transform.localScale.y / 2,-50));
    }

    #region Movement
    private void Move(Vector3 movePosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, movePosition, moveSpeed * Time.deltaTime);
    }
    #endregion

    #region DataManagement
    public void Init(float hp, float speed, int gauge)
    {
        actualHP = hp;
        moveSpeed = speed;
        gaugeIncrement = gauge;
    }

    #endregion

    #region CollisionAndTrigger

    private void OnTriggerEnter(Collider other)
    {
        //If enter a collision with a bullet
        if(other.gameObject.tag == "Bullet")
        {
            //Apply the damage
            actualHP -= other.GetComponent<Bullet>().Damage();
            Destroy(other.gameObject);

            //Destroy and increment the gauge if the enemy died
            if (actualHP < 0)
            {
                GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>().increaseGauge(gaugeIncrement);
                Destroy(this.gameObject);
            }
        }
    }
    #endregion
}
