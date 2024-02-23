using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Roads")]
    [SerializeField] private GameObject road;
    [SerializeField] private List<Road> actualRoads;
    [SerializeField] private float roadSpeed;

    [Header("Enemies")]
    [SerializeField] GameObject enemyGO;
    [SerializeField] float enemySpeed;
    [SerializeField] float enemyHp;
    [SerializeField] int enemyGaugeIncrement;

    //Spawn Data's
    private int minEnemiesSpawn = 50;
    private int maxEnemiesSpawn = 200;
    

    [Header("Obstacles")]
    [SerializeField] GameObject obstacleGO;
    [SerializeField] float obstacleSpeed;
    [SerializeField] float obstacleHp;
    [SerializeField] int obstacleGaugeIncrement;
    private float[] xSpawnPositions = new float[] { -5, 0, 5 };

    [Header("Player")]
    [SerializeField] PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        UpdateRoadsSpeed();
    }


    #region RoadManagement
    private void UpdateRoadsSpeed()
    {
        foreach (Road road in actualRoads) 
        {
            road.ChangeSpeed(roadSpeed);
        }
    }

    private void CreateRoad(Vector3 position)
    {
        //The -1 is here to not have a hole on the road
        float zOffset = road.transform.localScale.z * 5 - 1;

        //Instantiate the road at the right position
        GameObject newRoad = Instantiate(road, position + new Vector3(0,0, zOffset), Quaternion.identity);

        //Update the actualRoads List by adding the new one and deleting the old one
        actualRoads.Remove(actualRoads[0]);
        actualRoads.Add(newRoad.GetComponent<Road>());

        //Update Roads Speed
        UpdateRoadsSpeed();

        int nbEnemiesSpawn = Random.Range(minEnemiesSpawn, maxEnemiesSpawn);

        //Define the hp of entity
        float roadHp = getRoadHp();
        enemyHp = roadHp / nbEnemiesSpawn;
        obstacleHp = roadHp * 10 / nbEnemiesSpawn;

        //Define the max X and Z position
        float maxX = newRoad.transform.localScale.x * 5 - 1;
        float maxZ = newRoad.transform.localScale.z * 5 - 1;
        
        //Instiante the entities
        for (int i = 0; i <  nbEnemiesSpawn; i++) 
        {
            if (Random.Range(0,20) == 0)
            {
                InstantiateObstacle(new Vector3(xSpawnPositions[Random.Range(0, 3)], obstacleGO.transform.position.y, Random.Range(-maxZ, maxZ)), newRoad);
            }
            else
            {
                InstantiateEnemy(new Vector3(Random.Range(-maxX, maxX), enemyGO.transform.position.y, Random.Range(-maxZ, maxZ)), newRoad);
            }
            
        }
    }

    #endregion

    #region Enemies
    private void InstantiateEnemy(Vector3 position, GameObject parent)
    {
        //Instantiate the GameObject
        GameObject newEnnemy = Instantiate(enemyGO, parent.transform.position + position, Quaternion.identity);
        newEnnemy.transform.SetParent(parent.transform);

        //Instantiate the script Enemy
        newEnnemy.GetComponent<Enemy>().Init(enemyHp, enemySpeed, enemyGaugeIncrement);
    }
    #endregion

    #region Obstacles

    private void InstantiateObstacle(Vector3 position, GameObject parent)
    {
        //Instantiate the GameObject
        GameObject newObstacle = Instantiate(obstacleGO, parent.transform.position + position, Quaternion.identity);
        newObstacle.transform.SetParent(parent.transform);

        //Random between Gauge and Weapon Obstacle
        if (Random.Range(0, 2) == 0)
        {
            //Initialize a Gauge Obstacle
            newObstacle.GetComponent<Obstacle>().InitGaugeObstacle(obstacleHp, obstacleSpeed, obstacleGaugeIncrement);
        }
        else
        {
            //Initialize a Weapon Obstacle
            newObstacle.GetComponent<Obstacle>().InitWeaponObstacle(obstacleHp, obstacleSpeed);
        }
    }
    #endregion

    #region Balancing
    private float getTime()
    {
        return Time.realtimeSinceStartup;
    }

    private float getFullRoadTime()
    {
        return road.transform.localScale.z * 10 / roadSpeed;
    }

    private float getFirePower()
    {
        return playerController.getFirePower(getFullRoadTime());
    }

    private float getCoefficient(float yOrigin,float xOrigin, float yFinish, float xFinish)
    {
        return (yFinish - yOrigin) / (xFinish - xOrigin);
    }

    private float getRatio_hp_fp()
    {
        float min = getTime() / 60;

        //First linear function
        if(min < 2f)
        {
            return 0.5f + getCoefficient(0.5f, 0, 0.8f, 2) * min;
        }
        //Second linear function
        else
        {
            return 0.5f + getCoefficient(0.8f, 2, 1.5f, 30) * min;
        }
    }

    private float getRoadHp()
    {
        float roadHpmin = 0; // For Later Balance

        return Mathf.Max(roadHpmin, getRatio_hp_fp() * getFirePower());
    }

    #endregion

    #region CollisionAndTrigger
    //When it need a new road
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Road")
        {
            CreateRoad(gameObject.transform.position);
        }
    }
    #endregion
}
