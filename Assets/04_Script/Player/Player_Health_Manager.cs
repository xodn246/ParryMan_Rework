using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;

public class Player_Health_Manager : MonoBehaviour
{
    private GameManager manager;
    private Player_Manager playerManager;
    private Object_SoundManager soundManager;

    private DataManager dataManager;
    private Animator anim;
    private Rigidbody2D rigid;
    private UI_ParryFailed failedUI;

    private Scene currentScene;

    [SerializeField] private GameObject playerHurt;
    [SerializeField] private float invincibleTime;
    private float invincibletimer;

    [SerializeField] private List<GameObject> parryVFX;
    [SerializeField] private GameObject parryImpactVFX;

    [Space(10f)]
    [SerializeField] private Transform PlayerCenterPos;
    [SerializeField] private List<GameObject> jumpParryVFX;

    [SerializeField] private GameObject jumpParryImpactVFX;
    [SerializeField] private Transform VFXPos;

    [Space(5f)]
    [SerializeField] private float projectileParrySpeed;

    [Space(5f)]
    [SerializeField] private Vector2 bounceDir;

    [Space(10f)]
    [SerializeField] private int maxHealth;

    private int currentHealth;

    private CinemachineVirtualCamera currentCam;

    private string currentHitObjectTag;

    private float hitDelay = 0.1f;
    private float hitTimer;

    //public bool doHitstop = false;

    private void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerManager = transform.GetComponent<Player_Manager>();
        soundManager = transform.GetComponent<Object_SoundManager>();

        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        anim = transform.GetComponent<Animator>();
        rigid = transform.GetComponent<Rigidbody2D>();
        failedUI = GameObject.FindObjectOfType<UI_ParryFailed>().GetComponent<UI_ParryFailed>();
        currentHealth = maxHealth;
        currentScene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        invincibletimer -= Time.deltaTime;
        hitTimer -= Time.deltaTime;

        if (!manager.PlayerDie && invincibletimer <= 0) playerHurt.SetActive(true);
    }

    // enemyPos : �и� ���� ������, damage : ������, tagName : �и� ���� ��¿� �ĺ���
    public void Player_TakeDamage(Transform enemyPos, int damage, string tagName)
    {
        currentHitObjectTag = tagName;

        if (!playerManager.readyParry)
        {
            if (hitTimer <= 0) currentHealth -= damage;
            Player_Die();
            hitTimer = hitDelay;
        }
        else
        {

            Do_Parry(enemyPos);
            playerManager.Set_SuccessParryCoolTime();
            Parry_Impact();

            if (currentHitObjectTag == "Enemy_BounceAttack" || currentHitObjectTag == "Enemy_PurpleProjectile")
            {
                SoundManager.instance.SFXPlayer(/*"Parry",*/ soundManager.Get_AudioClip("PurpleParry"), gameObject.transform);
            }
            else
            {
                SoundManager.instance.SFXPlayer(/*"Parry",*/ soundManager.Get_AudioClip("Parry"), gameObject.transform);
            }

            hitTimer = hitDelay;
        }
    }


    public void Player_TakeDamage_CantDodge(int damage)
    {
        currentHealth -= damage;
        Player_Die();
        hitTimer = hitDelay;
    }

    public void Do_Parry(Transform enemyPos)
    {
        dataManager.nowData.parryCount++;
        //if (dataManager.nowData.parryCount == 1) SteamAchievement.instance.Achieve("First_Parry");

        playerManager.End_PressParry_WhenParry();
        invincibletimer = invincibleTime;
        playerHurt.SetActive(false);

        System_HitStop.instance.StartHitstop();
        //StartCoroutine("Hitstop");

        if (enemyPos.position.x > transform.position.x) // ���� ��� �������� ���Ƽ��� �и�
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        int parryNum;
        if (!playerManager.isParry)
        {
            if (!playerManager.onAir)
            {
                playerManager.readyParry = false;
                playerManager.isParry = true;
                parryNum = Random.Range(0, parryVFX.Count);

                anim.SetBool("parryReady", false);

                if (enemyPos.transform.CompareTag("Enemy_BounceAttack") || enemyPos.transform.CompareTag("Enemy_PurpleProjectile"))
                {
                    playerManager.isParry = false;
                    playerManager.isJumpParry = true;
                    anim.SetBool("isJumpParry", true);
                    anim.SetBool("isJump", true);
                    rigid.velocity = new(rigid.velocity.x, 0);
                    rigid.AddForce(bounceDir, ForceMode2D.Force);
                }
                else
                {
                    anim.SetBool("isParry", true);
                }

                //VFX ���
                GameObject VFX = Instantiate(parryVFX[parryNum], VFXPos.position, Quaternion.identity);
                VFX.transform.localScale = transform.localScale;
                GameObject HitVFX = Instantiate(parryImpactVFX, VFXPos.position, Quaternion.identity);
                HitVFX.transform.localScale = transform.localScale;

                anim.SetFloat("parryNum", parryNum);
            }
            else
            {
                playerManager.readyParry = false;
                playerManager.isJumpParry = true;
                parryNum = Random.Range(0, jumpParryVFX.Count);
                anim.SetBool("isParry", true);
                anim.SetBool("jumpReady", false);

                //Debug.Log(enemyPos.transform.gameObject.tag);
                if (enemyPos.transform.CompareTag("Enemy_BounceAttack") || enemyPos.transform.CompareTag("Enemy_PurpleProjectile"))
                {
                    rigid.velocity = new(rigid.velocity.x, 0);
                    rigid.AddForce(bounceDir, ForceMode2D.Force);
                }

                //VFX ��¿� ���� ���
                Vector2 direction = enemyPos.position - PlayerCenterPos.position;

                //VFX ���
                GameObject VFX = Instantiate(jumpParryVFX[parryNum], VFXPos.position, Quaternion.identity);
                VFX.transform.localScale = transform.localScale;

                //�����и� ����� ����Ʈ ���
                if (direction.normalized.x <= 0)
                {
                    //Debug.Log("�����и� ���1");
                    float angle = Vector2.Angle(Vector2.up, direction);
                    GameObject VFXimpact = Instantiate(jumpParryImpactVFX, VFXPos.position, Quaternion.Euler(0, 0, angle + 180f));
                    VFX.transform.localScale = new(1, 1, 1);
                }
                else
                {
                    //Debug.Log("�����и� ���2");
                    float angle = Vector2.Angle(Vector2.down, direction);
                    GameObject VFXimpact = Instantiate(jumpParryImpactVFX, VFXPos.position, Quaternion.Euler(0, 0, angle));
                    VFX.transform.localScale = new(-1, 1, 1);
                }
                //Instantiate(jumpParryImpactVFX, VFXPos.position, Quaternion.Euler(0, 0, 90 - Vector2.Angle(Vector2.right, direction)));

                anim.SetFloat("jumpParryNum", parryNum);
            }
        }
    }

    public void Player_Parry_EnemyProjectile_Skyblue(Transform enemyProjectile)
    {
        if (!playerManager.readyParry && !playerManager.isParry && !playerManager.isJumpParry)
        {
            Debug.Log("�Ķ� �и� ����");
            if (enemyProjectile.transform.CompareTag("EnemyProjectile")) enemyProjectile.transform.GetComponent<Enemy_projectile_Hitbox>().Set_Hit_Character(); // �� ����ü ��Ʈ ó��
            else if (enemyProjectile.transform.CompareTag("Enemy_ProjectileDestroyDelaied")) enemyProjectile.transform.GetComponent<Enemy_projectile_Hitbox>().Set_Hit_Delaied(); // �� ����ü ��Ʈ ������(�̹����� ������ ������)
        }
        else if (playerManager.readyParry)
        {
            Debug.Log("�Ķ� �и� ����");
            if (enemyProjectile.transform.CompareTag("EnemyProjectile") || enemyProjectile.transform.CompareTag("Enemy_ProjectileDestroyDelaied")) // �и��� ����ü �̵��� ���� ����
            {
                enemyProjectile.transform.GetComponent<Enemy_projectile_Hitbox>().Set_PlayerProjectile();
                enemyProjectile.transform.GetComponent<Enemy_projectile>().Set_Projectile_Move_Blue(projectileParrySpeed);
                enemyProjectile.transform.GetComponent<Enemy_projectile>().Set_Projectile_PlayerTag();
            }
        }
    }

    public void Player_Parry_EnemyProjectile_Yellow(Transform enemyProjectile)
    {
        if (!playerManager.readyParry && !playerManager.isParry && !playerManager.isJumpParry)
        {
            Debug.Log("��� �и� ����");
            if (enemyProjectile.transform.CompareTag("EnemyProjectile")) enemyProjectile.transform.GetComponent<Enemy_projectile_Hitbox>().Set_Hit_Character(); // �� ����ü ��Ʈ ó��
            else if (enemyProjectile.transform.CompareTag("Enemy_ProjectileDestroyDelaied")) enemyProjectile.transform.GetComponent<Enemy_projectile_Hitbox>().Set_Hit_Delaied();
        }
        else if (playerManager.readyParry)
        {
            Debug.Log("��� �и� ����");
            if (enemyProjectile.transform.CompareTag("EnemyProjectile")) // �и��� ����ü �̵��� ���� ����
            {
                if (enemyProjectile.transform.GetComponent<Enemy_projectile>().Check_Trail()) enemyProjectile.transform.GetComponentInChildren<TrailRenderer>().enabled = false;
                enemyProjectile.transform.GetComponent<Enemy_projectile_Hitbox>().Set_DisableHitbox();
                enemyProjectile.transform.GetComponent<Enemy_projectile>().Set_Projectile_Move_Yellow(transform.localScale.x);
            }
            else if (enemyProjectile.transform.CompareTag("Enemy_ProjectileDestroyDelaied"))
            {
                enemyProjectile.transform.GetComponent<Enemy_projectile_Hitbox>().Set_Hit_Delaied();
            }
            else Debug.Log("�̰ǹ���");
        }
    }

    public void Player_Parry_Master_Missile(Transform masterProjectile)
    {
        if (!playerManager.readyParry && !playerManager.isParry && !playerManager.isJumpParry)
        {
            return; //��Ʈó�� ���� �� �ʿ� ��� ���� ���ذ�
        }
        else if (playerManager.readyParry)
        {
            masterProjectile.transform.GetComponent<Boss_Master_MissileProjectile>().Set_Missile_Parry_Move(transform.localScale.x);
            masterProjectile.transform.GetComponent<Boss_Master_MissileProjectile>().Set_DisableHitbox();
        }
    }

    public void End_Parry()
    {
        playerManager.isParry = false;
        playerManager.isJumpParry = false;
        anim.SetBool("isParry", false);
        anim.SetBool("isJumpParry", false);
    }

    public void Player_Die()
    {
        dataManager.nowData.deathCount++;
        if (currentHealth <= 0)
        {
            SoundManager.instance.SFXPlayer(/*"Die",*/ soundManager.Get_AudioClip("Die"), gameObject.transform);

            rigid.velocity = Vector2.zero;
            playerHurt.SetActive(false);

            anim.SetBool("isDie", true);
            Print_ParryFailedUI();
            manager.PlayerDie = true;
        }
        else
        {
            SoundManager.instance.SFXPlayer(/*"Damaged",*/ soundManager.Get_AudioClip("Damage"), gameObject.transform);
        }
    }

    public void Print_ParryFailedUI()
    {
        if (currentScene.name == "Bamboo_Boss") failedUI.Boss01_Failed();
        else if (currentScene.name == "Sakura_Boss") failedUI.Boss02_Failed();
        else if (currentScene.name == "Beach_Boss") failedUI.Boss03_Failed();
        else if (currentScene.name == "Hidden_Boss") failedUI.Boss04_Failed();
        else failedUI.Normal_Failed();
    }

    private void Parry_Impact()
    {
        currentCam.m_Lens.OrthographicSize = 17.5f;
        StartCoroutine(Reset_Lens());
    }

    private IEnumerator Reset_Lens()
    {
        float timer = 0;

        while (timer <= 1)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            currentCam.m_Lens.OrthographicSize = Mathf.Lerp(17.5f, 18, timer);
        }
    }

    public void Set_CurrentCam(CinemachineVirtualCamera setCamera)
    {
        currentCam = setCamera;
    }

    public CinemachineVirtualCamera Get_CurrentCam()
    {
        return currentCam;
    }

    public void Set_InvincibleTime_Zero() // ���� �и� ���� ���� �ݶ��̴� �츮��
    {
        if (!manager.PlayerDie) invincibletimer = 0;
    }

    public void Set_BounceDir(Vector2 dir)
    {
        bounceDir = dir;
    }

    public string Get_CurrentHitObcjetTag()
    {
        return currentHitObjectTag;
    }

    // private IEnumerator Hitstop()
    // {
    //     Debug.Log("��Ʈ��ž");

    //     doHitstop = true;
    //     Time.timeScale = 0f;

    //     float timer = 0f;

    //     while (timer <= 1f)
    //     {
    //         yield return null;

    //         timer += Time.unscaledDeltaTime * 4f;
    //         Time.timeScale = Mathf.Lerp(0, 1, timer);
    //     }

    //     doHitstop = false;
    // }

    // public void StopHitstop()
    // {
    //     StopCoroutine("Hitstop");
    // }
}