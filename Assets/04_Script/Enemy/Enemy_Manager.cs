using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy_Manager : MonoBehaviour
{
    private enum Attack_Type
    {
        Melee,
        Range,
        Inplace,
        RangeInplace,
        Patrol,
        Teleport
    }

    private enum State
    {
        Spawn,
        Idle,
        Patrol,
        Trace,
        BattleIdle,
        Attack,
        Die
    }

    private Rigidbody2D rigid;
    private Animator anim;
    private Enemy_HealthManager health;
    private Enemy_AttackGroundCheck attackGroundCheck;

    [Header("Attack_Type")]
    [SerializeField] private Attack_Type attackType;

    [Space(10f)]
    [Header("Armor")]
    public bool haveArmor;

    [HideInInspector] public bool brokeArmor = false;
    private bool brokeCheck = false;
    [HideInInspector] public Animator armorAnim;

    [Space(10f)]
    [Header("Check")]
    public GameObject target;

    [Space(10f)]
    public LayerMask whatIsGround;

    public LayerMask whatIsTarget;

    [Space(10f)]
    public Transform ledgeCheck;

    public float ledgeCheckDistance;
    public Transform wallCheck;
    public float wallCheckDistance;

    private bool doSpawn = true;

    //public bool doAttack = false;       // 공격중인지 체크(AttackMovement 제어용)
    public bool isDead = false;

    [Space(10f)]
    public Transform targetDistanceCheck;

    public float targetCheckDistance;

    [Space(10f)]
    public float minTraceDistance;

    public float maxTraceDistance;
    public float attackDistace;
    public float attackDistanceY;

    [Space(10f)]
    [Header("Movement Control")]
    public float moveSpeed;

    private float currentSpeed;
    private bool attackReady = false;

    [Space(10f)]
    private int isRight = -1;

    [Space(10f)]
    [SerializeField] private State currentState;

    [Space(10)]
    [Header("Timer")]
    public float patrolTime;

    public float stopTime;
    public float attackTime;
    public float stunTime;
    public float checkTime;
    public float battleTime;
    public float waitTraceTime;

    private float patrolTimer;
    private float idleTimer;
    private float attackTimer;
    private float checkTimer;
    [SerializeField] private float battleTimer;
    private float waitTraceTimer;

    private void Start()
    {
        currentState = State.Spawn;
        //target = GameObject.Find("Player(Clone)").gameObject;
        rigid = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        health = gameObject.GetComponent<Enemy_HealthManager>();
        if (haveArmor) armorAnim = gameObject.transform.Find("Enemy_Armor").GetComponent<Animator>();
        if (attackType == Attack_Type.Teleport) attackGroundCheck = transform.GetComponent<Enemy_AttackGroundCheck>();
    }

    private void Update()
    {
        if (target == null) target = GameObject.Find("Player(Clone)").gameObject; //테스트씬 테스트용 코드

        patrolTimer -= Time.deltaTime;
        idleTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;
        checkTimer -= Time.deltaTime;
        battleTimer -= Time.deltaTime;
        waitTraceTimer -= Time.deltaTime;

        if (brokeArmor && !brokeCheck)
        {
            brokeCheck = true;
            Destroy(armorAnim.transform.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (CheckWall() || !CheckLedge())
        {
            if (!isDead)
            {
                rigid.velocity = Vector2.zero;
            }
            else
            {
                Set_Drag(1f);
                rigid.gravityScale = 5f;
            }
        }

        //if (isDead && CheckWall()) Set_Drag(10f);

        switch (currentState)
        {
            case State.Spawn:
                Spawn();
                if (!doSpawn)
                {
                    anim.SetBool("isSpawn", false);
                    anim.SetBool("isIdle", true);

                    if (haveArmor && !brokeArmor)
                    {
                        armorAnim.SetBool("isSpawn", false);
                        armorAnim.SetBool("isIdle", true);
                    }

                    currentState = State.Idle;
                }
                break;

            case State.Idle:
                Idle();
                if (attackType == Attack_Type.Patrol)
                {
                    if (CheckTarget())
                    {
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isPatrol", true);
                        anim.SetBool("isAttackReady", true);

                        if (haveArmor && !brokeArmor)
                        {
                            armorAnim.SetBool("isIdle", false);
                            armorAnim.SetBool("isPatrol", true);
                            armorAnim.SetBool("isAttackReady", true);
                        }

                        attackReady = true;
                        currentState = State.Patrol;
                    }
                    else if (CheckTargetBack())
                    {
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isPatrol", true);
                        anim.SetBool("isAttackReady", true);

                        if (haveArmor && !brokeArmor)
                        {
                            armorAnim.SetBool("isIdle", false);
                            armorAnim.SetBool("isPatrol", true);
                            armorAnim.SetBool("isAttackReady", true);
                        }

                        attackReady = true;
                        currentState = State.Patrol;
                        Flip();
                    }
                }
                else if (attackType == Attack_Type.Inplace)
                {
                    if (transform.localScale.x > 0)
                    {
                        if (CheckTarget())
                        {
                            anim.SetBool("isAttack", true);
                            anim.SetBool("isIdle", false);

                            if (haveArmor && !brokeArmor)
                            {
                                armorAnim.SetBool("isAttack", true);
                                armorAnim.SetBool("isIdle", false);
                            }

                            currentState = State.Attack;
                        }
                    }
                    else
                    {
                        if (CheckTargetBack())
                        {
                            anim.SetBool("isAttack", true);
                            anim.SetBool("isIdle", false);

                            if (haveArmor && !brokeArmor)
                            {
                                armorAnim.SetBool("isAttack", true);
                                armorAnim.SetBool("isIdle", false);
                            }

                            currentState = State.Attack;
                        }
                    }
                }
                else if (attackType == Attack_Type.RangeInplace || attackType == Attack_Type.Teleport)
                {
                    if (CheckTarget() || CheckTargetBack())
                    {
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isBattleIdle", true);

                        currentState = State.BattleIdle;
                    }
                }
                else
                {
                    if (idleTimer <= 0f)
                    {
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isPatrol", true);

                        if (haveArmor && !brokeArmor)
                        {
                            armorAnim.SetBool("isIdle", false);
                            armorAnim.SetBool("isPatrol", true);
                        }

                        Flip();
                        currentState = State.Patrol;
                    }
                    if (CheckTarget())
                    {
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isPatrol", false);
                        anim.SetBool("isTrace", true);

                        if (haveArmor && !brokeArmor)
                        {
                            armorAnim.SetBool("isIdle", false);
                            armorAnim.SetBool("isPatrol", false);
                            armorAnim.SetBool("isTrace", true);
                        }

                        currentState = State.Trace;
                    }
                    else if (CheckTargetBack())
                    {
                        anim.SetBool("isPatrol", false);
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isTrace", true);

                        if (haveArmor && !brokeArmor)
                        {
                            armorAnim.SetBool("isPatrol", false);
                            armorAnim.SetBool("isIdle", false);
                            armorAnim.SetBool("isTrace", true);
                        }

                        Flip();
                        currentState = State.Trace;
                    }
                }

                if (isDead)
                {
                    currentState = State.Die;
                }
                break;

            case State.Patrol:
                Patrol();
                if (patrolTimer <= 0)
                {
                    if (attackType != Attack_Type.Patrol)
                    {
                        if (CheckTarget())
                        {
                            anim.SetBool("isPatrol", false);
                            anim.SetBool("isIdle", false);
                            anim.SetBool("isTrace", true);

                            if (haveArmor && !brokeArmor)
                            {
                                armorAnim.SetBool("isPatrol", false);
                                armorAnim.SetBool("isIdle", false);
                                armorAnim.SetBool("isTrace", true);
                            }

                            currentState = State.Trace;
                        }
                        else if (CheckTargetBack())
                        {
                            anim.SetBool("isPatrol", false);
                            anim.SetBool("isIdle", false);
                            anim.SetBool("isTrace", true);

                            if (haveArmor && !brokeArmor)
                            {
                                armorAnim.SetBool("isPatrol", false);
                                armorAnim.SetBool("isIdle", false);
                                armorAnim.SetBool("isTrace", true);
                            }

                            Flip();
                            currentState = State.Trace;
                        }
                    }
                }

                if (CheckWall() || !CheckLedge())
                {
                    Flip();
                }
                if (isDead)
                {
                    currentState = State.Die;
                }
                break;

            case State.Trace:
                Trace();
                if (CheckDistance() < attackDistace && CheckDistanceY() < attackDistanceY && attackTimer <= 0)
                {
                    waitTraceTimer = waitTraceTime;
                    anim.SetBool("isTrace", false);
                    anim.SetBool("isAttack", true);

                    if (haveArmor && !brokeArmor)
                    {
                        armorAnim.SetBool("isTrace", false);
                        armorAnim.SetBool("isAttack", true);
                    }

                    currentState = State.Attack;
                }
                else if (CheckDistance() <= minTraceDistance && attackTimer > 0)
                {
                    waitTraceTimer = waitTraceTime;
                    anim.SetBool("isTrace", false);
                    anim.SetBool("isBattleIdle", true);

                    if (haveArmor && !brokeArmor)
                    {
                        armorAnim.SetBool("isTrace", false);
                        armorAnim.SetBool("isBattleIdle", true);
                    }

                    currentState = State.BattleIdle;
                }
                else if (waitTraceTimer > 0)
                {
                    anim.SetBool("isTrace", false);
                    anim.SetBool("isBattleIdle", true);

                    if (haveArmor && !brokeArmor)
                    {
                        armorAnim.SetBool("isTrace", false);
                        armorAnim.SetBool("isBattleIdle", true);
                    }

                    currentState = State.BattleIdle;
                }

                if (CheckWall() || !CheckLedge())
                {
                    Flip();
                    anim.SetBool("isTrace", false);
                    anim.SetBool("isPatrol", true);

                    if (haveArmor && !brokeArmor)
                    {
                        armorAnim.SetBool("isTrace", false);
                        armorAnim.SetBool("isPatrol", true);
                    }

                    currentState = State.Patrol;
                    patrolTimer = patrolTime;
                }

                if (isDead)
                {
                    currentState = State.Die;
                }
                break;

            case State.BattleIdle:
                BattleIdle();
                if (attackType == Attack_Type.RangeInplace || attackType == Attack_Type.Teleport)
                {
                    if (CheckDistance() <= attackDistace && CheckDistanceY() < attackDistanceY && attackTimer <= 0)
                    {
                        //doAttack = true;
                        anim.SetBool("isBattleIdle", false);
                        anim.SetBool("isAttack", true);

                        if (haveArmor && !brokeArmor)
                        {
                            armorAnim.SetBool("isBattleIdle", false);
                            armorAnim.SetBool("isAttack", true);
                        }

                        currentState = State.Attack;
                    }
                }
                else
                {
                    if (CheckDistance() > minTraceDistance && waitTraceTimer <= 0)
                    {
                        anim.SetBool("isBattleIdle", false);
                        anim.SetBool("isTrace", true);

                        if (haveArmor && !brokeArmor)
                        {
                            armorAnim.SetBool("isBattleIdle", false);
                            armorAnim.SetBool("isTrace", true);
                        }

                        currentState = State.Trace;
                    }
                    else if (CheckDistance() <= attackDistace && CheckDistanceY() < attackDistanceY && attackTimer <= 0)
                    {
                        // doAttack = true;
                        anim.SetBool("isBattleIdle", false);
                        anim.SetBool("isAttack", true);

                        if (haveArmor && !brokeArmor)
                        {
                            armorAnim.SetBool("isBattleIdle", false);
                            armorAnim.SetBool("isAttack", true);
                        }

                        currentState = State.Attack;
                    }
                }
                if (isDead)
                {
                    currentState = State.Die;
                }
                break;

            case State.Attack:
                Attack();
                if (attackType == Attack_Type.Inplace)
                {
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack"))
                    {
                        //anim.SetBool("isAttack", false);
                        anim.SetBool("isIdle", true);

                        if (haveArmor && !brokeArmor)
                        {
                            //armorAnim.SetBool("isAttack", false);
                            armorAnim.SetBool("isIdle", true);
                        }

                        //doAttack = false;
                        currentState = State.Idle;
                    }
                }
                else
                {
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack"))
                    {
                        //anim.SetBool("isAttack", false);
                        anim.SetBool("isBattleIdle", true);

                        if (haveArmor && !brokeArmor)
                        {
                            //armorAnim.SetBool("isAttack", false);
                            armorAnim.SetBool("isBattleIdle", true);
                        }

                        currentState = State.BattleIdle;
                    }
                    if (isDead)
                    {
                        currentState = State.Die;
                    }
                }
                break;

            case State.Die:
                Die();
                break;

            default:
                break;
        }
    }

    #region Check

    public bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, gameObject.transform.right * isRight, wallCheckDistance, whatIsGround);
    }

    public bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, ledgeCheckDistance, whatIsGround);
    }

    public bool CheckTarget()
    {
        return Physics2D.Raycast(targetDistanceCheck.position, gameObject.transform.right * isRight, targetCheckDistance, whatIsTarget);
    }

    public bool CheckTargetBack()
    {
        return Physics2D.Raycast(targetDistanceCheck.position, -gameObject.transform.right * isRight, targetCheckDistance, whatIsTarget);
    }

    public float CheckDistance()
    {
        return Mathf.Abs(target.transform.position.x - transform.position.x);
    }
    public float CheckDistanceY()
    {
        return Mathf.Abs(target.transform.position.y - transform.position.y);
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

    private void Patrol()
    {
        if (attackReady)
        {
            Set_Movespeed(0f);
        }
        else
        {
            Set_Movespeed(moveSpeed);
        }
        gameObject.transform.Translate(Vector2.right * currentSpeed * isRight * Time.deltaTime);
    }

    private void Trace()
    {
        Set_Movespeed(moveSpeed);
        LookAtPlayer();
        gameObject.transform.Translate(Vector2.right * currentSpeed * isRight * Time.deltaTime);
    }

    private void BattleIdle()
    {
        Set_Movespeed(0f);
        LookAtPlayer();
    }

    private void Attack()
    {
        if (attackType == Attack_Type.Range) LookAtPlayer();
        Set_Movespeed(0f);
    }

    public void AttackTeleport()
    {
        Vector2 setStartPos;    // 1차 보정값
        Vector2 result;         // 최종 보정값  

        //플레이어와 몹 위치 관계에 따른 공격 x 및 y 값 조정
        if (target.transform.localScale.x > 0) setStartPos = new(target.transform.position.x - 3, target.transform.position.y + 6.5f);
        else setStartPos = new(target.transform.position.x + 3, target.transform.position.y + 6.5f);

        transform.position = setStartPos;
        //Debug.Log("1차 보정 : " + setStartPos);

        //조정된 위치 기준 ground 의 유무에 따른 x 값 조정 및 y 값을 지표값으로 조정
        if (attackGroundCheck.CheckAttackGroundBool()) result = new(setStartPos.x, attackGroundCheck.CheckAttackGround().y); // 땅이 있다면 x값 고정 및 y 값 지표면으로 변경
        else    // 땅이 없다면 x값을 타겟의 x값으로 변경 후 y 값 지표면으로 변경
        {
            setStartPos = new(target.transform.position.x, target.transform.position.y + 6.5f);
            transform.position = setStartPos;
            result = new(setStartPos.x, attackGroundCheck.CheckAttackGround().y);
        }

        //Debug.Log("2차 보정 : " + result);

        //transform.position = result;

        anim.SetTrigger("doAttack");
        transform.position = result;
    }

    private void Die()
    {
        Set_Movespeed(0f);
        // doAttack = false;
    }

    #endregion State

    public void Set_Movespeed(float speed)
    {
        currentSpeed = speed;
    }

    public void Flip()
    {
        isRight *= -1;
        gameObject.transform.localScale = new Vector3(isRight, 1, 1);
    }

    public void LookAtPlayer()
    {
        if (gameObject.transform.position.x > target.transform.position.x)
        {
            isRight = -1;
            gameObject.transform.localScale = new Vector3(isRight, 1, 1);
        }
        else
        {
            isRight = 1;
            gameObject.transform.localScale = new Vector3(isRight, 1, 1);
        }
    }

    public void Set_SpawnEnd()
    {
        doSpawn = false;
    }

    public void EndAttack()
    {
        attackTimer = attackTime;
        anim.SetBool("isAttack", false);
        if (haveArmor && !brokeArmor)
        {
            armorAnim.SetBool("isAttack", false);
        }
    }

    public void EndAttackReady()
    {
        anim.SetBool("isAttackReady", false);
        if (haveArmor && !brokeArmor)
        {
            armorAnim.SetBool("isAttackReady", false);
        }
        attackReady = false;
    }

    public void Set_Drag(float drag)
    {
        rigid.drag = drag;
    }

    public void Set_AttackType()
    {
        attackType = Attack_Type.RangeInplace;
    }
}