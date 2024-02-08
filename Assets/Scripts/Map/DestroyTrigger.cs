using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTrigger : MonoBehaviour
{
    //Destroy all nonuseful gameobjects
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Road")
        {
            Destroy(other.gameObject);
        }
    }

    //Destroy old roads
    private void OnTriggerExit(Collider other)
    {
        Destroy(other.gameObject);
    }
}
