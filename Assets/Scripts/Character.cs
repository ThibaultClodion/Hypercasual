using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public void Move(Vector3 movePosition)
    {
        transform.position = Vector3.Lerp(transform.position, movePosition, Time.deltaTime);
    }
}
