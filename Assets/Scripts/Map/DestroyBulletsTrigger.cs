using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBulletsTrigger : MonoBehaviour
{
    //Destroy bullets
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(other.gameObject);
        }
    }
}
