using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Obstacle : MonoBehaviour
{
    //Common Datas
    private float moveSpeed;
    private float actualHP;

    //Gauge Obstacle
    [SerializeField] Material gaugeObstacleMaterial;
    private int gaugeIncrement;

    //Weapon Obstacle
    private bool isRedObstacle = false;
    [SerializeField] Material redObstacleMaterial;
    private bool isGreenObstacle = false;
    [SerializeField] Material greenObstacleMaterial;

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
    public void InitGaugeObstacle(float hp, float speed, int gauge)
    {
        actualHP = hp;
        moveSpeed = speed;
        gaugeIncrement = gauge;
        GetComponent<MeshRenderer>().material = gaugeObstacleMaterial;
    }

    public void InitWeaponObstacle(float hp, float speed)
    {
        actualHP = hp;
        moveSpeed = speed;
        gaugeIncrement = 0;

        //Define which type of weapon is
        int random = Random.Range(0, 2);

        if(random == 0)
        {
            isRedObstacle = true;
            GetComponent<MeshRenderer>().material = redObstacleMaterial;
        }
        else if(random == 1) 
        {
            isGreenObstacle = true;
            GetComponent<MeshRenderer>().material = greenObstacleMaterial;
        }
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

            //The Obstacle is eliminated
            if (actualHP < 0)
            {
                //Give the good upgrade
                if(isRedObstacle)
                {
                    GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>().UpgradeRedWeapon();
                }
                else if(isGreenObstacle)
                {
                    GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>().UpgradeGreenWeapon();
                }
                else
                {
                    GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>().increaseGauge(gaugeIncrement);
                }

                //Destroy the gameObject
                Destroy(this.gameObject);
            }
        }
    }
    #endregion
}
