using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem.Demo;
using UnityEngine;

public class Boss_Shotgun_Manager : MonoBehaviour
{
    private enum State
    {
        Idle,
        Attack,
        Die
    }

    private Rigidbody2D rigid;
    private Animator anim;
    private GameObject target;
    private Boss_Health_Manager healthmanager;

    [SerializeField] private State currentState;

    [Space(10f)]
    [Header("LayerMask")]
    [SerializeField] private LayerMask whatIsTarget;

    [SerializeField] private Transform targetDistanceCheck;
    [SerializeField] private float targetCheckDistance;

    [Space(5f)]
    [SerializeField] private float attack01Distance;



    [Space(10f)]
    [Header("Status")]
    [SerializeField] private float defaultGravity;
    [SerializeField] private float defaultDrag;

    private bool doAttack = false;
    private bool shotOnetime = false;

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        healthmanager = gameObject.GetComponent<Boss_Health_Manager>();

        defaultGravity = rigid.gravityScale;
        defaultDrag = rigid.drag;
        currentState = State.Idle;
    }

    private void Update()
    {
        if (target == null) target = GameObject.Find("Player(Clone)").gameObject;

        if (healthmanager.Boss_Defeat_Check()) currentState = State.Die;
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Idle:
                Idle();
                if (CheckDistance() <= attack01Distance && !shotOnetime)
                {
                    doAttack = true;
                    shotOnetime = true;
                    anim.SetBool("isIdle", false);
                    anim.SetBool("isAttack", true);
                    currentState = State.Attack;
                }
                break;


            case State.Attack:
                Attack();
                if (!doAttack)
                {
                    anim.SetBool("isAttack", false);
                    anim.SetBool("isIdle", true);
                    currentState = State.Idle;
                }
                break;



            case State.Die:
                Die();

                break;
        }
    }

    public float CheckDistance()
    {
        return Mathf.Abs(target.transform.position.x - transform.position.x);
    }

    public void Idle() { }

    public void Attack() { }

    public void Die() { }

    public void End_Attack()
    {
        doAttack = false;
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
}
