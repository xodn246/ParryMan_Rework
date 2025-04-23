using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AttackGroundCheck : MonoBehaviour
{
    public LayerMask whatIsGround;
    public Transform attackGroundCheck;
    public float attackGroundCheckDistance;
    public float attackGroundCheckDistanceBool;

    public bool CheckAttackGroundBool()
    {
        return Physics2D.Raycast(attackGroundCheck.position, Vector2.down, attackGroundCheckDistanceBool, whatIsGround);
    }

    public Vector2 CheckAttackGround()
    {
        Vector2 point = Physics2D.Raycast(attackGroundCheck.position, Vector2.down, attackGroundCheckDistance, whatIsGround).point;
        return point;
    }
}
