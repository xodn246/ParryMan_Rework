using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack_Manager : MonoBehaviour
{
    private Enemy_Manager manager;
    private Rigidbody2D rigid;
    private Animator anim;

    [SerializeField] private float movePower;

    [Space(10f)]
    [Header("Attack Manage")]
    public int attackDamage;

    public Vector2 attackMovementDir;

    private float defaultDrag;
    [SerializeField] private float attackDrag;

    [SerializeField] private bool isPuppet = false;

    private int attackCounter = 0;

    [SerializeField] private bool canDestory = false;
    [SerializeField] private bool steadyAttack = false;


    [Space(10f)]
    [SerializeField] private Vector2 playerBounceDir;

    // Start is called before the first frame update
    private void Start()
    {
        manager = transform.GetComponent<Enemy_Manager>();
        rigid = transform.GetComponent<Rigidbody2D>();
        defaultDrag = rigid.drag;

        if (canDestory) anim = transform.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.name == "Player_Hurt")
        {
            player.GetComponentInParent<Player_Health_Manager>().Set_BounceDir(playerBounceDir);

            if (!steadyAttack)
            {
                if (attackCounter < 1)
                {
                    attackCounter++;
                    if (!isPuppet) player.GetComponentInParent<Player_Health_Manager>().Player_TakeDamage(gameObject.transform, attackDamage, tag);
                    else player.GetComponentInParent<Player_Health_Manager>().Player_TakeDamage(gameObject.transform, 0, tag);
                    if (canDestory) anim.SetTrigger("isDestroy");
                }
            }
            else
            {
                player.GetComponentInParent<Player_Health_Manager>().Player_TakeDamage(gameObject.transform, attackDamage, tag);
                if (canDestory) anim.SetTrigger("isDestroy");
            }
        }
    }

    public void Enemy_AttackMovement()
    {
        manager.Set_Drag(attackDrag);
        Vector2 setAttackDir;
        if (transform.position.x < manager.target.transform.position.x)
        {
            setAttackDir = attackMovementDir;
        }
        else
        {
            setAttackDir = new Vector2(-attackMovementDir.x, attackMovementDir.y);
        }
        rigid.AddForce(setAttackDir, ForceMode2D.Impulse);
    }

    public void Enemy_EndAttackMovement()
    {
        manager.Set_Drag(defaultDrag);
    }

    public void Enemy_Reset_AttackCount()
    {
        attackCounter = 0;
    }
}