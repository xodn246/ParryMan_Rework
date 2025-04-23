using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_TurretBullet_Manager : MonoBehaviour
{
    private float moveSpeed;

    private void FixedUpdate()
    {
        if (transform.localScale.x > 0) transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        else transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    public void SEt_BulletSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
