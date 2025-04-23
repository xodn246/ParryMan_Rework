using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HealthManager : MonoBehaviour
{
    private Enemy_Manager manager;
    private Enemy_DronManager dManager;
    private Animator anim;
    private Rigidbody2D rigid;
    private Collider2D groundCheckBox;
    private GameObject enemyHurt;

    [Space(10f)]
    [SerializeField] private int maxHealth;

    private int currentHealth;

    [Space(10f)]
    [SerializeField] private bool dieDrop;

    [Space(5f)]
    [SerializeField] private Vector2 dieDir;

    [SerializeField] private float dieDarg;

    private bool isCheck = false;
    private bool isParry = false;

    [SerializeField] private bool isPuppet;
    [SerializeField] private bool isDron;

    // Start is called before the first frame update
    private void Start()
    {
        if (!isDron) manager = transform.GetComponent<Enemy_Manager>();
        else dManager = transform.GetComponent<Enemy_DronManager>();
        anim = transform.GetComponent<Animator>();
        rigid = transform.GetComponent<Rigidbody2D>();
        groundCheckBox = transform.GetComponent<BoxCollider2D>();
        enemyHurt = transform.Find("EnemyHurt").gameObject;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isPuppet || isDron)
        {
            if (currentHealth <= 0) anim.SetBool("isDie", true);
        }
        else
        {
            if (manager.haveArmor)
            {
                if (currentHealth == 1)
                {
                    manager.brokeArmor = true;
                }
            }

            if (dieDrop)
            {
                if (isCheck)
                {
                    if (manager.isDead && Check_Ground())
                    {
                        anim.SetBool("isDie", false);
                        anim.SetBool("isDieDrop", true);
                    }
                    if (manager.isDead && !Check_Ground()) rigid.drag = 1;
                }
            }
            else
            {
                if (manager.isDead) manager.Set_Drag(dieDarg);
            }
        }
    }

    public void Enemy_TakeDamage(int damage)
    {
        if (!isParry)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                if (!isDron) Enemy_Die();
                else Dron_Die();
            }
        }
    }

    public void Enemy_ParryProjectile(Transform projectile)
    {
        if (!isParry)
        {
            if (projectile.CompareTag("PlayerProjectile")) projectile.GetComponent<Enemy_projectile_Hitbox>().Set_Hit_Character();
            else if (projectile.CompareTag("Player_ProjectileDestroyDelaied")) projectile.GetComponent<Enemy_projectile_Hitbox>().Set_Hit_Delaied();
        }
    }

    private void Enemy_Die()
    {
        manager.isDead = true;

        //rigid.velocity = Vector2.zero;
        if (enemyHurt.GetComponent<CapsuleCollider2D>().enabled) enemyHurt.GetComponent<CapsuleCollider2D>().enabled = false;

        Vector2 setDieDir;
        if (transform.position.x > manager.target.transform.position.x) setDieDir = dieDir;
        else setDieDir = new Vector2(-dieDir.x, dieDir.y);

        rigid.AddForce(setDieDir, ForceMode2D.Impulse);
        anim.SetBool("isDie", true);
    }

    private void Dron_Die()
    {
        dManager.isDead = true;

        rigid.velocity = Vector2.zero;
        enemyHurt.SetActive(false);
    }

    private bool Check_Ground()
    {
        return Physics2D.BoxCast(groundCheckBox.bounds.center, groundCheckBox.bounds.size, 0f, Vector2.down, 0.1f, manager.whatIsGround);
    }

    public void Set_DieDrag()
    {
        manager.Set_Drag(dieDarg);
    }

    public void Start_Check_Ground()
    {
        isCheck = true;
    }

    public int CheckCurrentHP()
    {
        return currentHealth;
    }
}