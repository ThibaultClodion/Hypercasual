using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Roads")]
    [SerializeField] private GameObject road;
    [SerializeField] private GameObject initialRoad;
    private List<Road> actualRoads = new List<Road>();

    [Header("Spawn Datas")]
    [SerializeField] private int minEnemiesSpawn;
    [SerializeField] private int maxEnemiesSpawn;
    [SerializeField] private int minRoadHp;
    [SerializeField] private int maxRoadHp;    
    [SerializeField] private int minRoadSpeed;
    [SerializeField] private int maxRoadSpeed;
    private float roadSpeed;

    [Header("Enemies")]
    [SerializeField] GameObject enemyGO;
    [SerializeField] float enemySpeed;
    [SerializeField] int enemyGaugeIncrement;
    

    [Header("Obstacles")]
    [SerializeField] GameObject obstacleGO;
    [SerializeField] float obstacleSpeed;
    [SerializeField] int obstacleGaugeIncrement;
    private float[] xSpawnPositions = new float[] { -5, 0, 5 };

    [Header("Player")]
    [SerializeField] private GameObject playerGO;
    private PlayerController playerController;
    private int currentEarnGem = 0;
    private int currentEarnMoney = 0;

    [Header("Camera Data's")]
    [SerializeField] private FollowCharacter followCharacter;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    private float startTime;
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

    private void Update()
    {
        IncreaseScore();
    }

    #region GameManagement

    public void GameStart()
    {
        //Update the canvas
        startCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);

        //Update score and timer
        scoreText.text = "Score :" + score.ToString("F0");
        startTime = Time.realtimeSinceStartup;

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
        finalMoneyText.text = "Money " + currentEarnMoney.ToString("F0");
        finalGemText.text = "Gem " + currentEarnGem.ToString();
    }

    public void GameRestart()
    {
        //Update the canvas
        restartCanvas.gameObject.SetActive(false);
        startCanvas.gameObject.SetActive(true);

        //Add money and gems to the player
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") + currentEarnMoney);
        PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") + currentEarnGem);
        currentEarnGem = 0;
        currentEarnMoney = 0;

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

        //Update Total Highscore
        highscoreText.text = PlayerPrefs.GetFloat("Highscore").ToString("F0");

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
        GameObject newInitialRoad = Instantiate(initialRoad, new Vector3(0,0, initialRoad.transform.localScale.z*5 - 10), Quaternion.identity);
        actualRoads.Add(newInitialRoad.GetComponent<Road>());
        actualRoads.Add(newInitialRoad.GetComponent<Road>());
    }

    #endregion

    #region RoadManagement
    private void UpdateRoadsSpeed()
    {
        //If it grow to fast we can apply some function like SquareRoot ...
        roadSpeed =  minRoadSpeed + maxRoadSpeed * getTime() / 20 / 60;

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

        //Get the hp of the road
        float roadHp = getRoadHp();

        //Define the ennemy and obstacles
        int nbEnemiesSpawn = Random.Range(minEnemiesSpawn, (int) Mathf.Min(maxEnemiesSpawn,roadHp));
        float enemyHp = roadHp / nbEnemiesSpawn;
        float obstacleHp = roadHp * 10 / nbEnemiesSpawn;

        //Define the max X and Z position
        float maxX = newRoad.transform.localScale.x * 5 - 1;
        float maxZ = newRoad.transform.localScale.z * 5 - 1;
        
        //Instiante the entities
        for (int i = 0; i <  nbEnemiesSpawn; i++) 
        {
            if (Random.Range(0,20) == 0)
            {
                InstantiateObstacle(new Vector3(xSpawnPositions[Random.Range(0, 3)], obstacleGO.transform.position.y, Random.Range(-maxZ, maxZ)), newRoad, obstacleHp);
            }
            else
            {
                InstantiateEnemy(new Vector3(Random.Range(-maxX, maxX), enemyGO.transform.position.y, Random.Range(-maxZ, maxZ)), newRoad, enemyHp);
            }
            
        }
    }

    #endregion

    #region Score and Money
    public void IncreaseScore()
    {
        //Update the score
        score += roadSpeed * Time.deltaTime;

        //Update the text
        scoreText.text = "Score :" + score.ToString("F0");
    }

    public void AddAGem()
    {
        currentEarnGem++;
    }

    public void AddMoney(float money)
    {
        currentEarnMoney += (int)money;
    }
    #endregion

    #region Enemies
    private void InstantiateEnemy(Vector3 position, GameObject parent, float Hp)
    {
        //Instantiate the GameObject
        GameObject newEnnemy = Instantiate(enemyGO, parent.transform.position + position, Quaternion.identity);
        newEnnemy.transform.SetParent(parent.transform);

        //Instantiate the script Enemy
        newEnnemy.GetComponent<Enemy>().Init(Hp, enemySpeed, enemyGaugeIncrement, this);
    }
    #endregion

    #region Obstacles

    private void InstantiateObstacle(Vector3 position, GameObject parent, float Hp)
    {
        //Instantiate the GameObject
        GameObject newObstacle = Instantiate(obstacleGO, parent.transform.position + position, Quaternion.identity);
        newObstacle.transform.SetParent(parent.transform);

        int randomNum = Random.Range(0,11);

        //Random between Gauge and Gem Obstacle
        if (randomNum < 10)
        {
            //Initialize a Gauge Obstacle
            newObstacle.GetComponent<Obstacle>().InitCharacterObstacle(Hp, obstacleSpeed);
        }
        else if(randomNum == 10)
        {
            //Initialize a Gem Obstacle
            newObstacle.GetComponent<Obstacle>().InitGemObstacle(Hp, obstacleSpeed);
        }
    }
    #endregion

    #region Balancing
    private float getTime()
    {
        return Time.realtimeSinceStartup - startTime;
    }

    /*Previous Balance

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
    }*/

    private float getRoadHp()
    {
        //If it grow to fast we can apply some function like SquareRoot ...
        return minRoadHp + Mathf.Sqrt(maxRoadHp * getTime() / 20 / 60);
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
