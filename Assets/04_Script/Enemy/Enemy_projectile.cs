using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class Enemy_projectile : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Enemy_Projectile_Spin projectileSpin;
    private Transform target;
    private Vector2 moveDir;

    private GameObject beforeSprite;
    private GameObject afterSprite;

    [SerializeField] private float moveSpeed;
    [SerializeField] private bool movetoPlayer = true;
    [SerializeField] private bool isTurret = false;

    [Space(10f)]
    [SerializeField] private bool haveTrail = false;

    //[SerializeField] private TrailRenderer trail;

    // Start is called before the first frame update

    private void Awake()
    {
        rigid = transform.GetComponent<Rigidbody2D>();
        target = GameObject.Find("Player(Clone)").transform.Find("TargetCenter").GetComponent<Transform>();
        projectileSpin = GameObject.FindObjectOfType<Enemy_Projectile_Spin>();
    }
    private void Start()
    {
        if (movetoPlayer)
        {
            moveDir = (target.position - transform.position).normalized;
            Set_Projectile_Rotation();
        }

        if (isTurret)
        {
            beforeSprite = transform.Find("beforeParrysprite").gameObject;
            afterSprite = transform.Find("afterParrysprite").gameObject;
        }


    }

    private void FixedUpdate()
    {
        if (movetoPlayer)
        {
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        }

        else
        {
            if (transform.localScale.x > 0) transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            else transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
    }

    public void Set_Projectile_Move_Blue(float setMoveSpeed)
    {
        moveDir = -moveDir;
        moveSpeed += setMoveSpeed;
        rigid.transform.localScale = new(-rigid.transform.localScale.x, rigid.transform.localScale.y, rigid.transform.localScale.z);
    }

    public void Set_Projectile_PlayerTag()
    {
        if (transform.CompareTag("EnemyProjectile")) transform.gameObject.tag = "PlayerProjectile";
        else if (transform.CompareTag("Enemy_ProjectileDestroyDelaied")) transform.gameObject.tag = "Player_ProjectileDestroyDelaied";
    }

    public void Set_Projectile_EnemyTag()
    {
        transform.gameObject.tag = "EnemyProjectile";
    }

    public void Set_Projectile_Move_Yellow(float playerScale)
    {
        Vector2 randDir;

        if (isTurret)
        {
            beforeSprite.SetActive(false);
            afterSprite.SetActive(true);
        }

        if (playerScale > 0)
        {
            movetoPlayer = false;
            isTurret = false;
            rigid.gravityScale = 5f;
            moveSpeed = 0;
            randDir = new(Random.Range(20f, 30f), Random.Range(20f, 25f));
            rigid.AddForce(randDir, ForceMode2D.Impulse);
            projectileSpin.Set_SpinSpeed(2000f);
        }
        else
        {
            movetoPlayer = false;
            isTurret = false;
            rigid.gravityScale = 5f;
            moveSpeed = 0;
            randDir = new(Random.Range(-20f, -30f), Random.Range(20f, 25f));
            rigid.AddForce(randDir, ForceMode2D.Impulse);
            projectileSpin.Set_SpinSpeed(-2000f);
        }
    }

    public void Set_BulletSpeed(float speed)
    {
        moveSpeed = speed;
    }

    private void Set_Projectile_Rotation()
    {
        Vector2 direction = target.transform.position - gameObject.transform.position;
        float angle;
        if (direction.normalized.x <= 0)
        {
            angle = Vector2.Angle(Vector2.up, direction);
            transform.localScale = new(1, 1, 1);
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
        else
        {
            angle = Vector2.Angle(Vector2.down, direction);
            transform.localScale = new(-1, 1, 1);
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    public bool Check_Trail()
    {
        return haveTrail;
    }

    // private void OnDestroy()
    // {
    //     trail.transform.parent = null;
    //     trail.autodestruct = true;
    // }
}