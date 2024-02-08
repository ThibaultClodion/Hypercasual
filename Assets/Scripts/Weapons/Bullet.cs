using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float moveSpeed = 15.0f;
    private float damage = 5f;

    private void FixedUpdate()
    {
        Move(new Vector3 (0, 0, 1));
    }

    private void Move(Vector3 movePosition)
    {
        transform.Translate(movePosition * moveSpeed * Time.deltaTime);
    }

    public float Damage()
    {
        return damage;
    }
}
