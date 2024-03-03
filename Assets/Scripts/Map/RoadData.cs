using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoadData : ScriptableObject
{
    [Header("Road")]
    public GameObject road;

    [Header("Ennemies")]
    public int minEnemiesSpawn;
    public int maxEnemiesSpawn;

    [Header("Obstacles")]
    public int obstacleRandomMax;

    public int GetObstacleChance()
    {
        return Random.Range(0, obstacleRandomMax);
    }
}
