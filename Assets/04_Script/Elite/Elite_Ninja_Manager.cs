using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elite_Ninja_Manager : MonoBehaviour
{
    private enum State
    {
        Spawn,
        Idle,
        Attack,
        Rest,
        Die
    }

    private Rigidbody2D rigid;
    private Animator anim;
    private GameObject target;
    private Boss_Health_Manager healthmanager;
    private Enemy_Projectile_Manager projectileManager;
    private Object_SoundManager sfxManager;
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

    [Space(5f)]
    [SerializeField] private float minTraceDistance;
    [SerializeField] private float attack01Distance;
    [SerializeField] private float attack02Distance;

    [Space(5f)]
    [SerializeField] private Transform attack01GroundPointCheck;
    [SerializeField] private float attack01GroundPointDistance;

    [Space(5f)]
    [SerializeField] private float attack01BanishMaxDistance;
    [SerializeField] private float attack01BanishMinDistance;

    [Space(5f)]
    private bool doAttack02 = false;
    private bool attack02Onair = false;
    [SerializeField] private Transform attack02GroundCheck;
    [SerializeField] private float attack02GroundCheckDistance;

    [SerializeField] private Vector2 attack02Addforce;
    [SerializeField] private float attack02StartGravity;
    [SerializeField] private float attack02EndGravity;

    [Space(5f)]
    public GameObject rangeObject;
    private BoxCollider2D rangeCollider;

    [Space(10f)]
    [Header("Status")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float currentSpeed;
    [SerializeField] private float defaultGravity;
    [SerializeField] private float defaultDrag;

    [Space(5f)]
    [SerializeField] private float jumpDuration;

    [Space(5f)]
    private int currnetAttackNum;

    [Space(10f)]
    [Header("Timer")]
    [SerializeField] private float waitTraceTime;

    [SerializeField] private List<float> attackCoolTime;

    [SerializeField] private float restTime;
    private float attack01Timer;
    private float attack02Timer;
    private float restTimer;


    [Space(10f)]
    private bool doAttack = false;
    private bool isSpawned = false;

    // Start is called before the first frame update
    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        healthmanager = gameObject.GetComponent<Boss_Health_Manager>();
        projectileManager = gameObject.GetComponent<Enemy_Projectile_Manager>();
        sfxManager = transform.GetComponent<Object_SoundManager>();

        rangeObject = GameObject.Find("EliteBoundary").gameObject;
        rangeCollider = rangeObject.GetComponent<BoxCollider2D>();

        defaultGravity = rigid.gravityScale;
        defaultDrag = rigid.drag;

    }

    private void Start()
    {
        target = GameObject.Find("Player(Clone)").gameObject;
        currentState = State.Spawn;
        //anim.SetBool("isIdle", true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (target == null) target = GameObject.Find("Player(Clone)").gameObject;  // test 씬 테스트용

        if (attack01Timer >= 0) attack01Timer -= Time.deltaTime;
        if (attack02Timer >= 0) attack02Timer -= Time.deltaTime;
        if (restTimer >= 0) restTimer -= Time.deltaTime;

        if (healthmanager.Boss_Defeat_Check()) currentState = State.Die;


    }

    private void FixedUpdate()
    {
        if (attack02Onair)
        {
            if (rigid.velocity.y < 0) Set_Gravity(attack02EndGravity);
            if (Attack02GroundCheck())
            {
                ATtack02EndTrigger();
            }
        }
        if (!doAttack02)
        {
            if (!CheckLedge() || CheckWall())
            {
                rigid.velocity = Vector2.zero;
            }
        }



        switch (currentState)
        {
            case State.Spawn:
                Spawn();
                if (isSpawned)
                {
                    anim.SetBool("isIdle", true);
                    currentState = State.Rest;
                    restTimer = restTime;
                }
                break;

            case State.Idle:
                Idle();
                if (CheckDistance() <= attack01Distance && attack01Timer <= 0)
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

    private Vector2 Return_GroundPoint()
    {
        return Physics2D.Raycast(attack01GroundPointCheck.position, Vector2.down, attack01GroundPointDistance, whatIsGround).point;
    }
    public bool Attack02GroundCheck()
    {
        return Physics2D.Raycast(attack02GroundCheck.position, Vector2.down, attack02GroundCheckDistance, whatIsGround);
    }

    private Vector3 Return_RandomPosition()
    {
        Vector3 originPosition = rangeObject.transform.position;

        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = rangeCollider.bounds.size.x;
        float range_Y = rangeCollider.bounds.size.y;

        range_X = Random.Range(-(range_X / 2), range_X / 2);
        range_Y = Random.Range(-(range_Y / 2), range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y, 0f);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }


    #endregion Check

    #region stateControl

    public void Spawn()
    {
        Set_MoveSpeed(0);
        LookAtPlayer();
    }

    public void Idle()
    {
        Set_MoveSpeed(0);
        LookAtPlayer();
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

    // -------------------------------- attack trigger ---------------------------------

    public void Attack01Trigger()
    {
        StartCoroutine(Attack01Banish());
    }

    private IEnumerator Attack01Banish()
    {
        yield return new WaitForSeconds(0.5f);

        // 정해진 범위내에서 플레이어로 부터 일정거리로 이동
        do
        {
            transform.position = Return_RandomPosition();
            transform.position = new(transform.position.x, Return_GroundPoint().y);
            InfiniteLoopDetector.Run();
        } while (CheckDistance() < attack01BanishMinDistance || CheckDistance() > attack01BanishMaxDistance);

        anim.SetTrigger("Attack01Trigger");
    }



    public IEnumerator Attack02Trigger()
    {
        Vector2 result;
        doAttack02 = true;
        anim.SetTrigger("Attack02Trigger");
        Set_Gravity(attack02StartGravity);

        if (transform.position.x > target.transform.position.x) result = new(-attack02Addforce.x, attack02Addforce.y);
        else result = attack02Addforce;

        rigid.AddForce(result, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);
        attack02Onair = true;
    }

    public IEnumerator Attack02ThrowKnife()
    {
        yield return new WaitForSeconds(0.4f);

        for (int i = 0; i < 3; i++)
        {
            projectileManager.Instance_Projectile(1);
            SoundManager.instance.SFXPlayer(/*"Player_Animation",*/ sfxManager.Get_AudioClip("yellow"), gameObject.transform);
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void ATtack02EndTrigger()
    {
        doAttack02 = false;
        attack02Onair = false;
        anim.SetTrigger("Attack02End");
        Set_Drag(5f);
        Reset_Gravity();
    }


    public void EndSpawn()
    {
        isSpawned = true;
        anim.SetBool("isSpawn", false);
    }
}