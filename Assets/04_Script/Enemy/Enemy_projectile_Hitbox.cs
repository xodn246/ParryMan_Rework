using System.Collections;
using UnityEngine;

public class Enemy_projectile_Hitbox : MonoBehaviour
{
    [SerializeField] private BoxCollider2D hitbox;
    [SerializeField] private SpriteRenderer projectileSprite;
    private Transform parentTransform;

    [Space(10f)]
    [SerializeField] private bool passGround;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;
    private bool playerProjectile;
    private bool hitSomething;

    [SerializeField] private bool canReflect;

    [Space(10f)]
    [SerializeField] private bool destroyDelaied;
    [SerializeField] private Animator anim;

    [Space(10f)]
    [SerializeField] private bool haveSpark;
    [SerializeField] private GameObject bulletSpark;


    private void Update()
    {
        if (hitSomething) Destroy(gameObject);
        if (!passGround && CheckWall())
        {
            Set_Hit();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!playerProjectile) // 플레이어 히트 처리
        {
            if (collider.CompareTag("PlayerHurt"))
            {
                if (canReflect) collider.transform.GetComponentInParent<Player_Health_Manager>().Player_Parry_EnemyProjectile_Skyblue(transform);
                else collider.transform.GetComponentInParent<Player_Health_Manager>().Player_Parry_EnemyProjectile_Yellow(transform);

                collider.transform.GetComponentInParent<Player_Health_Manager>().Player_TakeDamage(parentTransform, 10, tag);
            }
        }
        else if (playerProjectile && collider.CompareTag("EnemyHurt")) // 일반적 히트 처리
        {
            collider.transform.GetComponentInParent<Enemy_HealthManager>().Enemy_TakeDamage(1); // 데미지 처리 해줘야함 왜 안죽지? 하면 여기임
            collider.transform.GetComponentInParent<Enemy_HealthManager>().Enemy_ParryProjectile(transform);
        }
        else if (playerProjectile && collider.CompareTag("BossHurt"))
        {
            Debug.Log("보스 충돌");
            collider.transform.GetComponentInParent<Boss_Health_Manager>().Boss_Parry(transform);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!passGround && collider.CompareTag("Ground"))  // 타일 히트 처리
        {
            Set_Hit();
        }
    }

    public bool CheckWall()
    {
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(-Mathf.Cos(angle), -Mathf.Sin(angle));
        return Physics2D.Raycast(wallCheck.position, dir, wallCheckDistance, whatIsGround);
    }


    public void Set_PlayerProjectile()
    {
        playerProjectile = true;
    }

    public void Set_DisableHitbox()
    {
        transform.Find("hitbox").gameObject.SetActive(false);
    }

    public void Set_EnemyProjectile()
    {
        playerProjectile = false;
    }

    public void Set_Hit_Character()
    {
        hitSomething = true;
    }

    public void Set_Hit()
    {
        StartCoroutine(Hit_Process());
        //hitSomething = true;
    }

    private IEnumerator Hit_Process()
    {
        if (haveSpark) bulletSpark.SetActive(true);
        hitbox.enabled = false;
        projectileSprite.color = new(0, 0, 0, 0);
        gameObject.GetComponent<Enemy_projectile>().Set_BulletSpeed(0f);
        yield return new WaitForSeconds(1f);
        hitSomething = true;
    }

    public void Set_Hit_Delaied()
    {
        hitbox.enabled = false;
        anim.SetTrigger("Fadeout");
    }

    public void Set_Parent_Transform(Transform enemyTransform)
    {
        parentTransform = enemyTransform;
    }
}