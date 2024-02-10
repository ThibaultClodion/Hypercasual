using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    //Datas
    private float moveSpeed;
    private float actualHP;
    private float gaugeIncrement;
    private GameObject targetedCharacter;
    private float detectionDistance = 30f;
    

    // Start is called before the first frame update
    void Start()
    {
        //Initialize the Targeted Character
        UpdateTargetedCharacter();
    }

    private void FixedUpdate()
    {
        //If the targetedCharacter still exist then run on Him
        if(targetedCharacter != null) 
        {
            //Move to the player if he is close
            if(Vector3.Distance(transform.position, targetedCharacter.transform.position) < detectionDistance)
            {
                Move(targetedCharacter.transform.position);
            }
        }
        //Else find another targetedCharacter
        else
        {
            UpdateTargetedCharacter();
        }
    }

    #region Movement
    private void UpdateTargetedCharacter()
    {
        //Find a member of the troup
        targetedCharacter = GameObject.FindGameObjectWithTag("Player");
    }

    private void Move(Vector3 movePosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, movePosition, moveSpeed * Time.deltaTime);
    }
    #endregion

    #region DataManagement

    public void Init(float hp, float speed, float gauge)
    {
        actualHP = hp;
        moveSpeed = speed;
        gaugeIncrement = gauge;
    }

    #endregion

    #region CollisionsAndTrigger
    private void OnCollisionEnter(Collision collision)
    {
        //If enter a collision with a player then destroy both
        if(collision.gameObject.tag == "Player")
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If enter a collision with a bullet
        if(other.gameObject.tag == "Bullet")
        {
            //Apply the damage
            actualHP -= other.GetComponent<Bullet>().Damage();
            Destroy(other.gameObject);

            //Destroy and increment the gauge if the enemy died
            if(actualHP < 0)
            {
                GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>().increaseGauge(gaugeIncrement);
                Destroy(this.gameObject);
            }
        }
    }
    #endregion
}
