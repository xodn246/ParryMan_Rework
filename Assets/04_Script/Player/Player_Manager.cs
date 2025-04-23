using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Manager : MonoBehaviour
{
    private GameManager gameManager;
    private Player_Health_Manager healthManager;
    private Player_SpawnManager spawnManager;
    private Object_SoundManager soundManager;
    private Player_ParryCooltimeUI cooltimeUI;
    private Rigidbody2D rigid;
    private Animator anim;
    private BoxCollider2D groundCehckBox;
    private PauseMenu pauseMenu;

    public Queue<string> jumpQueue = new();
    public Queue<string> parryQueue = new();
    private bool jumpDequeue = false;
    private bool parryDequeue = false;
    private bool canInputJumpBuffer = false;
    private bool canInputParryBuffer = false;
    private float inputTimer;
    private float inputTime = 0.05f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    public int platformNum;

    [Space(10f)]
    [SerializeField] private float movePower;

    private Vector2 moveVelocity;
    [SerializeField] private float jumpPower;
    private float jumpCheckTimer;
    private int jumpCounter = 0;
    private bool doJump = false;
    public bool onAir = false;
    public bool readyParry = false;
    public bool isParry = false;
    public bool isJumpParry = false;
    [SerializeField] private float successTime = 0f;
    [SerializeField] private float parryCoolTime = 1f;
    private float parryCoolTimer;
    [SerializeField] private float parryReadyTime;
    private float parryReadyTimer = 0;
    [SerializeField] private float coyoteTime;
    private float coyoteTimer;

    private bool crystalParry = false;
    [SerializeField] private float crystalMaxSpeed;

    // ---------------------------------------------------- < input manager 테스트> ----------------------------------------------
    private float inputAxis;

    private bool inputJump;
    private bool inputParry;
    public PlayerInput playerinput;

    // Start is called before the first frame update
    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        healthManager = transform.GetComponent<Player_Health_Manager>();
        soundManager = transform.GetComponent<Object_SoundManager>();
        cooltimeUI = transform.GetComponentInChildren<Player_ParryCooltimeUI>();
        pauseMenu = GameObject.FindObjectOfType<PauseMenu>().GetComponent<PauseMenu>();
        spawnManager = GameObject.FindObjectOfType<Player_SpawnManager>().GetComponent<Player_SpawnManager>();

        rigid = transform.GetComponent<Rigidbody2D>();
        anim = transform.GetComponent<Animator>();
        groundCehckBox = transform.Find("Check_Ground_Collider").GetComponent<BoxCollider2D>();
        coyoteTimer = coyoteTime;

        playerinput = transform.GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        if (!gameManager.Active_Dialogue && !gameManager.camMoveCheck && !PauseMenu.GameIsPaused)
        {
            if (!gameManager.PlayerDie && !gameManager.LoadScene)
            {
                if (!crystalParry)
                {
                    Player_Move();
                    Player_Jump();
                }
                else
                {
                    Player_Move_Crystal();
                }
            }
        }

        if (Mathf.Abs(rigid.velocity.x) > 0.5f || Mathf.Abs(rigid.velocity.y) > 0.5f) gameManager.playerMove = true;
        else gameManager.playerMove = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (jumpQueue.Count != 0 && !jumpDequeue) StartCoroutine(Dequeue_JumpQueue());
        if (parryQueue.Count != 0 && !parryDequeue) StartCoroutine(Dequeue_ParryQueue());

        if (canInputParryBuffer) inputTimer -= Time.deltaTime;
        parryCoolTimer -= Time.deltaTime;
        if (!Check_Ground()) coyoteTimer -= Time.deltaTime;
        jumpCheckTimer -= Time.deltaTime;

        parryReadyTimer -= Time.deltaTime;
        if (parryReadyTimer <= 0) End_Ready_Parry();

        if (Check_Ground() && jumpCheckTimer <= 0)
        {
            jumpCounter = 0;
            if (crystalParry) crystalParry = false;
            coyoteTimer = coyoteTime;

            //----------- 점프 선입력 -----------------------------------------------------------------------
            canInputJumpBuffer = false;     // 점프큐 입력가능 종료
            Do_JumpBufferAction();  // 점프 선입력 실행
        }

        if (parryCoolTimer <= 0) canInputParryBuffer = false;

        if (parryQueue.Count != 0 && parryCoolTimer < 0)   //패링 선입력 실행
        {
            Do_ParryBufferAction();
        }

        if (!gameManager.Active_Dialogue && !gameManager.camMoveCheck && !PauseMenu.GameIsPaused)
        {
            if (!gameManager.PlayerDie)
            {
                if (jumpCounter == 0 && !readyParry && !doJump && !isParry && inputJump)
                {
                    if (!onAir || onAir && coyoteTimer >= 0)
                    {
                        jumpCounter++;
                        doJump = true;
                        jumpCheckTimer = 0.1f;
                    }
                }
                if (inputJump && inputParry && !readyParry && parryCoolTimer <= 0)
                {
                    anim.SetBool("isParry", false);
                    anim.SetBool("isJumpParry", false);
                    isParry = false;
                    isJumpParry = false;
                    canInputJumpBuffer = false; // 패링 입력시 점프 선입력 종료
                    canInputParryBuffer = true;
                    inputTimer = inputTime;

                    if (onAir) JumpParry();
                }
                else if (inputParry && onAir && !readyParry && parryCoolTimer <= 0)
                {
                    anim.SetBool("isParry", false);
                    anim.SetBool("isJumpParry", false);
                    isParry = false;
                    isJumpParry = false;
                    canInputJumpBuffer = false; // 패링 입력시 점프 선입력 종료
                    canInputParryBuffer = true;
                    inputTimer = inputTime;
                    JumpParry();
                }
                else if (inputParry && !onAir && !readyParry && parryCoolTimer <= 0)
                {
                    anim.SetBool("isParry", false);
                    anim.SetBool("isJumpParry", false);
                    isParry = false;
                    isJumpParry = false;
                    canInputParryBuffer = true;
                    inputTimer = inputTime;
                    Parry();
                }

                if (canInputJumpBuffer)
                {
                    if (inputJump)
                    {
                        jumpQueue.Enqueue("c");
                    }
                }

                if (inputTimer <= 0 && canInputParryBuffer)
                {
                    if (inputJump)
                    {
                        parryQueue.Enqueue("c");
                    }
                    else if (inputParry)
                    {
                        parryQueue.Enqueue("z");
                    }
                }
            }
        }
        else
        {
            if (!PauseMenu.GameIsPaused)
            {
                Set_Idle();
                rigid.velocity = new(0, rigid.velocity.y);
                //rigid.velocity = Vector2.zero;
            }
        }

        OnAir_Manage();
    }

    // ================================================================= < Move Text > =================================================================

    private void Player_Move()
    {
        float inputRaw = inputAxis;
        moveVelocity = Vector2.zero;
        if (!isParry)
        {
            if (inputRaw > 0)
            {
                if ((readyParry && !onAir) || isParry) moveVelocity = Vector2.zero;
                else moveVelocity = Vector2.right * movePower;
                transform.localScale = new Vector3(-1, 1, 1);
                if (!readyParry)
                {
                    anim.SetBool("isIdle", false);
                    anim.SetBool("isRun", true);
                }
            }
            else if (inputRaw < 0)
            {
                if ((readyParry && !onAir) || isParry)
                {
                    moveVelocity = Vector2.zero;
                }
                else moveVelocity = Vector2.left * movePower;
                transform.localScale = new Vector3(1, 1, 1);
                if (!readyParry)
                {
                    anim.SetBool("isIdle", false);
                    anim.SetBool("isRun", true);
                }
            }
            else
            {
                moveVelocity = Vector2.zero;
                if (!onAir && !readyParry) anim.SetBool("isIdle", true);
                else anim.SetBool("isIdle", false);
                anim.SetBool("isRun", false);
            }
        }
        rigid.velocity = new Vector2(moveVelocity.x, rigid.velocity.y);
    }

    private void Player_Move_Crystal()
    {
        float inputRaw = inputAxis;
        Vector2 moveDetale = new(inputRaw * movePower, 0);

        //Debug.Log("x속도 : " + rigid.velocity.x);

        if (inputRaw > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            if (rigid.velocity.x < crystalMaxSpeed) rigid.AddForce(moveDetale, ForceMode2D.Force);
            else rigid.velocity = new Vector2(crystalMaxSpeed, rigid.velocity.y);
        }
        else if (inputRaw < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            if (rigid.velocity.x > -crystalMaxSpeed) rigid.AddForce(moveDetale, ForceMode2D.Force);
            else rigid.velocity = new Vector2(-crystalMaxSpeed, rigid.velocity.y);
        }
        else return;
    }

    private void Player_Jump()
    {
        Vector2 setVelocity = rigid.velocity;
        if (doJump)
        {
            canInputJumpBuffer = true;  // 점프 버퍼 입력활성화

            SoundManager.instance.SFXPlayer(/*"Jump",*/ soundManager.Get_AudioClip("Jump"), gameObject.transform);
            anim.SetBool("isIdle", false);
            anim.SetBool("isRun", false);
            anim.SetBool("isJump", true);
            doJump = false;

            rigid.velocity = new Vector2(setVelocity.x, 0);
            rigid.AddRelativeForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    //=========================== new input system 키입력 방식 ===============================================
    public void ActionMove(InputAction.CallbackContext context)
    {
        inputAxis = context.ReadValue<float>();
    }

    public void ActionJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            inputJump = true;
            StartCoroutine(End_PressJump());
        }
    }

    public void ActionParry(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            inputParry = true;
            StartCoroutine(End_PressParry());
        }
    }

    public void ActionRespawn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            spawnManager.inputRespawn = true;
            StartCoroutine(End_PressRespawn());
        }
    }

    public void ActionPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            pauseMenu.inputPause = true;
        }
        if (context.canceled)
        {
            pauseMenu.inputPause = false;
            pauseMenu.doPause = false;
        }
    }

    private IEnumerator End_PressJump()
    {
        yield return new WaitForSeconds(0.01f);
        inputJump = false;
    }

    private IEnumerator End_PressParry()
    {
        yield return new WaitForSeconds(0.01f);
        inputParry = false;
    }

    public void End_PressParry_WhenParry()
    {
        inputParry = false;
    }

    private IEnumerator End_PressRespawn()
    {
        yield return new WaitForSeconds(0.01f);
        spawnManager.inputRespawn = false;
    }

    public bool Player_InputCheck()
    {
        if (inputAxis != 0) return true;
        else if (Input.anyKey) return true;
        else return false;
    }

    //============================================================================================================================================

    private void Set_GravityScale(float scale)
    {
        rigid.gravityScale = scale;
    }

    private bool Check_Ground()
    {
        return Physics2D.BoxCast(groundCehckBox.bounds.center, groundCehckBox.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
    }

    private void Check_Platform() // 플렛폼 번호 식별용 코드 > 적과 다른 플렛폼으로 이동시 적 추적 종료하기위해 추가 >> 나중에 필요하면 쓰도록 하자 당장은 필요는 없음
    {
        var hit = Physics2D.BoxCast(groundCehckBox.bounds.center, groundCehckBox.bounds.size, 0f, Vector2.down, 1f, platformLayer);

        if (hit.collider != null)
        {
            platformNum = hit.collider.transform.GetComponent<Map_SortPlatform>().platformNum;
        }
    }

    private void OnAir_Manage()
    {
        if (rigid.velocity.y < 0)
        {
            Set_GravityScale(6.5f);
            if (!readyParry)
            {
                anim.SetBool("isJump", false);
                anim.SetBool("isDrop", true);
            }
            else anim.SetBool("isDrop", false);
        }

        if (!Check_Ground())
        {
            onAir = true;
            anim.SetBool("onAir", true);
        }
        else if (Check_Ground() && rigid.velocity.y > 0.001f)
        {
            onAir = true;
            anim.SetBool("onAir", true);
        }
        else
        {
            Set_GravityScale(5f);
            anim.SetBool("isDrop", false);
            anim.SetBool("onAir", false);
            onAir = false;
        }
    }

    private void Parry()
    {
        anim.SetBool("parryReady", true);
        parryReadyTimer = parryReadyTime;
        Ready_Parry02();
        //StartCoroutine(Ready_Parry());
    }

    private void JumpParry()
    {
        anim.SetBool("jumpReady", true);
        parryReadyTimer = parryReadyTime;
        Ready_Parry02();
        //StartCoroutine(Ready_Parry());
    }

    private IEnumerator Ready_Parry()       // 코루틴 쓴거
    {
        readyParry = true;
        anim.SetBool("isJump", false);
        anim.SetBool("isIdle", false);
        anim.SetBool("isRun", false);
        anim.SetBool("isDrop", false);
        healthManager.Set_InvincibleTime_Zero();
        Set_ParryCoolTime(parryCoolTime);
        yield return new WaitForSeconds(parryReadyTime);
        readyParry = false;
        OnAir_Manage();
        anim.SetBool("parryReady", false);
        anim.SetBool("jumpReady", false);
        cooltimeUI.Print_Cooltime_UI();
    }

    private void Ready_Parry02()        // 타이머 쓴거
    {
        readyParry = true;
        anim.SetBool("isJump", false);
        anim.SetBool("isIdle", false);
        anim.SetBool("isRun", false);
        anim.SetBool("isDrop", false);
        healthManager.Set_InvincibleTime_Zero();
        Set_ParryCoolTime(parryCoolTime);
    }

    private void End_Ready_Parry()      // 타이머 쓴거 끝낼때
    {
        readyParry = false;
        OnAir_Manage();
        anim.SetBool("parryReady", false);
        anim.SetBool("jumpReady", false);
        cooltimeUI.Print_Cooltime_UI();

        if (parryQueue.Count != 0) Delete_ParryBuffer();
    }

    public void Set_ParryCoolTime(float coolTime)
    {
        parryCoolTimer = coolTime;
    }

    public void Set_SuccessParryCoolTime()
    {
        parryCoolTimer = successTime;
    }

    private void Set_Idle()
    {
        anim.SetBool("isRun", false);
        anim.SetBool("isJump", false);
        anim.SetBool("parryReady", false);
        anim.SetBool("jumpReady", false);
        anim.SetBool("isDrop", false);
        anim.SetBool("isIdle", true);
    }

    public float Get_ParryCooltime()
    {
        return parryCoolTimer;
    }

    public void Set_Crystal_True()
    {
        rigid.velocity = Vector2.zero;
        jumpCounter++;
        jumpCheckTimer = 0.1f;
        crystalParry = true;
    }

    public bool Get_CrystalParry()
    {
        return crystalParry;
    }

    private IEnumerator Dequeue_JumpQueue()
    {
        jumpDequeue = true;
        yield return new WaitForSeconds(0.2f);
        if (jumpQueue.Count != 0) jumpQueue.Clear();
        jumpDequeue = false;
    }

    private IEnumerator Dequeue_ParryQueue()
    {
        parryDequeue = true;
        yield return new WaitForSeconds(0.2f);
        if (parryQueue.Count != 0) parryQueue.Dequeue();
        parryDequeue = false;
    }

    public void Delete_JumpBuffer() // 모든 선입력큐 초기화 : 점프시작 or 패링시작 시 (애니메이션에서 실행)
    {
        jumpQueue.Clear();
        parryQueue.Clear();
    }

    public void Delete_ParryBuffer()
    {
        parryQueue.Clear();
    }

    public void Do_JumpBufferAction()   // 점프 선입력 실행
    {
        if (jumpQueue.Count != 0)
        {
            if (jumpQueue.Dequeue() == "c")
            {
                jumpCounter++;
                doJump = true;
                jumpCheckTimer = 0.1f;
            }
            else return;

            if (parryQueue.Count != 0) Delete_ParryBuffer();
            if (jumpQueue.Count != 0) Delete_JumpBuffer();
        }
        else return;
    }

    public void Do_ParryBufferAction()  // 패링 선입력 실행
    {
        if (parryQueue.Count != 0)
        {
            string action = parryQueue.Dequeue();
            if (jumpCounter < 1 && !isParry && action == "c")
            {
                jumpCounter++;
                doJump = true;
                jumpCheckTimer = 0.1f;
            }
            else if (parryCoolTimer <= 0 && action == "z")
            {
                isParry = false;
                isJumpParry = false;
                anim.SetBool("isParry", false);
                anim.SetBool("isJumpParry", false);
                if (onAir) JumpParry();
                else Parry();
            }
            else return;

            if (parryQueue.Count != 0) Delete_ParryBuffer();
            if (jumpQueue.Count != 0) Delete_JumpBuffer();
        }
        else return;
    }

    public void OnControlsChanged()
    {
        GameManager.instance.Set_CurrentControlScheme(playerinput.currentControlScheme);
        //Debug.Log("ControlChanged : " + playerinput.currentControlScheme);
    }
}