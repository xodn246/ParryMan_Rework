using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Boss_Master_Manager : MonoBehaviour
{
    private enum State
    {
        Idle,
        Run,
        Attack,
        Dash,
        Groggy,
        Die
    }

    private Rigidbody2D rigid;
    private Animator anim;
    private GameObject target;
    private Boss_Health_Manager healthmanager;
    private Boss_PrintVFX printVFX;
    private DataManager dataManager;
    private PlayerInput playerInput;
    private float isRight = -1;

    [SerializeField] private Object_SoundManager soundManager;
    [Space(10f)]

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
    [SerializeField] private float attack03Distance;
    [SerializeField] private float attack04Distance;
    [SerializeField] private float attack05Distance;
    [SerializeField] private float dashDistance;


    [Space(10f)]
    [Header("Status")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeed;


    [Space(10f)]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float defaultGravity;
    [SerializeField] private float defaultDrag;

    [Space(10f)]
    [SerializeField] private PhysicsMaterial2D rbMaterial;
    [SerializeField] private int maxBounceCount = 0;
    private int bounceCounter = 0;
    [SerializeField] private float attack01JumpPower = 0;
    [SerializeField] private float attack01TracePower = 0f;
    [SerializeField] private float attack01RejectPower = 0f;
    [SerializeField] private float upGravity;
    [SerializeField] private float dropGravity;
    private bool attack01Bounce = false;


    [Space(5f)]
    [SerializeField] private GameObject attack03Prefab;
    [SerializeField] private int attack03Count;
    private int attack03Counter;
    [SerializeField] private float attack03Delay;
    [SerializeField] private float attack03Interver;
    private int currnetAttackNum;


    [Space(5f)]
    [SerializeField] private GameObject axtraMissilePrefab;
    [SerializeField] private Transform missilePos;

    [Space(10f)]
    [SerializeField] private GameObject attack01Hitbox;
    [SerializeField] private GameObject purpleHitbox;
    [SerializeField] private GameObject redHitbox;

    [Space(10f)]
    [Header("Timer")]
    [SerializeField] private float waitTraceTime;

    [SerializeField] private List<float> attackCoolTime;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashMoveTime;
    [SerializeField] private float groggyTime;
    private float waitTracetimer;
    private float attack01Timer;// = 10000f;
    private float attack02Timer;// = 10000f;
    private float attack03Timer;// = 10000f;
    private float attack04Timer;// = 10000f;
    private float attack05Timer;// = 10000f;
    private float dashTimer;
    private float dashMoveTimer;
    private float groggyTimer;

    [Space(10f)]
    private bool isStand = true;

    private bool startCombat = false;
    private bool doAttack = false;
    private bool startAttack01 = false;
    private bool doAttack01 = false;
    private bool attack01Endready = false;
    private bool doDash = false;
    private bool startDash = false;
    private bool endDash = false;
    private bool endGroggy = false;
    public bool phase02 = false;

    // Start is called before the first frame update
    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        healthmanager = gameObject.GetComponent<Boss_Health_Manager>();
        printVFX = gameObject.GetComponent<Boss_PrintVFX>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();

        defaultGravity = rigid.gravityScale;
        defaultDrag = rigid.drag;
        currentState = State.Idle;
        attack03Counter = attack03Count;

        anim.SetBool("CutScene", dataManager.nowData.boss04_Cutscene);
    }

    // Update is called once per frame
    private void Update()
    {
        if (healthmanager.Boss_Defeat_Check())
        {
            target.GetComponentInChildren<CapsuleCollider2D>().enabled = false;
            //SteamAchievement.instance.Achieve("Clear_Final");
            currentState = State.Die;
        }

        if (target == null) target = GameObject.Find("Player(Clone)").gameObject;

        if (target.transform.GetComponent<Player_Manager>().Player_InputCheck() && !startCombat && isStand && dataManager.nowData.boss04_Cutscene)
        {
            isStand = false;
            anim.SetBool("isStand", false);
        }

        if (waitTracetimer > 0) waitTracetimer -= Time.deltaTime;

        if (attack01Timer > 0) attack01Timer -= Time.deltaTime;
        if (attack02Timer > 0) attack02Timer -= Time.deltaTime;
        if (attack03Timer > 0) attack03Timer -= Time.deltaTime;
        if (attack04Timer > 0) attack04Timer -= Time.deltaTime;
        if (attack05Timer > 0) attack05Timer -= Time.deltaTime;

        if (dashTimer > 0) dashTimer -= Time.deltaTime;
        if (dashMoveTimer > 0) dashMoveTimer -= Time.deltaTime;

        if (groggyTimer > 0) groggyTimer -= Time.deltaTime;



        if (healthmanager.currentHealth == healthmanager.groggyHealth)
        {
            if (!healthmanager.isGroggy && !phase02)
            {
                rigid.sharedMaterial = null;
                attack01Hitbox.SetActive(false);

                Set_Drag(5f);
                Reset_Gravity();
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
                    doAttack01 = false;
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
                    else if (CheckDistance() <= attack03Distance && attack03Timer <= 0)
                    {
                        currnetAttackNum = 3;
                        doAttack = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= attack04Distance && attack04Timer <= 0)
                    {
                        currnetAttackNum = 4;
                        doAttack = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= attack05Distance && attack05Timer <= 0)
                    {
                        currnetAttackNum = 5;
                        doAttack = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= dashDistance && dashTimer <= 0)
                    {
                        doDash = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isDash", true);
                        currentState = State.Dash;
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
                    else if (CheckDistance() <= attack03Distance && attack03Timer <= 0)
                    {
                        currnetAttackNum = 3;
                        doAttack = true;
                        anim.SetBool("isRun", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= attack04Distance && attack04Timer <= 0)
                    {
                        currnetAttackNum = 4;
                        doAttack = true;
                        anim.SetBool("isRun", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= attack05Distance && attack05Timer <= 0)
                    {
                        currnetAttackNum = 5;
                        doAttack = true;
                        anim.SetBool("isRun", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= dashDistance && dashTimer <= 0)
                    {
                        doDash = true;
                        anim.SetBool("isRun", false);
                        anim.SetBool("isDash", true);
                        currentState = State.Dash;
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

                case State.Dash:
                    Dash();
                    if (!doDash)
                    {
                        if (CheckDistance() <= minTraceDistance)
                        {
                            anim.SetBool("isDash", false);
                            anim.SetBool("isIdle", true);
                            currentState = State.Idle;
                        }
                        else
                        {
                            anim.SetBool("isDash", false);
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

    public bool CheckTargetBack()
    {
        return Physics2D.Raycast(targetDistanceCheck.position, -gameObject.transform.right * isRight, targetCheckDistance, whatIsTarget);
    }

    public float CheckDistance()
    {
        return Mathf.Abs(target.transform.position.x - transform.position.x);
    }

    public bool CheckAttack01Ground()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, ledgeCheckDistance, whatIsGround);
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
        if (doAttack01)
        {
            if (rigid.velocity.y > 0) Set_Gravity(upGravity);
            else Set_Gravity(dropGravity);

            if (!startAttack01) StartCoroutine(startDelayAttack01());

            // attack01 bounce 로직
            if (target.transform.position.x > transform.position.x)
            {
                if (rigid.velocity.x > 0)
                {
                    rigid.AddForce(new(Mathf.Lerp(0, attack01TracePower, CheckDistance() / attack01Distance), 0), ForceMode2D.Force);
                }
                else
                {
                    rigid.AddForce(new Vector2(attack01RejectPower, 0), ForceMode2D.Force);
                }
            }
            else
            {
                if (rigid.velocity.x < 0)
                {
                    rigid.AddForce(new(-Mathf.Lerp(0, attack01TracePower, CheckDistance() / attack01Distance), 0), ForceMode2D.Force);
                }
                else
                {
                    rigid.AddForce(new Vector2(-attack01RejectPower, 0), ForceMode2D.Force);
                }
            }

            if (!attack01Bounce && CheckAttack01Ground())   // 바운스 바닥 체크 
            {
                System_CameraShake.instance.Start_Shake_Camera(4, 0.1f);
                printVFX.Print_VFX(1);
                StartCoroutine(DelayCheckAttack01Ground());
                SoundManager.instance.SFXPlayer(soundManager.Get_AudioClip("Attack01_3_Slam"), gameObject.transform);

                if (attack01Endready) End_Attack01();
                else return;
            }
            else if (!attack01Bounce && healthmanager.takedamage)  // 피격 체크
            {
                float powerX = Random.Range(40f, 50f);

                if (transform.position.x >= target.transform.position.x)
                {
                    Debug.Log("좌측 튕겨내기");
                    rigid.velocity = new(0, rigid.velocity.y);
                    rigid.AddForce(new(powerX, 0), ForceMode2D.Impulse);
                }
                else
                {
                    Debug.Log("우측 튕겨내기");
                    rigid.velocity = new(0, rigid.velocity.y);
                    rigid.AddForce(new(-powerX, 0), ForceMode2D.Impulse);
                }

                StartCoroutine(DelayCheckAttack01Takedamage());

                if (attack01Endready) End_Attack01();
                else return;
            }
            // else if (bounceCounter >= maxBounceCount)
            // {
            //     if (CheckAttack01Ground() || healthmanager.takedamage)
            //     {
            //         Debug.Log("공격 끝");
            //         End_Attack01();
            //     }
            // }
        }
        else
        {
            Set_VelocityZero_CheckGround();
            Set_MoveSpeed(0);
        }
    }

    public IEnumerator startDelayAttack01()
    {
        attack01Bounce = true;
        yield return new WaitForSeconds(0.5f);
        startAttack01 = true;
        attack01Bounce = false;
    }

    public IEnumerator DelayCheckAttack01Ground()
    {
        if (!attack01Bounce)
        {
            attack01Bounce = true;
            bounceCounter++;
        }
        yield return new WaitForSeconds(0.5f);
        if (bounceCounter >= maxBounceCount)
        {
            attack01Endready = true;
            rigid.sharedMaterial = null;
        }
        attack01Bounce = false;
    }
    public IEnumerator DelayCheckAttack01Takedamage()
    {
        if (!attack01Bounce)
        {
            attack01Bounce = true;
            bounceCounter++;
            attack01Hitbox.SetActive(false);
        }
        yield return new WaitForSeconds(0.5f);
        if (bounceCounter >= maxBounceCount)
        {
            attack01Endready = true;
            rigid.sharedMaterial = null;
        }
        attack01Bounce = false;
        attack01Hitbox.SetActive(true);
    }

    public void Dash()
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

    #endregion stateControl

    private void Set_VelocityZero_CheckGround()
    {
        if (!CheckLedge() || CheckWall())
        {
            //rigid.velocity = Vector2.zero;
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
                break;

            case 3:
                attack03Timer = attackCoolTime[currnetAttackNum - 1];
                break;

            case 4:
                attack04Timer = attackCoolTime[currnetAttackNum - 1];
                break;

            case 5:
                attack05Timer = attackCoolTime[currnetAttackNum - 1];
                break;
        }
    }

    public IEnumerator Attack03_Trigger()
    {
        // 플레이어 위치에 포격 생성
        GameObject shelling = Instantiate(attack03Prefab, target.transform.position, Quaternion.identity);
        shelling.transform.position = new Vector2(shelling.transform.position.x, shelling.GetComponent<Enemy_AttackGroundCheck>().CheckAttackGround().y);

        int instanceCount = 0;

        while (attack03Counter > 0)
        {
            instanceCount++;
            Debug.Log("포격!");
            attack03Counter--;
            yield return new WaitForSeconds(attack03Delay);
            GameObject addShelling = Instantiate(attack03Prefab, shelling.transform.position, Quaternion.identity);
            if (instanceCount % 2 == 1) addShelling.transform.position = new Vector2(shelling.transform.position.x - attack03Interver * (instanceCount + 1) / 2, addShelling.GetComponent<Enemy_AttackGroundCheck>().CheckAttackGround().y); //홀수
            else addShelling.transform.position = new Vector2(shelling.transform.position.x + attack03Interver * instanceCount / 2, addShelling.GetComponent<Enemy_AttackGroundCheck>().CheckAttackGround().y); //짝수
        }
    }

    public void End_Attack()
    {
        Set_AttackCoolTime();
        doAttack = false;
        if (currnetAttackNum == 1) bounceCounter = 0;
        if (currnetAttackNum == 3) attack03Counter = attack03Count;
        else return;
    }

    public void Start_Attack01()    // attack01 bounce 공격 진입
    {
        anim.SetTrigger("doAttack01");
        attack01Hitbox.SetActive(true);
        doAttack01 = true;
        rigid.sharedMaterial = rbMaterial;
        rigid.AddForce(new(0, attack01JumpPower), ForceMode2D.Impulse);
    }

    public void End_Attack01()      // attack01 bounce 공격 끝
    {
        StopAllCoroutines();
        anim.SetTrigger("endAttack01");
        attack01Hitbox.SetActive(false);
        doAttack01 = false;
        startAttack01 = false;
        attack01Endready = false;
    }

    public void Start_Dash()
    {
        if (!startDash)
        {
            startDash = true;
            SoundManager.instance.SFXPlayer(soundManager.Get_AudioClip("Attack_Dash_Dash"), gameObject.transform);
            dashMoveTimer = dashMoveTime;
            anim.SetTrigger("startDash");
            StartCoroutine(Do_Dash());
            purpleHitbox.SetActive(true);
            redHitbox.SetActive(true);
        }
    }

    public IEnumerator Do_Dash()
    {
        while (dashMoveTimer > 0)
        {
            yield return null;
            transform.Translate(isRight * dashSpeed * Vector2.right * Time.deltaTime);
        }

        if (dashMoveTimer <= 0 && !endDash)
        {
            endDash = true;
            anim.SetTrigger("endDash");
            purpleHitbox.SetActive(false);
            redHitbox.SetActive(false);
        }
    }

    public void End_Dash()
    {
        dashTimer = dashTime;
        doDash = false;
        startDash = false;
        endDash = false;
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

    public void Spawn_Axtra_Missile()
    {
        GameObject missile = Instantiate(axtraMissilePrefab, missilePos.position, Quaternion.identity);
        missile.GetComponent<Boss_Master_AxtraMissle_Manager>().Set_Parent_Transform(transform);
    }

    public void Start_Combat()
    {
        anim.SetTrigger("CombatReady");
        startCombat = true;
    }
}
