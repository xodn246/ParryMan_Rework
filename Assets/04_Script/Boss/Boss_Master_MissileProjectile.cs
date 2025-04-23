using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Boss_Master_MissileProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private Animator anim;
    [SerializeField] private Enemy_AttackGroundCheck groundCheck;
    [SerializeField] private BoxCollider2D hitbox;
    [SerializeField] private Transform rayPos;
    [SerializeField] private Enemy_Projectile_Spin projectileSpin;

    [Space(10f)]
    [SerializeField] private GameObject groundVFX;

    [Space(10f)]
    [SerializeField] private float explosionTurm;
    [SerializeField] private float parryDestroyTurm;
    private float explosionTimer;
    private float destroyTimer;

    private bool printGroundVFX = false;
    private bool isLand = false;
    private bool isParried = false;
    private bool explosionTrigger = false;
    private bool destroyTrigger = false;

    [Space(10f)]
    [Header("ShockStatus")]
    [SerializeField] private Transform shockPos_front;
    [SerializeField] private Transform shockPos_back;
    [SerializeField] private GameObject shockPrefab;
    [SerializeField] private int shockCount;
    [SerializeField] private float shockTurm;
    [SerializeField] private float shockDistance;
    [SerializeField] private Object_SoundManager soundManager;


    private void Awake()
    {
        explosionTimer = explosionTurm;
        destroyTimer = parryDestroyTurm;
    }

    private void Update()
    {
        if (isParried) destroyTimer -= Time.deltaTime;
        if (isLand) explosionTimer -= Time.deltaTime;

        if (!destroyTrigger && destroyTimer <= 0)
        {
            destroyTrigger = true;
            projectileSpin.Set_SpinSpeed(0);
            rigid.gravityScale = 0;
            rigid.velocity = Vector2.zero;
            anim.SetBool("isDestroy", true);
        }

        if (!explosionTrigger && explosionTimer <= 0)
        {
            explosionTrigger = true;
            anim.SetBool("isExplosion", true);
        }
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(rayPos.position, Vector2.down, Color.blue, 2f);
        if (groundCheck.CheckAttackGroundBool())
        {
            if (!isLand)
            {
                isLand = true;
                SoundManager.instance.SFXPlayer(soundManager.Get_AudioClip("Land"), gameObject.transform);
                anim.SetBool("isLand", true);
                rigid.velocity = Vector2.zero;
                rigid.gravityScale = 0;
                transform.position = groundCheck.CheckAttackGround();
                Set_DisableHitbox();
            }

            if (!printGroundVFX)
            {
                printGroundVFX = true;
                GameObject goundShockVFX = Instantiate(groundVFX, groundCheck.CheckAttackGround(), Quaternion.identity);
            }
            //바닥에 박히는 애니메이션 출력 및 히트박스 끄기
            //속도 0, 중력 0 설정
        }
    }

    public void Set_Missile_Parry_Move(float playerScale)
    {

        isParried = true;
        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 5f;
        anim.SetBool("isParried", true);

        Vector2 randDir;

        if (playerScale > 0)
        {
            randDir = new(Random.Range(30f, 40f), Random.Range(40f, 50f));
            rigid.AddForce(randDir, ForceMode2D.Impulse);
            projectileSpin.Set_SpinSpeed(2000f);
        }
        else
        {
            randDir = new(Random.Range(-30f, -40f), Random.Range(40f, 50f));
            rigid.AddForce(randDir, ForceMode2D.Impulse);
            projectileSpin.Set_SpinSpeed(-2000f);
        }
    }

    public void Set_DisableHitbox()
    {
        hitbox.enabled = false;
    }

    public IEnumerator ShockWave()
    {
        for (int i = 0; i < shockCount; i++)
        {
            Debug.Log("쇼크웨이브 출력");
            yield return new WaitForSeconds(shockTurm);
            Debug.Log(shockCount);

            float resultFrontX;
            float resultBackX;
            if (transform.localScale.x > 0)
            {
                resultFrontX = shockPos_front.position.x + i * shockDistance;
                resultBackX = shockPos_back.position.x - i * shockDistance;
            }
            else
            {
                resultFrontX = shockPos_front.position.x - i * shockDistance;
                resultBackX = shockPos_back.position.x + i * shockDistance;
            }


            Vector3 resultFrontPos = new(resultFrontX, shockPos_front.position.y, shockPos_front.position.z);
            Vector3 resultBackPos = new(resultBackX, shockPos_back.position.y, shockPos_back.position.z);

            Instantiate(shockPrefab, resultFrontPos, Quaternion.identity);
            Instantiate(shockPrefab, resultBackPos, Quaternion.identity);

        }
    }

    public void Boss04_Start_Shockwave()
    {
        StartCoroutine(ShockWave());
    }
}
