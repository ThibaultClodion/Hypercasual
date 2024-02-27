using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Roads")]
    [SerializeField] private GameObject road;
    private List<Road> actualRoads = new List<Road>();
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
    [SerializeField] private GameObject playerGO;
    private PlayerController playerController;
    private int currentEarnGem = 0;

    [Header("Camera Data's")]
    [SerializeField] private FollowCharacter followCharacter;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    private float score = 0;

    [Header("Canvas")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject restartCanvas;
    [SerializeField] private MoneyText[] moneyText;
    [SerializeField] private GemText[] gemsText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalMoneyText;
    [SerializeField] private TextMeshProUGUI finalGemText;

    void Awake()
    {
        PlayerPrefs.SetInt("HasPlayed", 0);

        //Check if the player already play the game, if not initialize his datas
        if (PlayerPrefs.GetInt("HasPlayed", 0) == 0)
        {
            //Set first time opening to false
            PlayerPrefs.SetInt("HasPlayed", 1);

            //Economy
            PlayerPrefs.SetInt("Gems", 0);
            PlayerPrefs.SetInt("Money", 0);

            //Weapons
            PlayerPrefs.SetInt("Upgrade_redWeaponIndex", 0);
            PlayerPrefs.SetInt("Upgrade_greenWeaponIndex", 0);
            PlayerPrefs.SetInt("Upgrade_yellowWeaponIndex", 0);

            //Upgrades
            PlayerPrefs.SetInt("Upgrade_nbStartCharacter", 1);
            PlayerPrefs.SetInt("Upgrade_nbStartCharacterIndex", 0);
            PlayerPrefs.SetFloat("Upgrade_fireRateMultiply", 1f);
            PlayerPrefs.SetInt("Upgrade_fireRateMultiplyIndex", 0);
            PlayerPrefs.SetFloat("Upgrade_bulletSpeedMultiply", 1f);
            PlayerPrefs.SetInt("Upgrade_bulletSpeedMultiplyIndex", 0);
            PlayerPrefs.SetFloat("Upgrade_bulletDamageMultiply", 1f);
            PlayerPrefs.SetInt("Upgrade_bulletDamageMultiplyIndex", 0);
            PlayerPrefs.SetFloat("Upgrade_bulletRangeMultiply", 1f);
            PlayerPrefs.SetInt("Upgrade_bulletRangeMultiplyIndex", 0);
            PlayerPrefs.SetFloat("Upgrade_moveSpeedMultiply", 1f);
            PlayerPrefs.SetInt("Upgrade_moveSpeedMultiplyIndex", 0);
            PlayerPrefs.SetFloat("Upgrade_luckMultiply", 1f);
            PlayerPrefs.SetInt("Upgrade_luckMultiplyIndex", 0);
            PlayerPrefs.SetFloat("Upgrade_scoreMultiply", 1f);
            PlayerPrefs.SetInt("Upgrade_scoreMultiplyIndex", 0);

            //High Score
            PlayerPrefs.SetFloat("Highscore", 0f);
        }

        //Test purposes
        PlayerPrefs.SetInt("Gems", 110);
        PlayerPrefs.SetInt("Money", 1400);

        //The beginning is like restarting the game
        GameRestart();
    }

    #region GameManagement

    public void GameStart()
    {
        //Update the canvas
        startCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);

        //Update score
        scoreText.text = "Score :" + score.ToString("F0");

        //Make the roads move
        UpdateRoadsSpeed();

        //Destroy the old Player
        if(playerController != null)
        {
            Destroy(playerController.gameObject);
        }

        //Init the Player
        GameObject player = Instantiate(playerGO);
        playerController = player.GetComponent<PlayerController>();
        playerController.StartToPlay(followCharacter);
    }

    public void GameOver()
    {
        //Update the canvas
        gameCanvas.gameObject.SetActive(false);
        restartCanvas.gameObject.SetActive(true);

        //Update final Score, Money and Gems Text
        finalScoreText.text = "Score " + score.ToString("F0");
        finalMoneyText.text = "Money " + score.ToString("F0");
        finalGemText.text = "Gem " + currentEarnGem.ToString();
    }

    public void GameRestart()
    {
        //Update the canvas
        restartCanvas.gameObject.SetActive(false);
        startCanvas.gameObject.SetActive(true);

        //Add money and gems to the player
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") + (int)score);
        PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") + currentEarnGem);
        currentEarnGem = 0;

        //Update Total Highscore
        highscoreText.text = PlayerPrefs.GetFloat("Highscore").ToString("F0");

        //Update Money Text
        foreach(MoneyText text in moneyText) 
        {
            text.UpdateText();
        }       
        
        //Update Gem Text
        foreach(GemText text in gemsText) 
        {
            text.UpdateText();
        }

        //Reset score
        if (score > PlayerPrefs.GetFloat("Highscore"))
        {
            PlayerPrefs.SetFloat("Highscore", score);
        }

        score = 0;

        //Destroy the old roads
        foreach (Road road in actualRoads)
        {
            if(road != null) 
            {
                road.Destroy();
            }
        }
        actualRoads.Clear();

        //Instantiate the initial road
        GameObject initialRoad = Instantiate(road, new Vector3(0,0,transform.position.z/6), Quaternion.identity);
        actualRoads.Add(initialRoad.GetComponent<Road>());
        actualRoads.Add(initialRoad.GetComponent<Road>());
    }

    #endregion

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

    #region Score
    public void IncreaseScore(float augmentation)
    {
        //Update the score
        score += (augmentation * PlayerPrefs.GetFloat("Upgrade_scoreMultiply"));

        //Update the text
        scoreText.text = "Score :" + score.ToString("F0");
    }

    public void AddAGem()
    {
        currentEarnGem++;
    }
    #endregion

    #region Enemies
    private void InstantiateEnemy(Vector3 position, GameObject parent)
    {
        //Instantiate the GameObject
        GameObject newEnnemy = Instantiate(enemyGO, parent.transform.position + position, Quaternion.identity);
        newEnnemy.transform.SetParent(parent.transform);

        //Instantiate the script Enemy
        newEnnemy.GetComponent<Enemy>().Init(enemyHp, enemySpeed, enemyGaugeIncrement, this);
    }
    #endregion

    #region Obstacles

    private void InstantiateObstacle(Vector3 position, GameObject parent)
    {
        //Instantiate the GameObject
        GameObject newObstacle = Instantiate(obstacleGO, parent.transform.position + position, Quaternion.identity);
        newObstacle.transform.SetParent(parent.transform);

        int randomNum = Random.Range(0,11);

        //Random between Gauge and Gem Obstacle
        if (randomNum < 10)
        {
            //Initialize a Gauge Obstacle
            newObstacle.GetComponent<Obstacle>().InitCharacterObstacle(obstacleHp, obstacleSpeed);
        }
        else if(randomNum == 10)
        {
            //Initialize a Gem Obstacle
            newObstacle.GetComponent<Obstacle>().InitGemObstacle(obstacleHp, obstacleSpeed);
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
