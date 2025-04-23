using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class Boss_CEO_Manager : MonoBehaviour
{
    private enum State
    {
        Idle,
        Run,
        Attack,
        Dash,
        Teleport,
        Backdash,
        Parry,
        Groggy,
        Die
    }

    private Rigidbody2D rigid;
    private Animator anim;
    private GameObject target;
    private Boss_Health_Manager healthmanager;
    private DataManager dataManager;
    private float isRight = -1;
    [SerializeField] private GameObject enemyHurt;

    [Space(10f)]
    private Light2D vfxLight;

    [SerializeField] private State currentState;
    [Space(10f)]
    [Header("LayerMask")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsTarget;

    [Space(10f)]
    [Header("DistanceCheck")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;

    [Space(10f)]
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private Transform ledgeCheckBack;
    [SerializeField] private float ledgeCheckDistance;

    [Space(10f)]
    [SerializeField] private Transform targetDistanceCheck;
    [SerializeField] private float targetCheckDistance;

    [Space(10f)]
    [SerializeField] private Transform teleportGroundPointCheck;
    [SerializeField] private float teleportGroundPointDistance;

    [Space(10f)]
    [SerializeField] private float minTraceDistance;
    [SerializeField] private float minAttackDistance;

    [Space(10f)]
    [SerializeField] private List<float> attackDistance;

    [Space(10f)]
    [SerializeField] private float backdashDistance;
    [SerializeField] private float dashDistance;
    [SerializeField] private float teleportDistance;

    [Space(10f)]
    [SerializeField] private BoxCollider2D rangeCollider;
    [SerializeField] private float teleportMinDistance;
    [SerializeField] private float teleportMaxDistance;
    [SerializeField] private float teleportBanishTurm;

    [Space(10f)]
    [Header("Status")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float defaultGravity;
    [SerializeField] private float defaultDrag;
    [SerializeField] private float attack03Speed;



    [Space(10f)]
    [SerializeField] private GameObject attackProjectile;
    [SerializeField] private GameObject attackMuzzle;

    [Space(10f)]
    [SerializeField] private float startLightLerpSpeed;
    [SerializeField] private float endLightLerpSpeed;
    [SerializeField] private GameObject attack03Prefab;
    [SerializeField] private GameObject attack03Particle;
    [SerializeField] private GameObject attack03Muzzle;

    [Space(10f)]
    [SerializeField] private int maxDronCount;
    [SerializeField] private GameObject attack04Prefab;

    [SerializeField] private List<GameObject> dronDestroyList;

    [Space(10f)]
    [SerializeField] private List<Transform> attackPos;
    [SerializeField] private float bulletSpeed;
    private int currnetAttackNum;

    [Space(10f)]
    [SerializeField] private GameObject parryActiveParticle;
    [SerializeField] private Transform parryParticlePos;
    [SerializeField] private GameObject parryParticle;
    [Space(10f)]

    [Space(10f)]
    [Header("Timer")]
    [SerializeField] private float waitTraceTime;

    [Space(10f)]
    [SerializeField] private List<float> attackCoolTime;
    [SerializeField] private float attack03startTurm;
    [SerializeField] private float attack03Duration;

    [Space(10f)]
    [SerializeField] private float dashTime;
    [SerializeField] private float backdashTime;
    [SerializeField] private float teleportTime;
    [SerializeField] private float groggyTime;
    [SerializeField] private float parryStopTime;


    private bool isStand = true;

    private int dronCount = 0;

    private float waitTracetimer;
    private float attack01Timer;
    private float attack02Timer;
    private float attack03Timer;
    private float attack04Timer;

    private float dashTimer;
    private float backdashTimer;
    private float teleportTimer;

    private float groggyTimer;
    private float parryStopTimer;

    private bool startCombat = false;
    private bool doAttack = false;
    private bool dodash = false;
    private bool doBackdash = false;
    private bool doTeleport = false;
    private bool endGroggy = false;
    [Space(10f)]
    public bool phase02 = false;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        healthmanager = gameObject.GetComponent<Boss_Health_Manager>();
        vfxLight = GameObject.FindObjectOfType<Light2D>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        rangeCollider = GameObject.Find("Boss03_Boundary").GetComponent<BoxCollider2D>();


        defaultGravity = rigid.gravityScale;
        defaultDrag = rigid.drag;
        currentState = State.Idle;

        anim.SetBool("Dialogue", dataManager.nowData.boss03_Dialogue);
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) target = GameObject.Find("Player(Clone)").gameObject;

        if (healthmanager.Boss_Defeat_Check())
        {
            Stop_All_Anim();
            //SteamAchievement.instance.Achieve("Clear_Fullmetal");
            target.GetComponentInChildren<CapsuleCollider2D>().enabled = false;

            if (dronCount > 0) dronDestroyList.AddRange(GameObject.FindGameObjectsWithTag("Enemy_Dron"));
            for (int i = 0; i < dronDestroyList.Count; i++)
            {
                dronDestroyList[i].GetComponent<Enemy_HealthManager>().Enemy_TakeDamage(1);
            }
            currentState = State.Die;
        }

        if (healthmanager.Return_ParryActive()) parryActiveParticle.SetActive(true);
        else parryActiveParticle.SetActive(false);

        if (healthmanager.doParry)
        {
            anim.SetBool("isParry", true);
            currentState = State.Parry;
        }


        if (target.transform.GetComponent<Player_Manager>().Player_InputCheck() && !startCombat && isStand && dataManager.nowData.boss03_Dialogue)
        {
            isStand = false;
            anim.SetBool("isStand", false);
        }

        waitTracetimer -= Time.deltaTime;

        attack01Timer -= Time.deltaTime;
        attack02Timer -= Time.deltaTime;
        attack03Timer -= Time.deltaTime;
        attack04Timer -= Time.deltaTime;

        dashTimer -= Time.deltaTime;
        backdashTimer -= Time.deltaTime;
        teleportTimer -= Time.deltaTime;

        groggyTimer -= Time.deltaTime;

        parryStopTimer -= Time.deltaTime;

        // if (!CheckLedge_Back() || !CheckLedge() || CheckWall())
        // {
        //     rigid.velocity = Vector2.zero;
        // }

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
            Debug.Log("전투는 진행중");
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
                    else if (CheckDistance() <= attackDistance[0] && CheckDistance() > minAttackDistance && attack01Timer <= 0)
                    {
                        currnetAttackNum = 1;
                        doAttack = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= attackDistance[1] && CheckDistance() > minAttackDistance && attack02Timer <= 0)
                    {
                        currnetAttackNum = 2;
                        doAttack = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (phase02 && CheckDistance() <= attackDistance[2] && CheckDistance() > minAttackDistance && attack03Timer <= 0)
                    {
                        currnetAttackNum = 3;
                        doAttack = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (dronCount < maxDronCount && CheckDistance() <= attackDistance[3] && CheckDistance() > minAttackDistance && attack04Timer <= 0)
                    {
                        currnetAttackNum = 4;
                        doAttack = true;
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (!CheckWall() && CheckDistance() <= backdashDistance && backdashTimer <= 0 && !doBackdash)
                    {
                        doBackdash = true;
                        enemyHurt.SetActive(false);
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isBackdash", true);
                        currentState = State.Backdash;
                    }
                    else if (!CheckWall() && CheckDistance() <= dashDistance && dashTimer <= 0 && !dodash)
                    {
                        dodash = true;
                        enemyHurt.SetActive(false);
                        int dashRand = Random.Range(1, 3);
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isDash", true);
                        anim.SetInteger("DashNum", dashRand);
                        currentState = State.Dash;
                    }
                    else if (CheckDistance() <= teleportDistance && teleportTimer <= 0 && !doTeleport)
                    {
                        doTeleport = true;
                        enemyHurt.SetActive(false);
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isTeleport", true);
                        currentState = State.Teleport;
                    }
                    else if (CheckWall())
                    {
                        doTeleport = true;
                        enemyHurt.SetActive(false);
                        anim.SetBool("isIdle", false);
                        anim.SetBool("isTeleport", true);
                        currentState = State.Teleport;
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
                    else if (CheckDistance() <= attackDistance[0] && CheckDistance() > minAttackDistance && attack01Timer <= 0)
                    {
                        currnetAttackNum = 1;
                        doAttack = true;
                        anim.SetBool("isRun", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (CheckDistance() <= attackDistance[1] && CheckDistance() > minAttackDistance && attack02Timer <= 0)
                    {
                        currnetAttackNum = 2;
                        doAttack = true;
                        anim.SetBool("isRun", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (phase02 && CheckDistance() <= attackDistance[2] && CheckDistance() > minAttackDistance && attack03Timer <= 0)
                    {
                        currnetAttackNum = 3;
                        doAttack = true;
                        anim.SetBool("isRun", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (dronCount < maxDronCount && CheckDistance() <= attackDistance[3] && CheckDistance() > minAttackDistance && attack04Timer <= 0)
                    {
                        currnetAttackNum = 4;
                        doAttack = true;
                        anim.SetBool("isRun", false);
                        anim.SetBool("isAttack", true);
                        anim.SetInteger("AttackNum", currnetAttackNum);
                        currentState = State.Attack;
                    }
                    else if (!CheckWall() && CheckDistance() <= backdashDistance && backdashTimer <= 0 && !doBackdash)
                    {
                        doBackdash = true;
                        enemyHurt.SetActive(false);
                        anim.SetBool("isRun", false);
                        anim.SetBool("isBackdash", true);
                        currentState = State.Backdash;
                    }
                    else if (!CheckWall() && CheckDistance() <= dashDistance && dashTimer <= 0 && !dodash)
                    {
                        dodash = true;
                        enemyHurt.SetActive(false);
                        int dashRand = Random.Range(1, 3);
                        anim.SetBool("isRun", false);
                        anim.SetBool("isDash", true);
                        anim.SetInteger("DashNum", dashRand);
                        currentState = State.Dash;
                    }
                    else if (CheckDistance() <= teleportDistance && teleportTimer <= 0 && !doTeleport)
                    {
                        doTeleport = true;
                        enemyHurt.SetActive(false);
                        anim.SetBool("isRun", false);
                        anim.SetBool("isTeleport", true);
                        currentState = State.Teleport;
                    }
                    else if (CheckWall())
                    {
                        doTeleport = true;
                        enemyHurt.SetActive(false);
                        anim.SetBool("isRun", false);
                        anim.SetBool("isTeleport", true);
                        currentState = State.Teleport;
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
                    if (!dodash)
                    {
                        if (CheckDistance() <= minTraceDistance)
                        {
                            anim.SetBool("isDash", false);
                            anim.SetBool("isIdle", true);
                            currentState = State.Idle;
                        }
                        else if (CheckDistance() > minTraceDistance)
                        {
                            anim.SetBool("isDash", false);
                            anim.SetBool("isRun", true);
                            currentState = State.Run;
                        }

                    }
                    else if (dodash)
                    {
                        if (CheckWall())
                        {
                            rigid.velocity = Vector2.zero;
                            doTeleport = true;
                            enemyHurt.SetActive(false);
                            anim.SetBool("isDash", false);
                            anim.SetBool("isTeleport", true);
                            End_Dash();
                            Reset_Drag();
                            currentState = State.Teleport;
                        }
                    }
                    else if (healthmanager.isGroggy)
                    {
                        anim.SetBool("isAttack", false);
                        currentState = State.Groggy;
                    }
                    break;

                case State.Backdash:
                    Backdash();
                    if (!doBackdash)
                    {
                        if (CheckDistance() <= minTraceDistance)
                        {
                            anim.SetBool("isBackdash", false);
                            anim.SetBool("isIdle", true);
                            currentState = State.Idle;
                        }
                        else
                        {
                            anim.SetBool("isBackdash", false);
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

                case State.Teleport:
                    Teleport();
                    if (!doTeleport)
                    {
                        if (CheckDistance() <= minTraceDistance)
                        {
                            anim.SetBool("isBackdash", false);
                            anim.SetBool("isIdle", true);
                            currentState = State.Idle;
                        }
                        else
                        {
                            anim.SetBool("isBackdash", false);
                            anim.SetBool("isRun", true);
                            currentState = State.Run;
                        }
                    }
                    break;

                case State.Parry:
                    Parry();
                    if (parryStopTimer <= 0)
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

    public bool CheckLedge_Back()
    {
        return Physics2D.Raycast(ledgeCheckBack.position, Vector2.down, ledgeCheckDistance, whatIsGround);
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
        return Physics2D.Raycast(teleportGroundPointCheck.position, Vector2.down, teleportGroundPointDistance, whatIsGround).point;
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

    public void Dash()
    {
        Set_MoveSpeed(0);
    }

    public void Backdash()
    {
        Set_MoveSpeed(0);
    }

    public void Teleport()
    {
        Set_MoveSpeed(0);
    }

    public void Parry()
    {
        Stop_All_Anim();
        rigid.velocity = Vector2.zero;
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

    #endregion

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
        }
    }

    public void Set_Attack03_Light()
    {
        StartCoroutine(Set_Light_Lerp());
    }

    private IEnumerator Set_Light_Lerp()
    {
        float timer01 = 0;

        while (timer01 <= 1)
        {
            yield return null;
            timer01 += Time.unscaledDeltaTime * startLightLerpSpeed;
            vfxLight.color = new Color(Mathf.Lerp(1f, 0.3f, timer01), Mathf.Lerp(1f, 0.2f, timer01), Mathf.Lerp(1f, 0.2f, timer01));
        }

        yield return new WaitForSeconds(0.5f);

        float timer02 = 0;

        while (timer02 <= 1)
        {
            yield return null;
            timer02 += Time.unscaledDeltaTime * endLightLerpSpeed;
            vfxLight.color = new Color(Mathf.Lerp(0.3f, 1f, timer02), Mathf.Lerp(0.2f, 1f, timer02), Mathf.Lerp(0.2f, 1f, timer02));
        }
    }

    public void Attack03(int posNum)
    {
        //조명도 어둡게 했다가 돌아오도록 할것
        Instantiate(attack03Muzzle, attackPos[posNum].position, Quaternion.identity);
        Instantiate(attack03Particle, attackPos[posNum].position, Quaternion.identity);
        Attack03_Instance_Laser();
    }

    private void Attack03_Instance_Laser()
    {
        GameObject laser = Instantiate(attack03Prefab, target.transform.position, Quaternion.identity);
        laser.GetComponent<Boss_CEO_Laser_Manager>().Set_LaserParameter(attack03startTurm, attack03Duration, target, attack03Speed);
        laser.transform.position = new Vector2(laser.transform.position.x, laser.GetComponent<Boss_CEO_Laser_Manager>().Get_Laser_Ground_Point().y);
    }

    public void End_Attack()
    {
        Set_AttackCoolTime();
        doAttack = false;
        Debug.Log("공격 끝");
    }

    public void Teleport_Trigger()
    {
        StartCoroutine(Teleport_Logic());
        healthmanager.Set_Parry_deActivate();
    }

    private IEnumerator Teleport_Logic()
    {
        yield return new WaitForSeconds(teleportBanishTurm);

        // 정해진 범위내에서 플레이어로 부터 일정거리로 이동
        do
        {
            transform.position = Return_RandomPosition();
            transform.position = new(transform.position.x, Return_GroundPoint().y);

            InfiniteLoopDetector.Run();
        } while (CheckDistance() < teleportMinDistance || CheckDistance() > teleportMaxDistance);

        anim.SetTrigger("TeleportTrigger");

        if (phase02)
        {
            if (dronCount < maxDronCount && attack04Timer <= 0)
            {
                Summon_Dron(3);
                attack04Timer = attackCoolTime[3];
            }
        }
    }

    private Vector3 Return_RandomPosition()
    {
        Vector3 originPosition = rangeCollider.transform.position;
        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = rangeCollider.bounds.size.x;
        float range_Y = rangeCollider.bounds.size.y;

        range_X = Random.Range(-(range_X / 2), range_X / 2);
        range_Y = Random.Range(-(range_Y / 2), range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y, 0f);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }

    public void Instantiate_Bullet(int posNum)
    {
        GameObject bullet = Instantiate(attackProjectile, attackPos[posNum].position, Quaternion.identity);
        GameObject muzzle = Instantiate(attackMuzzle, attackPos[posNum].position, Quaternion.identity);

        bullet.transform.rotation = transform.rotation;
        bullet.transform.localScale = -transform.localScale;
        bullet.GetComponent<Enemy_projectile>().Set_BulletSpeed(bulletSpeed);
        bullet.GetComponent<Enemy_projectile_Hitbox>().Set_Parent_Transform(transform);

        muzzle.transform.rotation = transform.rotation;
        muzzle.transform.localScale = transform.localScale;
    }

    public void Summon_Dron(int posNum)
    {
        dronCount++;

        GameObject dron = Instantiate(attack04Prefab, attackPos[posNum].position, Quaternion.identity);

        // 드론 소팅레이어 설정 > 안하면 겹쳤을때 이상해짐
        if (dronCount < maxDronCount)
        {
            for (int i = 0; i < 5; i++)
            {
                dron.GetComponentsInChildren<SpriteRenderer>()[i].sortingOrder += 5 * dronCount;
            }
        }
        else return;

        dron.AddComponent<Enemy_Dron_Spawned>();
        dron.GetComponent<Enemy_Dron_Spawned>().Set_CEO_Manager(transform.GetComponent<Boss_CEO_Manager>());
    }

    public void End_Dash()
    {
        dashTimer = dashTime;
        dodash = false;
        enemyHurt.SetActive(true);
    }
    public void End_Backdash()
    {
        backdashTimer = backdashTime;
        doBackdash = false;
        enemyHurt.SetActive(true);
    }

    public void End_Teleport()
    {
        teleportTimer = teleportTime;
        doTeleport = false;
        enemyHurt.SetActive(true);
        healthmanager.Set_Parry_Activate();
        anim.SetBool("isTeleport", false);
    }

    public void End_Parry()
    {
        parryStopTimer = parryStopTime;
        healthmanager.doParry = false;
        anim.SetBool("isParry", false);
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

    public void Print_ParryVFX()
    {
        Instantiate(parryParticle, parryParticlePos.position, Quaternion.identity);
    }

    public void Count_Dron_Minuse()
    {
        dronCount--;
    }


    private void Stop_All_Anim()
    {
        anim.SetBool("isIdle", false);
        anim.SetBool("isRun", false);
        anim.SetBool("isDash", false);
        anim.SetBool("isBackdash", false);
        anim.SetBool("isAttack", false);

        Reset_Drag();
        Reset_Gravity();

        End_Attack();
        End_Dash();
        End_Backdash();
    }

    public void Start_Combat()
    {
        anim.SetTrigger("CombatReady");
        startCombat = true;
    }
}
