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
    private bool isCharacterObstacle = false;
    [SerializeField] Material characterObstacleMaterial;

    //Weapon Obstacle
    private bool isWeaponObstacle = false;
    [SerializeField] Material weaponObstacleMaterial;

    //Gem Obstacle
    private bool isGemObstacle = false;
    [SerializeField] Material gemObstacleMaterial;

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
    public void InitCharacterObstacle(float hp, float speed)
    {
        //Basic Information
        actualHP = hp;
        moveSpeed = speed;

        //Character information
        isCharacterObstacle = true;
        GetComponent<MeshRenderer>().material = characterObstacleMaterial;
    }

    public void InitWeaponObstacle(float hp, float speed)
    {
        //Basic Information
        actualHP = hp;
        moveSpeed = speed;

        //Weapon information
        isWeaponObstacle = true;
        GetComponent<MeshRenderer>().material = weaponObstacleMaterial;
    }

    public void InitGemObstacle(float hp, float speed)
    {
        //Basic Information
        actualHP = hp;
        moveSpeed = speed;

        //Gem information
        isGemObstacle = true;
        GetComponent<MeshRenderer>().material = gemObstacleMaterial;
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
                if(isWeaponObstacle)
                {
                    GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>().UpgradeWeapon();
                }
                else if(isGemObstacle)
                {
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().AddAGem();
                }
                else if(isCharacterObstacle)
                {
                    GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>().CreateNewCharacter();
                }

                //Destroy the gameObject
                Destroy(this.gameObject);
            }
        }
    }
    #endregion
}
