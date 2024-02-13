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
    [SerializeField] float enemyGaugeIncrement;
    private int minEnemiesSpawn = 20;
    private int maxEnemiesSpawn = 60;
    

    [Header("Obstacles")]
    [SerializeField] GameObject obstacleGO;
    [SerializeField] float obstacleSpeed;
    [SerializeField] float obstacleHp;
    [SerializeField] float obstacleGaugeIncrement;
    private float[] xPositions = new float[] { -5, 0, 5 };

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
                InstantiateObstacle(new Vector3(xPositions[Random.Range(0, 3)], 1.5f, Random.Range(-maxZ, maxZ)), newRoad);
            }
            else
            {
                InstantiateEnemy(new Vector3(Random.Range(-maxX, maxX), 1, Random.Range(-maxZ, maxZ)), newRoad);
            }
            
        }
    }

    #endregion

    #region Enemies
    /*private void InstantiateEnemies(GameObject parent, int nbEnemies)
    {
        float maxX = parent.transform.localScale.x * 5 - 1;
        float maxZ = parent.transform.localScale.z * 5 - 1;

        for(int i = 0; i < nbEnemies; i++) 
        {
             InstantiateEnemy(new Vector3(Random.Range(-maxX, maxX), 1, Random.Range(-maxZ, maxZ)), parent);
        }
    }*/


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
    /*private void InstantiateObstacles(GameObject parent, int nbObstacles)
    {
        float[] xPosition = new float[] { -3, 0, 3 };
        float maxZ = parent.transform.localScale.z * 5 - 1;

        for (int i = 0; i < nbObstacles; i++)
        {
            InstantiateObstacle(new Vector3(xPosition[Random.Range(0, 3)], 1.5f, Random.Range(-maxZ, maxZ)), parent);
        }
    }*/

    private void InstantiateObstacle(Vector3 position, GameObject parent)
    {
        //Instantiate the GameObject
        GameObject newObstacle = Instantiate(obstacleGO, parent.transform.position + position, Quaternion.identity);
        newObstacle.transform.SetParent(parent.transform);

        //Initialize the script Obstacle
        newObstacle.GetComponent<Obstacle>().Init(obstacleHp, obstacleSpeed, obstacleGaugeIncrement);
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
