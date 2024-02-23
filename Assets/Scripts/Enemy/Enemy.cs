using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    //Datas
    private float moveSpeed;
    private float maxHp;
    private float actualHP;
    private int gaugeIncrement;
    private GameObject targetedCharacter;
    private float detectionDistance = 40f;

    //Game Manager
    private GameManager gameManager;
    

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

    public void Init(float hp, float speed, int gauge, GameManager GM)
    {
        maxHp = hp;
        actualHP = hp;
        moveSpeed = speed;
        gaugeIncrement = gauge;
        gameManager = GM;
    }

    private float GetScore()
    {
        return 15 * maxHp + 100;
    }

    #endregion

    #region CollisionsAndTrigger

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
                gameManager.IncreaseScore(GetScore());
                Destroy(this.gameObject);
            }
        }
    }
    #endregion
}
