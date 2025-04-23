using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DronManager : MonoBehaviour
{
    private enum State
    {
        Spawn,
        Idle,
        Trace,
        Attack,
        AttackStop,
        Die
    }


    private GameObject target;

    [SerializeField] private Object_SoundManager soundManager;

    [SerializeField] private State currentState;
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject dronBarrel;
    [SerializeField] private Transform projectilePos;

    [Space(10f)]
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject attackVFX;

    [Space(10f)]
    [Header("Moveparameter")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedLerpTime;
    [SerializeField] private float upToTarget;
    [SerializeField] private float attackDrag;
    [SerializeField] private float minTrace;


    [Space(10f)]
    [SerializeField] private float targetDistance = 0;

    [Space(10f)]
    [SerializeField] private float attackDistace = 0;
    [SerializeField] private float startAttackDelay = 0;
    [SerializeField] private float attackDelay = 0;
    [SerializeField] private float attackMoveStopTime = 0;

    [Space(10f)]
    [SerializeField] private GameObject dronSprite;
    [SerializeField] private GameObject destroyVFX;
    [SerializeField] private Transform destroyVFXPos;

    private int attackCount = 0;
    private int alertCount = 0;
    private float attackStartDelayTimer = 0;
    private float attackTimer = 0;
    private float attackMoveStopTimer = 0;

    private float currentSpeed;


    private bool doSpawn = true;

    public bool isDead = false;
    private bool dieEnd = false;

    void Start()
    {
        attackStartDelayTimer = startAttackDelay;
    }

    private void Update()
    {
        if (attackTimer >= 0) attackTimer -= Time.deltaTime;
        if (attackMoveStopTimer >= 0) attackMoveStopTimer -= Time.deltaTime;
        if (attackStartDelayTimer >= 0) attackStartDelayTimer -= Time.deltaTime;

        if (target == null) target = GameObject.Find("Player(Clone)").transform.Find("TargetCenter").gameObject; //테스트씬 테스트용 코드

        if (!doSpawn) DronBaller_Look_Player(); // 추적 상태 들어갔을때부터 활성화 시킬것
        else return;

        if (Input.GetKeyDown(KeyCode.K)) currentState = State.Attack;
        if (Input.GetKeyDown(KeyCode.J)) currentState = State.Trace;
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Spawn:
                Spawn();
                if (!doSpawn)
                {
                    anim.SetBool("isSpawn", false);
                    anim.SetBool("isIdle", true);

                    currentState = State.Idle;
                }

                if (isDead) currentState = State.Die;
                break;

            case State.Idle:
                Idle();
                // 인지 범위 내로 들어오면 추적 시작
                if (CheckDistance() <= targetDistance)
                {
                    currentState = State.Trace;
                }

                if (isDead) currentState = State.Die;
                break;

            case State.Trace:
                Trace();
                if (CheckDistance() <= attackDistace && attackStartDelayTimer < 0 && attackTimer < 0)
                {
                    currentState = State.Attack;
                }

                if (isDead) currentState = State.Die;
                break;

            case State.Attack:
                Attack();
                if (isDead) currentState = State.Die;
                break;

            case State.AttackStop:
                AttackStop();
                if (attackMoveStopTimer <= 0)
                {
                    currentState = State.Trace;
                    attackTimer = attackDelay;
                    attackCount = 0;
                    alertCount = 0;
                }

                if (isDead) currentState = State.Die;
                break;

            case State.Die:
                Die();
                break;
        }
    }

    private void DronBaller_Look_Player()
    {
        Vector2 direction = target.transform.position - gameObject.transform.position;
        float angle;
        if (direction.normalized.x <= 0)
        {
            angle = Vector2.Angle(Vector2.up, direction);
            transform.localScale = new(1, 1, 1);
            dronBarrel.transform.rotation = Quaternion.Euler(0, 0, angle - 110f);
        }
        else
        {
            angle = Vector2.Angle(Vector2.down, direction);
            transform.localScale = new(-1, 1, 1);
            dronBarrel.transform.rotation = Quaternion.Euler(0, 0, angle - 70f);
        }
    }

    #region Check

    public float CheckDistance()
    {
        return Vector2.Distance(target.transform.position, transform.position);
    }
    #endregion Check

    #region State

    private void Spawn()
    {
        Set_Movespeed(0f);
    }

    private void Idle()
    {
        Set_Movespeed(0f);
    }

    private void Trace()
    {
        rigid.drag = 0;
        Vector2 traceDir = target.transform.position - transform.position;
        Set_Movespeed(maxSpeed);
        //LookAtPlayer();

        if (CheckDistance() > minTrace) rigid.AddForce(traceDir.normalized * currentSpeed, ForceMode2D.Force);
        else rigid.AddForce(-traceDir.normalized * currentSpeed * 0.5f, ForceMode2D.Force);

        if (transform.position.y < target.transform.position.y + upToTarget) rigid.AddForce(new Vector2(traceDir.x, 80f).normalized * 0.8f, ForceMode2D.Impulse);
        LimitSpeed();
    }

    private void LimitSpeed()
    {
        if (rigid.velocity.x >= maxSpeed) rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        if (rigid.velocity.x <= -maxSpeed) rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
        if (rigid.velocity.y >= maxSpeed) rigid.velocity = new Vector2(rigid.velocity.x, maxSpeed);
        if (rigid.velocity.y <= -maxSpeed) rigid.velocity = new Vector2(rigid.velocity.x, -maxSpeed);
    }

    private void Attack()
    {
        if (alertCount == 0)
        {
            alertCount++;
            transform.GetComponent<Enemy_Attack_Alert>().Print_Alert_Sky();
        }

        Set_Movespeed(0f);
        rigid.drag = attackDrag;

        StartCoroutine(SpawnProjectile());
    }

    private void AttackStop()
    {
        Set_Movespeed(0f);
    }

    private IEnumerator SpawnProjectile()
    {
        yield return new WaitForSeconds(0.5f);

        if (attackCount > 0) EndAttack();
        else
        {

            Vector2 direction = target.transform.position - gameObject.transform.position;
            GameObject pj = Instantiate(projectile, projectilePos.position, Quaternion.identity);
            GameObject vfx = Instantiate(attackVFX, projectilePos.position, Quaternion.identity);

            pj.transform.localScale = transform.localScale;
            pj.transform.GetComponent<Enemy_projectile_Hitbox>().Set_Parent_Transform(transform);

            vfx.transform.localScale = transform.localScale;

            float angle;
            if (direction.normalized.x <= 0)
            {
                angle = Vector2.Angle(Vector2.up, direction);
                transform.localScale = new(1, 1, 1);
                vfx.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }
            else
            {
                angle = Vector2.Angle(Vector2.down, direction);
                transform.localScale = new(-1, 1, 1);
                vfx.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }

            SoundManager.instance.SFXPlayer(soundManager.Get_AudioClip("Shot"), gameObject.transform);

            attackCount++;
        }
    }

    public void EndAttack()
    {
        attackMoveStopTimer = attackMoveStopTime;
        currentState = State.AttackStop;
    }

    private void Die()
    {
        Set_Movespeed(0f);

        if (!dieEnd)
        {
            dieEnd = true;
            dronSprite.SetActive(false);
            Vector2 direction = target.transform.position - gameObject.transform.position;
            GameObject dieVfx = Instantiate(destroyVFX, projectilePos.position, Quaternion.identity);

            dieVfx.transform.localScale = transform.localScale;

            float angle;
            if (direction.normalized.x <= 0)
            {
                angle = Vector2.Angle(Vector2.up, direction);
                transform.localScale = new(1, 1, 1);
                dieVfx.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }
            else
            {
                angle = Vector2.Angle(Vector2.down, direction);
                transform.localScale = new(-1, 1, 1);
                dieVfx.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }
        }



        Destroy(gameObject);
    }

    #endregion State



    public void Set_Movespeed(float speed)
    {
        currentSpeed = Mathf.Lerp(currentSpeed, speed, speedLerpTime);
    }

    public void Dron_SpawnEnd()
    {
        doSpawn = false;
    }
}
