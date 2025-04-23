using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Attack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player_Hurt")
        {
            collision.GetComponentInParent<Player_Health_Manager>().Player_TakeDamage(gameObject.transform, 1, tag);
        }
    }
}