
using UnityEngine;

public class Boss_Projectile_Movement : MonoBehaviour
{
    private enum moveType
    {
        addForce,
        movetoward
    }

    private Rigidbody2D rigid;
    private Transform target;
    [SerializeField] private moveType type;
    [SerializeField] private Vector2 moveDir;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float setDrag;

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        rigid.drag = setDrag;

        if (type == moveType.movetoward)
        {
            target = GameObject.Find("Player(Clone)").transform.Find("TargetCenter").GetComponent<Transform>();
            moveDir = (target.position - transform.position).normalized;
        }
    }

    private void Start()
    {
        if (type == moveType.addForce)
        {
            Vector2 result;
            if (rigid.transform.localScale.x > 0)
            {
                result = new(-moveDir.x, moveDir.y);
                Debug.Log("여기 실행");
            }
            else
            {
                result = moveDir;
                Debug.Log("저기 실행");
            }

            rigid.AddForce(result, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    public void Set_Projectile_Move(float setMoveSpeed)
    {
        moveDir = -moveDir;
        moveSpeed = setMoveSpeed;
    }
}
