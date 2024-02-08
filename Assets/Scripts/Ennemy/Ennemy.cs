using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Ennemy : MonoBehaviour
{
    //Datas
    [SerializeField] DataEnnemy datas;
    private float actualHP;
    private GameObject targetedCharacter;
    

    // Start is called before the first frame update
    void Start()
    {
        //Initialize the HP
        actualHP = datas.maxHP;

        //Initialize the Targeted Character
        UpdateTargetedCharacter();
    }

    private void FixedUpdate()
    {
        //If the targetedCharacter still exist then run on Him
        if(targetedCharacter != null) 
        {
            Move(targetedCharacter.transform.position);
        }
        //Else find another targetedCharacter
        else
        {
            UpdateTargetedCharacter();
        }
    }

    private void UpdateTargetedCharacter()
    {
        //Find a member of the troup
        targetedCharacter = GameObject.FindGameObjectWithTag("Player");
    }

    private void Move(Vector3 movePosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, movePosition, datas.moveSpeed * Time.deltaTime);
    }

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

            //Destroy if he has no health
            if(actualHP < 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
