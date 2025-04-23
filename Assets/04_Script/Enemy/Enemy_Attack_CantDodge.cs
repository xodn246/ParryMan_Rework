using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack_CantDodge : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player_Hurt")
        {
            other.transform.GetComponentInParent<Player_Health_Manager>().Player_TakeDamage_CantDodge(10);
        }
    }
}
