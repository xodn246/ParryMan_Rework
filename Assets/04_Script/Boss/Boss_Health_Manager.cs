using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Health_Manager : MonoBehaviour
{
    private GameManager gameManager;
    private Animator anim;
    private Object_DamageFlash damageFlash;

    //[SerializeField] private Collider2D enemHurt;

    [SerializeField] private GameObject boss_healthbar;
    [SerializeField] private List<GameObject> healthBar;

    [Space(10f)]
    [SerializeField] private int maxHealth;

    public int currentHealth;

    [Space(10f)]
    public int groggyHealth;

    public int groggyHitCout;
    public int groggyHitCounter;

    public bool takedamage = false;
    public bool isGroggy = false;
    private bool isDefeat = false;

    [Space(10f)]
    [SerializeField] private bool canParry = false;
    [SerializeField] private float parryTime = 0f;
    [SerializeField] private float projectileParrySpeed;

    [HideInInspector] public bool activeParry = true;
    public bool doParry = false;
    private float parryTimer;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        anim = transform.GetComponent<Animator>();
        damageFlash = transform.GetComponent<Object_DamageFlash>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (canParry) parryTimer -= Time.deltaTime;

        if (parryTimer <= 0) activeParry = true;
        else activeParry = false;

        if (currentHealth <= 0)
        {
            anim.SetBool("isDefeat", true);
            isDefeat = true;
        }
    }

    public void Boss_TakeDmage()
    {
        if (!gameManager.PlayerDie)
        {
            if (canParry && parryTimer <= 0)
            {
                return;
            }
            else if (canParry && parryTimer > 0)
            {
                currentHealth--;
                StartCoroutine(Destroy_HP_Fill(currentHealth));
            }
            else
            {
                currentHealth--;
                StartCoroutine(Destroy_HP_Fill(currentHealth));
            }

            if (isGroggy) groggyHitCounter++;

            damageFlash.Call_DamageFlash();

            StartCoroutine(TakeDamage_Check());
        }

        else return;
    }

    public void Boss_Parry(Transform playerProjectile)
    {
        if (!canParry)
        {
            Debug.Log("패링 불가 적");
            if (playerProjectile.transform.CompareTag("PlayerProjectile")) playerProjectile.transform.GetComponent<Enemy_projectile_Hitbox>().Set_Hit_Character(); // 적 투사체 히트 처리
            Boss_TakeDmage();
        }
        else if (canParry && parryTimer > 0)
        {
            Debug.Log("Boss 패링 쿨타임");
            if (playerProjectile.transform.CompareTag("PlayerProjectile")) playerProjectile.transform.GetComponent<Enemy_projectile_Hitbox>().Set_Hit_Character(); // 적 투사체 히트 처리
            Boss_TakeDmage();
        }
        else if (!isGroggy && canParry && parryTimer <= 0)
        {
            Debug.Log("Boss 패링!!!");
            System_HitStop.instance.StartHitstop();
            parryTimer = parryTime;
            doParry = true;
            if (playerProjectile.transform.CompareTag("PlayerProjectile")) // 패링시 투사체 이동및 판정 변경
            {
                playerProjectile.transform.GetComponent<Enemy_projectile_Hitbox>().Set_EnemyProjectile();
                playerProjectile.transform.GetComponent<Enemy_projectile>().Set_Projectile_Move_Blue(projectileParrySpeed);
                playerProjectile.transform.GetComponent<Enemy_projectile>().Set_Projectile_EnemyTag();
            }
        }

    }

    public bool Boss_Defeat_Check()
    {
        return isDefeat;
    }

    public IEnumerator TakeDamage_Check()
    {
        takedamage = true;
        yield return new WaitForSeconds(0.05f);
        takedamage = false;
    }

    public IEnumerator Destroy_HP_Fill(int fillCount)
    {
        float timer = 0f;

        healthBar[fillCount].transform.GetComponent<Image>().color = new(1, 1, 1);

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 1f;
            Color setAlpha = new(1, 1, 1, Mathf.Lerp(1, 0, timer));
            healthBar[fillCount].transform.GetComponent<Image>().color = setAlpha;
        }

        healthBar[fillCount].SetActive(false);
    }

    public void Active_HealthBar()
    {
        boss_healthbar.SetActive(true);
    }

    public void Destroy_HealthBar()
    {
        boss_healthbar.SetActive(false);
    }

    public bool Return_ParryActive()
    {
        return activeParry;
    }

    public void Set_Parry_Activate()
    {
        parryTimer = 0;
    }

    public void Set_Parry_deActivate()
    {
        parryTimer = parryTime;
    }
}