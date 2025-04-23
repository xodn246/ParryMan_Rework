using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elite_Worker_Manager : MonoBehaviour
{
    private enum State
    {
        Idle,
        Run,
        Attack,
        Rest,
        Die
    }

    private Rigidbody2D rigid;
    private Animator anim;
    private GameObject target;
    private Boss_Health_Manager healthmanager;
    private Object_SoundManager sfxManager;
    private float isRight = -1;

    [SerializeField] private BoxCollider2D standCollider;

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

    [Space(5f)]
    [SerializeField] private float minTraceDistance;
    [SerializeField] private float attack01Distance;
    [SerializeField] private float attack02Distance;

    [Space(10f)]
    [SerializeField] private GameObject bounceHitbox;
    private bool bounceActive;


    [Space(10f)]
    [Header("Status")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float currentSpeed;
    [SerializeField] private float defaultGravity;
    [SerializeField] private float defaultDrag;


    [Space(5f)]
    private int currnetAttackNum;

    [Space(10f)]
    [Header("Attack Status")]
    [SerializeField] private GameObject attack01Shock;
    [SerializeField] private int shockCount;
    [SerializeField] private float shockDistance;
    [SerializeField] private float shockTurm;
    [SerializeField] private Transform attack01ShockPos_front;
    [SerializeField] private Transform attack01ShockPos_back;

    private bool doAttack02 = false;

    [Space(10f)]
    [Header("Timer")]
    [SerializeField] private float waitTraceTime;
    [SerializeField] private List<float> attackCoolTime;
    [SerializeField] private float restTime;

    private float waitTracetimer;
    private float attack01Timer;
    private float attack02Timer;
    private float restTimer;
    private bool isDie = false;

    [Space(10f)]
    private bool doAttack = false;

    // Start is called before the first frame update
    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        healthmanager = gameObject.GetComponent<Boss_Health_Manager>();
        sfxManager = transform.GetComponent<Object_SoundManager>();

        defaultGravity = rigid.gravityScale;
        defaultDrag = rigid.drag;

        bounceHitbox = transform.Find("hitbox_bounce").gameObject;
    }

    private void Start()
    {
        currentState = State.Idle;
        anim.SetBool("isIdle", true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (target == null) target = GameObject.Find("Player(Clone)").gameObject;

        attack01Timer -= Time.deltaTime;
        attack02Timer -= Time.deltaTime;
        restTimer -= Time.deltaTime;
        waitTracetimer -= Time.deltaTime;

        if (healthmanager.Boss_Defeat_Check())
        {
            isDie = true;
            Set_Drag(0);
            standCollider.enabled = false;
            currentState = State.Die;
        }

        if (doAttack02)
        {
            if (CheckDistance() <= 10f) Set_Drag(10f);
        }

        bounceHitbox.SetActive(bounceActive);
    }

    private void FixedUpdate()
    {
        if (!isDie)
        {
            if (!CheckLedge() || CheckWall())
            {
                rigid.velocity = Vector2.zero;
            }
        }

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
                    doAttack02 = true;
                    anim.SetBool("isIdle", false);
                    anim.SetBool("isAttack", true);
                    anim.SetInteger("AttackNum", currnetAttackNum);
                    currentState = State.Attack;
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
                else if (CheckDistance() <= attack02Distance && attack02Timer <= 0)
                {
                    currnetAttackNum = 2;
                    doAttack = true;
                    doAttack02 = true;
                    anim.SetBool("isRun", false);
                    anim.SetBool("isAttack", true);
                    anim.SetInteger("AttackNum", currnetAttackNum);
                    currentState = State.Attack;
                }
                break;

            case State.Attack:
                Attack();
                if (!doAttack)
                {
                    anim.SetBool("isAttack", false);
                    anim.SetBool("isIdle", true);
                    currentState = State.Rest;
                    restTimer = restTime;
                }
                break;

            case State.Rest:
                Rest();
                if (restTimer <= 0)
                {
                    currentState = State.Idle;
                }
                break;

            case State.Die:
                Die();

                break;
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

    public bool CheckTargetBack()
    {
        return Physics2D.Raycast(targetDistanceCheck.position, -gameObject.transform.right * isRight, targetCheckDistance, whatIsTarget);
    }

    public float CheckDistance()
    {
        return Mathf.Abs(target.transform.position.x - transform.position.x);
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
    }

    public void Rest()
    {
        Set_MoveSpeed(0);
    }


    public void Die()
    {
        Set_MoveSpeed(0);
        StopCoroutine("Attack01_ShockWave");
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
                doAttack02 = false;
                break;
        }
    }

    public void End_Attack()
    {
        Set_AttackCoolTime();
        doAttack = false;
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

    public void Set_Velocity_Zero()
    {
        rigid.velocity = Vector2.zero;
    }

    //================================ attack detail ==============================

    public IEnumerator Attack01_ShockWave()
    {
        for (int i = 0; i < shockCount; i++)
        {
            yield return new WaitForSeconds(shockTurm);

            float resultFrontX;
            float resultBackX;
            if (/*transform.position.x > target.transform.position.x*/ isRight == -1)
            {
                resultFrontX = attack01ShockPos_front.position.x - i * shockDistance;
                resultBackX = attack01ShockPos_back.position.x + i * shockDistance;
            }
            else
            {
                resultFrontX = attack01ShockPos_front.position.x + i * shockDistance;
                resultBackX = attack01ShockPos_back.position.x - i * shockDistance;
            }

            Vector3 resultFrontPos = new(resultFrontX, attack01ShockPos_front.position.y, attack01ShockPos_front.position.z);
            Vector3 resultBackPos = new(resultBackX, attack01ShockPos_back.position.y, attack01ShockPos_back.position.z);

            Instantiate(attack01Shock, resultFrontPos, Quaternion.identity);
            SoundManager.instance.SFXPlayer(/*"Player_Animation",*/ sfxManager.Get_AudioClip("shockwave"), gameObject.transform);

            Instantiate(attack01Shock, resultBackPos, Quaternion.identity);
            SoundManager.instance.SFXPlayer(/*"Player_Animation",*/ sfxManager.Get_AudioClip("shockwave"), gameObject.transform);
        }
    }

    public void Start_Shockwave()
    {
        StartCoroutine("Attack01_ShockWave");
    }
}
