using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Roads")]
    [SerializeField] private GameObject road;
    [SerializeField] private List<Road> actualRoads;
    [SerializeField] private float roadSpeed;

    [Header("Enemies")]
    [SerializeField] GameObject enemyGO;

    // Start is called before the first frame update
    void Start()
    {
        UpdateRoadsSpeed();
    }

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

        //Instantiate enemies (test)
        InstantiateEnemies(newRoad, 50);
    }

    private void InstantiateEnemies(GameObject parent, int nbEnemies)
    {
        float maxX = parent.transform.localScale.x * 5 - 1;
        float maxZ = parent.transform.localScale.z * 5 - 1;

        for(int i = 0; i < nbEnemies; i++) 
        {
            InstantiateEnemy(new Vector3(Random.Range(-maxX, maxX), 1, Random.Range(-maxZ, maxZ)), parent);
        }
    }

    private void InstantiateEnemy(Vector3 position, GameObject parent)
    {
        GameObject newEnnemy = Instantiate(enemyGO, parent.transform.position + position, Quaternion.identity);
        newEnnemy.transform.SetParent(parent.transform);
    }

    //When it need a new road
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Road")
        {
            CreateRoad(gameObject.transform.position);
        }
    }
}
