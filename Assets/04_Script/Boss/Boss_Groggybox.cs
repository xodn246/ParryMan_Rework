using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Groggybox : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.name == "Player_Hurt")
        {
            if (player.GetComponentInParent<Player_Manager>().readyParry)
            {
                player.GetComponentInParent<Player_Health_Manager>().Player_TakeDamage(transform, 0, tag);
            }
        }
    }
}