using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Boss_Rosemary_Manager : MonoBehaviour
{
    private enum State
    {
        Idle,
        Run,
        Attack,
        FixingPosture,
        Step,
        Backstep,
        Groggy,
        Die
    }

    private Rigidbody2D rigid;
    private Animator anim;
    private GameObject target;
    private Boss_Health_Manager healthmanager;
    private DataManager dataManager;
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
    [SerializeField] private Transform targetDistanceCheck;
    [SerializeField] private float targetCheckDistance;
    [SerializeField] private float attack02CheckDistance;

    [Space(5f)]
    [SerializeField] private float minTraceDistance;
    [SerializeField] private float attack01Distance;
    [SerializeField] private float attack02Distance;
    [SerializeField] private float attack03Distance;
    [SerializeField] private float backstepDistance;
    [SerializeField] private float stepDistance;

    [Space(10f)]
    [Header("Status")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float defaultGravity;
    [SerializeField] private float defaultDrag;

    [Space(5f)]
    private int currnetAttackNum;

    [Space(10f)]
    [Header("Timer")]
    [SerializeField] private float waitTraceTime;
    [SerializeField] private List<float> attackCoolTime;
    [SerializeField] private float stepTime;
    [SerializeField] private float backstepTime;
    [SerializeField] private float groggyTime;
    private float waitTracetimer;
    private float attack01Timer;
    private float attack02Timer;
    private float attack03Timer;
    private float stepTimer;
    private float backstepTimer;
    private float groggyTimer;

    [Space(10f)]
    private bool isStand = true;

    private bool startCombat = false;
    private bool doAttack = false;
    private bool doAttack02 = false;
    private bool endAttack02 = false;
    private bool fixPos = false;
    private bool doStep = false;
    private bool doBackstep = false;
    private bool endGroggy = false;
    public bool phase02 = false;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        // target = GameObject.Find("Player(Clone)").gameObject;
        healthmanager = gameObject.GetComponent<Boss_Health_Manager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();

        defaultGravity = rigid.gravityScale;
        defaultDrag = rigid.drag;
        currentState = State.Idle;

        anim.SetBool("Dialogue", dataManager.nowData.boss02_Dialogue);
    }

    // Update is called once per frame
    void Update()
    {
        if (healthmanager.Boss_Defeat_Check())
        {
            target.GetComponentInChildren<CapsuleCollider2D>().enabled = false;
            //SteamAchievement.instance.Achieve("Clear_Rosemary");
            currentState = State.Die;
        }

        if (target == null) target = GameObject.Find("Player(Clone)").gameObject;

        if (target.transform.GetComponent<Player_Manager>().Player_InputCheck() && !startCombat && isStand && dataManager.nowData.boss02_Dialogue)
        {
            isStand = false;
            anim.SetBool("isStand", false);
        }

        waitTracetimer -= Time.deltaTime;

        attack01Timer -= Time.deltaTime;
        attack02Timer -= Time.deltaTime;
        attack03Timer -= Time.deltaTime;

        stepTimer -= Time.deltaTime;
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
                    else if (CheckDistance() <= attack02Distance && attack02Timer <= 0 && !CheckAttack02Distance())
                    {
                        currnetAttackNum = 2;
                        doAttack = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= attack03Distance && attack03Timer <= 0)
                    {
                        currnetAttackNum = 3;
                        doAttack = true;
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
                        currentState = State.Backstep;
                    }
                    else if (CheckDistance() <= stepDistance && stepTimer <= 0 && !doStep)
                    {
                        doStep = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isStep", true);
                        currentState = State.Step;
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
                        anim.SetBool("isRun", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= attack02Distance && attack02Timer <= 0 && !CheckAttack02Distance())
                    {
                        currnetAttackNum = 2;
                        doAttack = true;
                        anim.SetBool("isRun", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= attack03Distance && attack03Timer <= 0)
                    {
                        currnetAttackNum = 3;
                        doAttack = true;
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
                        currentState = State.Backstep;
                    }
                    else if (CheckDistance() <= stepDistance && stepTimer <= 0 && !doStep)
                    {
                        doStep = true;
                        anim.SetBool("isRun", false);
                        anim.SetBool("isStep", true);
                        currentState = State.Step;
                    }
                    else if (healthmanager.isGroggy)
                    {
                        anim.SetBool("isRun", false);
                        currentState = State.Groggy;
                    }

                    break;

                case State.Attack:
                    Attack();
                    if (fixPos)
                    {
                        anim.SetBool("isFixPos", true);
                        currentState = State.FixingPosture;
                    }
                    else if (healthmanager.isGroggy)
                    {
                        anim.SetBool("isAttack", false);
                        currentState = State.Groggy;
                    }
                    break;

                case State.FixingPosture:
                    FixingPosture();
                    if (!doAttack)
                    {
                        if (CheckDistance() <= minTraceDistance)
                        {
                            anim.SetBool("isAttack", false);
                            anim.SetBool("isFixPos", false);
                            anim.SetBool("isIdle", true);
                            currentState = State.Idle;
                        }
                        else
                        {
                            anim.SetBool("isAttack", false);
                            anim.SetBool("isFixPos", false);
                            anim.SetBool("isRun", true);
                            currentState = State.Run;
                        }
                    }
                    break;

                case State.Step:
                    Step();
                    if (!doStep)
                    {
                        if (CheckDistance() <= minTraceDistance)
                        {
                            anim.SetBool("isStep", false);
                            anim.SetBool("isIdle", true);
                            currentState = State.Idle;
                        }
                        else
                        {
                            anim.SetBool("isStep", false);
                            anim.SetBool("isRun", true);
                            currentState = State.Run;
                        }
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

    public void UnLookAtPlayer()
    {
        if (gameObject.transform.position.x > target.transform.position.x)
        {
            isRight = 1;
            gameObject.transform.localScale = new Vector3(isRight, 1, 1);
        }
        else
        {
            isRight = -1;
            gameObject.transform.localScale = new Vector3(isRight, 1, 1);
        }
    }
    #endregion

    #region Check
    public bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, gameObject.transform.right * isRight, wallCheckDistance, whatIsGround);
    }

    public bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, ledgeCheckDistance, whatIsGround);
    }

    public bool CheckTargetBack()
    {
        return Physics2D.Raycast(targetDistanceCheck.position, -gameObject.transform.right * isRight, targetCheckDistance, whatIsTarget);
    }

    public float CheckDistance()
    {
        return Mathf.Abs(target.transform.position.x - transform.position.x);
    }

    public bool CheckAttack02Distance()
    {
        return Physics2D.Raycast(wallCheck.position, gameObject.transform.right * isRight, attack02CheckDistance, whatIsGround);
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

        Set_VelocityZero_CheckGround();
    }

    public void FixingPosture()
    {
        Set_MoveSpeed(0);
    }

    public void Step()
    {
        Set_MoveSpeed(0);

        Set_VelocityZero_CheckGround();
    }
    public void Backstep()
    {
        Set_MoveSpeed(0);

        Set_VelocityZero_CheckGround();
    }
    public void Groggy()
    {
        Set_MoveSpeed(0);
    }
    public void Die()
    {
        Set_MoveSpeed(0);
    }
    #endregion

    private void Set_VelocityZero_CheckGround()
    {
        if (!CheckLedge() || CheckWall())
        {
            rigid.velocity = Vector2.zero;
        }
    }

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
                doAttack02 = false;
                endAttack02 = false;
                break;

            case 3:
                attack03Timer = attackCoolTime[currnetAttackNum - 1];
                break;
        }
    }

    public void End_Attack()
    {
        fixPos = true;
        LookAtPlayer();
    }

    public void End_FixPos()
    {
        Set_AttackCoolTime();
        doAttack = false;
        fixPos = false;
    }
    public void End_Step()
    {
        stepTimer = stepTime;
        doStep = false;
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

    public void Do_Attack02()
    {
        StartCoroutine(Attack02_Delay());
    }

    private IEnumerator Attack02_Delay()
    {
        yield return new WaitForSeconds(1f);

        if (!doAttack02)
        {
            doAttack02 = true;

            if (transform.localScale.x == 1)
            {
                transform.position = new(transform.position.x + 19, transform.position.y);
            }
            else
            {
                transform.position = new(transform.position.x - 19, transform.position.y);
            }
            anim.SetTrigger("doAttack02");
        }
    }

    public void End_Attack02()
    {
        if (!endAttack02)
        {
            endAttack02 = true;

            anim.SetTrigger("endAttack02");
        }
    }

    public void Set_Attack02_Position()
    {
        if (transform.localScale.x == 1)
        {
            //transform.position = new(transform.position.x - 19, transform.position.y);
            transform.localScale = new(-1, 1, 1);
        }
        else
        {
            //transform.position = new(transform.position.x + 19, transform.position.y);
            transform.localScale = new(1, 1, 1);
        }
    }

    public void Start_Combat()
    {
        anim.SetTrigger("CombatReady");
        startCombat = true;
    }
}
