using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Master_AxtraMissile_Hitbox : MonoBehaviour
{
    private Transform parentTransform;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("PlayerHurt"))
        {
            collider.transform.GetComponentInParent<Player_Health_Manager>().Player_Parry_Master_Missile(transform);
            collider.transform.GetComponentInParent<Player_Health_Manager>().Player_TakeDamage(parentTransform, 10, tag);
        }
    }

    public void Set_Parent_Transform(Transform enemyTransform)
    {
        parentTransform = enemyTransform;
    }
}
