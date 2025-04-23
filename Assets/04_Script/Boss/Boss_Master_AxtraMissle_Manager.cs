using System.Collections;
using UnityEngine;

public class Boss_Master_AxtraMissle_Manager : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;
    private GameObject target;
    private Enemy_AttackGroundCheck groundCheck;
    private Transform parentTransform;

    [SerializeField] private GameObject misslePrefab;
    [SerializeField] private Transform misslePosition;

    [SerializeField] private float launchTurm;
    [SerializeField] private float attackTurm;

    [SerializeField] private Vector2 launchDir;

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        groundCheck = gameObject.GetComponent<Enemy_AttackGroundCheck>();
        //target = GameObject.Find("Player(Clone)").gameObject;

    }

    private void Update()
    {
        if (target == null)
        {
            Debug.Log("Å¸°Ù ¾øÀ½");
            target = GameObject.Find("Player(Clone)").gameObject;

        }

    }
    public void Luanch_Missale()
    {
        rigid.AddForce(launchDir, ForceMode2D.Impulse);

        StartCoroutine(Missale_Logic());
    }

    public IEnumerator Missale_Logic()
    {

        yield return new WaitForSeconds(launchTurm);
        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 0;

        yield return new WaitForSeconds(attackTurm);
        transform.position = target.transform.position;
        transform.position = new(target.transform.position.x, groundCheck.CheckAttackGround().y);
        anim.SetBool("isAlert", true);
        // GameObject alert = Instantiate(missaleAlertPrefab, target.transform.position, Quaternion.identity);
        // alert.transform.position = new(alert.transform.position.x, alert.GetComponent<Enemy_AttackGroundCheck>().CheckAttackGround().y);
    }

    private void Instance_Missle()
    {
        GameObject missile = Instantiate(misslePrefab, misslePosition.position, Quaternion.identity);
        missile.GetComponent<Boss_Master_AxtraMissile_Hitbox>().Set_Parent_Transform(parentTransform);
        Destroy(gameObject);
    }

    public void Set_Parent_Transform(Transform transform)
    {
        parentTransform = transform;
    }
}
