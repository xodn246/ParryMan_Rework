using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Elite_Farmer_Manager : MonoBehaviour
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
    [SerializeField] private List<GameObject> DestroyThorn;

    [SerializeField] private List<GameObject> Attack01ThornPos;
    [SerializeField] private List<int> Attack01SellectPos;
    [SerializeField] private List<GameObject> Attack01Thorns;
    [SerializeField] private int thornCount;
    private int currentThornCount = 0;

    [Space(10f)]
    [SerializeField] private GameObject bounceHitbox;
    private bool bounceActive;
    [SerializeField] private float Attack02MaxSpeed;
    [SerializeField] private float Attack02Duration;
    private float Attack02DurationTimer;
    [SerializeField] private float attack02Speed;
    private IEnumerator CoroutineAttack02;
    private bool doAttack02 = false;

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

        defaultGravity = rigid.gravityScale;
        defaultDrag = rigid.drag;

        CoroutineAttack02 = Attack02_Logic();
        bounceHitbox = transform.Find("hitbox_bounce").gameObject;

        Attack01ThornPos.AddRange(GameObject.FindGameObjectsWithTag("Farmer_Thorn_Pos"));
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

            DestroyThorn.AddRange(GameObject.FindGameObjectsWithTag("Enemy_PurpleProjectile"));
            for (int i = 0; i < DestroyThorn.Count; i++)
            {
                DestroyThorn[i].GetComponent<Animator>().SetTrigger("isDestroy");
            }
        }

        if (doAttack02)
        {
            Attack02DurationTimer -= Time.deltaTime;

            if (CheckWall()) End_Attack02();

            if (healthmanager.takedamage) End_Attack02();
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

    public void Attack01_Spawn_Thorn()
    {
        int selectPos;

        int thornSelect = Random.Range(0, Attack01Thorns.Count);
        GameObject playerThorn = Instantiate(Attack01Thorns[thornSelect], target.transform.position, Quaternion.identity); // 가시 소환시 땅에 붙여야됨 밑에 애들도 적용시켜서 한번에 같은 높이로 만들어주기
        playerThorn.transform.position = new(playerThorn.transform.position.x, playerThorn.GetComponent<Elite_Farmer_ThornManager>().CheckGroundPoint().y);

        for (int i = 0; i < thornCount; i++)
        {
            while (currentThornCount < Attack01ThornPos.Count)
            {
                selectPos = Random.Range(0, Attack01ThornPos.Count);
                if (Attack01SellectPos.Contains(selectPos)) // 선택 위치 중복시 재선택
                {
                    continue;
                }
                else
                {
                    Attack01SellectPos.Add(selectPos);  //선택된 위치 배열에 저장 > 중복 방지

                    thornSelect = Random.Range(0, Attack01Thorns.Count);
                    GameObject thorn = Instantiate(Attack01Thorns[thornSelect], Attack01ThornPos[selectPos].transform.position, Quaternion.identity);
                    thorn.GetComponent<Elite_Farmer_ThornManager>().Set_ThornPosNum(selectPos);
                    thorn.transform.position = new(thorn.transform.position.x, thorn.GetComponent<Elite_Farmer_ThornManager>().CheckGroundPoint().y);
                    currentThornCount++;
                    break;
                }
            }
        }
    }

    public void Attack01_Destroy_Thorn(int posNum)  //파괴된 위치의 인덱스값 선택리스트에서 제거
    {
        if (Attack01SellectPos.Contains(posNum))
        {
            Attack01SellectPos = Attack01SellectPos.Where(num => num != posNum).ToList();
            currentThornCount--;
        }
    }


    public void Start_Attack02()
    {
        Attack02DurationTimer = Attack02Duration;
        anim.SetTrigger("doAttack02");
        StartCoroutine(CoroutineAttack02);
    }
    public void End_Attack02()
    {
        anim.SetTrigger("endAttack02");
        doAttack02 = false;
        StopCoroutine(CoroutineAttack02);
    }

    public IEnumerator Attack02_Logic()
    {
        while (doAttack02)
        {
            if (Attack02DurationTimer <= 0) End_Attack02();

            if (target.transform.position.x > transform.position.x)
            {
                rigid.AddForce(Vector2.right * attack02Speed, ForceMode2D.Impulse);
            }
            else
            {
                rigid.AddForce(Vector2.left * attack02Speed, ForceMode2D.Impulse);
            }
            LookAtPlayer();
            Attack02_Limite_Speed();

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Attack02_Limite_Speed()
    {
        if (rigid.velocity.x > Attack02MaxSpeed) rigid.velocity = new Vector2(Attack02MaxSpeed, 0);
        if (rigid.velocity.x < -Attack02MaxSpeed) rigid.velocity = new Vector2(-Attack02MaxSpeed, 0);
    }
}
