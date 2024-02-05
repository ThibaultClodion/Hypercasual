using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject road;
    [SerializeField] private List<Road> actualRoads;
    [SerializeField] private float roadSpeed;

    // Start is called before the first frame update
    void Start()
    {
        UpdateRoadsSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }

    //When it need a new road
    private void OnTriggerExit(Collider other)
    {
        CreateRoad(gameObject.transform.position);
    }
}
