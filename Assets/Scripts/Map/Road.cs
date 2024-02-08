using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    private float movementSpeed;

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.Translate(-transform.forward * Time.deltaTime * movementSpeed);
    }

    public void ChangeSpeed(float speed)
    {
        movementSpeed = speed;
    }
}
