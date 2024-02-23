using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestruction : MonoBehaviour
{
    [SerializeField] float lifeTime;

    void Start()
    {
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(this.gameObject);
    }
}
