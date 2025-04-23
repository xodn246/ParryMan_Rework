using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_ParryHitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D enemy)
    {
        if (enemy.CompareTag("EnemyHurt"))
        {
            enemy.GetComponentInParent<Enemy_HealthManager>().Enemy_TakeDamage(1);
        }
        else if (enemy.CompareTag("BossHurt"))
        {
            enemy.GetComponentInParent<Boss_Health_Manager>().Boss_TakeDmage();
        }
    }
}