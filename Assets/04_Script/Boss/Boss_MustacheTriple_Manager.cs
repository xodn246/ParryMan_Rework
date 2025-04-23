using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Steamworks;

public class Boss_MustacheTriple_Manager : MonoBehaviour
{
    private enum State
    {
        Idle,
        Run,
        Attack,
        Backstep,
        Groggy,
        Die
    }

    private Rigidbody2D rigid;
    private Animator anim;
    private GameObject target;
    private Boss_Health_Manager healthmanager;
    private DataManager dataManager;
    //private PlayerInput playerInput;
    private float isRight = -1;

    [SerializeField] private State currentState;

    [Space(10f)]
    [Header("LayerMask")]
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private LayerMask whatIsTarget;

    [Space(10f)]
    [Header("DistanceCheck")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private float ledgeCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float jumpHeight;
    [SerializeField] private Transform targetDistanceCheck;
    [SerializeField] private float targetCheckDistance;

    [Space(5f)]
    [SerializeField] private float minTraceDistance;

    [SerializeField] private float attack01Distance;
    [SerializeField] private float attack02Distance;
    [SerializeField] private float attack03MinDistance;
    [SerializeField] private float attack03MaxDistance;
    [SerializeField] private float attack03DropDistance;
    [SerializeField] private float backstepDistance;

    [Space(10f)]
    [Header("Status")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float currentSpeed;
    [SerializeField] private float defaultGravity;
    [SerializeField] private float defaultDrag;

    [Space(5f)]
    [SerializeField] private float minJumpWidth;
    [SerializeField] private float maxJumpWidth;
    [SerializeField] private float jumpHieght;
    [SerializeField] private float onAirVelocity;
    [SerializeField] private float onAirGravity;
    [SerializeField] private float dropGravity;
    private bool doJumpAttack = false;
    private bool onAir = false;

    [Space(5f)]
    private int currnetAttackNum;

    [Space(10f)]
    [Header("Timer")]
    [SerializeField] private float waitTraceTime;

    [SerializeField] private List<float> attackCoolTime;
    [SerializeField] private float backstepTime;
    [SerializeField] private float groggyTime;
    private float waitTracetimer;
    private float attack01Timer;
    private float attack02Timer;
    private float attack03Timer;
    private float backstepTimer;
    private float groggyTimer;

    [Space(10f)]
    private bool isStand = true;

    private bool startCombat = false;
    private bool doAttack = false;
    private bool doBackstep = false;
    private bool doLand = false;
    private bool endGroggy = false;
    public bool phase02 = false;

    // Start is called before the first frame update
    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        healthmanager = gameObject.GetComponent<Boss_Health_Manager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();

        defaultGravity = rigid.gravityScale;
        defaultDrag = rigid.drag;
        currentState = State.Idle;

        anim.SetBool("Dialogue", dataManager.nowData.boss01_Dialogue);
    }

    // Update is called once per frame
    private void Update()
    {
        if (target == null) target = GameObject.Find("Player(Clone)").gameObject;

        if (healthmanager.Boss_Defeat_Check())
        {
            //SteamAchievement.instance.Achieve("Clear_Mustache");
            target.GetComponentInChildren<CapsuleCollider2D>().enabled = false;

            currentState = State.Die;
        }

        if (target.transform.GetComponent<Player_Manager>().Player_InputCheck() && !startCombat && isStand && dataManager.nowData.boss01_Dialogue)
        {
            isStand = false;
            anim.SetBool("isStand", false);
        }

        waitTracetimer -= Time.deltaTime;

        attack01Timer -= Time.deltaTime;
        attack02Timer -= Time.deltaTime;
        attack03Timer -= Time.deltaTime;

        backstepTimer -= Time.deltaTime;

        groggyTimer -= Time.deltaTime;



        if (healthmanager.currentHealth == healthmanager.groggyHealth)
        {
            if (!healthmanager.isGroggy && !phase02)
            {
                anim.SetTrigger("isGroggy");
                anim.SetBool("Phase02", true);
                groggyTimer = groggyTime;
                healthmanager.isGroggy = true;
                phase02 = true;
            }
        }

        if (healthmanager.isGroggy)
        {
            if (healthmanager.groggyHitCounter == healthmanager.groggyHitCout || groggyTimer <= 0)
            {
                if (!endGroggy)
                {
                    endGroggy = true;
                    anim.SetTrigger("endGroggy");
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (startCombat)
        {
            switch (currentState)
            {
                case State.Idle:
                    Idle();
                    if (CheckDistance() > minTraceDistance && waitTracetimer <= 0)
                    {
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isRun", true);
                        currentState = State.Run;
                    }
                    else if (CheckDistance() <= attack01Distance && attack01Timer <= 0)
                    {
                        currnetAttackNum = 1;
                        doAttack = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= attack02Distance && attack02Timer <= 0)
                    {
                        currnetAttackNum = 2;
                        doAttack = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (phase02 && CheckDistance() > attack03MinDistance && CheckDistance() <= attack03MaxDistance && attack03Timer <= 0)
                    {
                        currnetAttackNum = 3;
                        doAttack = true;
                        doJumpAttack = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= backstepDistance && backstepTimer <= 0 && !doBackstep)
                    {
                        doBackstep = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isBackstep", true);
                        attack02Timer = 0f;
                        currentState = State.Backstep;
                    }
                    else if (healthmanager.isGroggy)
                    {
                        anim.SetBool("isIdle", false);
                        currentState = State.Groggy;
                    }

                    break;

                case State.Run:
                    Run();
                    if (CheckDistance() <= minTraceDistance)
                    {
                        anim.SetBool("isIdle", true);
                        anim.SetBool("isRun", false);
                        currentState = State.Idle;
                    }
                    else if (CheckDistance() <= attack01Distance && attack01Timer <= 0)
                    {
                        currnetAttackNum = 1;
                        doAttack = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= attack02Distance && attack02Timer <= 0)
                    {
                        currnetAttackNum = 2;
                        doAttack = true;
                        anim.SetBool("isRun", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (phase02 && CheckDistance() > attack03MinDistance && CheckDistance() <= attack03MaxDistance && attack03Timer <= 0)
                    {
                        currnetAttackNum = 3;
                        doAttack = true;
                        doJumpAttack = true;
                        anim.SetBool("isRun", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= backstepDistance && backstepTimer <= 0 && !doBackstep)
                    {
                        doBackstep = true;
                        anim.SetBool("isRun", false);
                        anim.SetBool("isBackstep", true);
                        attackCoolTime[1] = 0f;
                        currentState = State.Backstep;
                    }
                    else if (healthmanager.isGroggy)
                    {
                        anim.SetBool("isRun", false);
                        currentState = State.Groggy;
                    }

                    break;

                case State.Attack:
                    Attack();
                    if (!doAttack)
                    {
                        if (CheckDistance() <= minTraceDistance)
                        {
                            anim.SetBool("isAttack", false);
                            anim.SetBool("isIdle", true);
                            currentState = State.Idle;
                        }
                        else
                        {
                            anim.SetBool("isAttack", false);
                            anim.SetBool("isRun", true);
                            currentState = State.Run;
                        }
                    }
                    else if (healthmanager.isGroggy)
                    {
                        anim.SetBool("isAttack", false);
                        currentState = State.Groggy;
                    }
                    break;

                case State.Backstep:
                    Backstep();
                    if (!doBackstep)
                    {
                        if (CheckDistance() <= minTraceDistance)
                        {
                            anim.SetBool("isBackstep", false);
                            anim.SetBool("isIdle", true);
                            currentState = State.Idle;
                        }
                        else
                        {
                            anim.SetBool("isBackstep", false);
                            anim.SetBool("isRun", true);
                            currentState = State.Run;
                        }
                    }
                    break;

                case State.Groggy:
                    Groggy();
                    if (!healthmanager.isGroggy)
                    {
                        if (CheckDistance() <= minTraceDistance)
                        {
                            anim.SetBool("isIdle", true);
                            currentState = State.Idle;
                        }
                        else
                        {
                            anim.SetBool("isRun", true);
                            currentState = State.Run;
                        }
                    }
                    break;

                case State.Die:
                    Die();

                    break;
            }
        }
    }

    #region moveControl

    public void Set_MoveSpeed(float speed)
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

    #endregion moveControl

    #region Check

    public bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, gameObject.transform.right * isRight, wallCheckDistance, whatIsGround);
    }

    public bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, ledgeCheckDistance, whatIsGround);
    }

    public bool CheckGround()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    public bool CheckTargetBack()
    {
        return Physics2D.Raycast(targetDistanceCheck.position, -gameObject.transform.right * isRight, targetCheckDistance, whatIsTarget);
    }

    public float CheckDistance()
    {
        return Mathf.Abs(target.transform.position.x - transform.position.x);
    }

    public bool CheckGround_JumpAttack()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, jumpHeight, whatIsGround);
    }

    #endregion Check

    #region stateControl

    public void Idle()
    {
        Set_MoveSpeed(0);
        LookAtPlayer();
    }

    public void Run()
    {
        Set_MoveSpeed(moveSpeed);
        LookAtPlayer();
        gameObject.transform.Translate(Vector2.right * currentSpeed * isRight * Time.deltaTime);
        waitTracetimer = waitTraceTime;
    }

    public void Attack()
    {
        Set_MoveSpeed(0);

        if (!doJumpAttack)
        {
            if (!CheckLedge() || CheckWall())
            {
                rigid.velocity = Vector2.zero;
                Debug.Log("점프공격 아닌 공격중에만 발동");
            }
        }
        else
        {
            if (rigid.velocity.y >= 0 && CheckGround_JumpAttack())
            {
                Set_Gravity(0f);
                rigid.AddForce(new Vector2(0, -100), ForceMode2D.Force);
                Debug.Log("점프 공격 시작");
            }
            else if (rigid.velocity.y >= 0 && !CheckGround_JumpAttack())
            {
                if (!onAir)
                {
                    Debug.Log("점프공격 체공");
                    onAir = true;
                    Set_Gravity(onAirGravity);
                    rigid.velocity = new Vector2(rigid.velocity.x, onAirVelocity);
                }
            }
            else if (onAir && rigid.velocity.y < 0)
            {
                Debug.Log("점프공격 하강");
                Set_Gravity(dropGravity);
                if (rigid.velocity.x >= 0) rigid.AddForce(new(-rigid.velocity.x * 2, -150), ForceMode2D.Force);
                else rigid.AddForce(new(-rigid.velocity.x * 2, -150), ForceMode2D.Force);
            }

            Check_Land();
        }
    }

    private void Check_Land()
    {
        if (!doLand && onAir && CheckGround())
        {
            Debug.Log("점프공격 착지");
            doLand = true;
            anim.SetTrigger("isLand");
            Reset_Drag();
            Reset_Gravity();
        }
    }

    public void JumpStart()
    {
        if (target.transform.position.x > transform.position.x) rigid.AddForce(new(Mathf.Lerp(minJumpWidth, maxJumpWidth, CheckDistance() / (attack03MaxDistance - attack03MinDistance)), jumpHieght), ForceMode2D.Impulse);
        else rigid.AddForce(new(-Mathf.Lerp(minJumpWidth, maxJumpWidth, CheckDistance() / (attack03MaxDistance - attack03MinDistance)), jumpHieght), ForceMode2D.Impulse);
    }

    public void Backstep()
    {
        Set_MoveSpeed(0);
    }

    public void Groggy()
    {
        Set_MoveSpeed(0);
    }

    public void Die()
    {
        Set_MoveSpeed(0);
    }

    #endregion stateControl

    private void Set_AttackCoolTime()
    {
        /** 사용 스킬 갯수에 따라 case 수 조절 해야함**/
        switch (currnetAttackNum)
        {
            case 1:
                attack01Timer = attackCoolTime[currnetAttackNum - 1];
                break;

            case 2:
                attack02Timer = attackCoolTime[currnetAttackNum - 1];
                break;

            case 3:
                attack03Timer = attackCoolTime[currnetAttackNum - 1];
                break;
        }
    }

    public void AttackJumpMovement()
    {
        doJumpAttack = true;
    }

    public void End_Attack()
    {
        Set_AttackCoolTime();
        doAttack = false;
        doJumpAttack = false;
        onAir = false;
        doLand = false;
    }

    public void End_Backstep()
    {
        backstepTimer = backstepTime;
        doBackstep = false;
    }

    public void Set_ExtraAttack()
    {
        doAttack = true;
    }

    public void Set_Gravity(float gravityScale)
    {
        rigid.gravityScale = gravityScale;
    }

    public void Reset_Gravity()
    {
        rigid.gravityScale = defaultGravity;
    }

    public void Set_Drag(float dragScale)
    {
        rigid.drag = dragScale;
    }

    public void Reset_Drag()
    {
        rigid.drag = defaultDrag;
    }

    public void Start_Groggy()
    {
        anim.SetTrigger("doGroggy");
    }

    public void End_Groggy()
    {
        healthmanager.isGroggy = false;
        healthmanager.groggyHitCounter = 0;
    }

    public void Start_Combat()
    {
        anim.SetTrigger("CombatReady");
        startCombat = true;
    }
}