using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    private float movementSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-transform.forward * Time.deltaTime * movementSpeed);
    }

    public void ChangeSpeed(float speed)
    {
        movementSpeed = speed;
    }
}
