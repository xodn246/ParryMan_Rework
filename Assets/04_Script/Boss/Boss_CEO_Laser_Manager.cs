using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_CEO_Laser_Manager : MonoBehaviour
{
    private Animator anim;
    private GameObject target;
    private float startTurm;
    private float duration;
    private float moveSpeed;

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private BoxCollider2D hitbox;
    [SerializeField] private GameObject laserParticle_strim;
    [SerializeField] private GameObject laserParticle_ground;

    [Space(10f)]
    [SerializeField] private Transform groundCehckPos;
    [SerializeField] private float groundCheckDistance;

    [Space(10f)]
    [SerializeField] private Transform VFXPos;
    [SerializeField] private GameObject laserVFX;

    [Space(10f)]
    [SerializeField] private Transform shockPos;
    [SerializeField] private GameObject laserShockParticle;

    private bool laserStart = false;
    private bool laserEnd = false;
    [SerializeField] private float impactTime;

    private void Awake()
    {
        anim = transform.GetComponent<Animator>();
    }

    private void Update()
    {
        startTurm -= Time.deltaTime;
        duration -= Time.deltaTime;

        if (!laserStart && startTurm <= 0)
        {
            laserStart = true;
            anim.SetTrigger("startLaserTrigger");
            laserParticle_ground.SetActive(true);
            laserParticle_strim.SetActive(true);
            Instantiate(laserShockParticle, shockPos.position, Quaternion.identity);
            hitbox.enabled = true;

            StartCoroutine(Reset_Lens());
        }
        if (!laserEnd && duration <= 0)
        {
            laserEnd = true;
            anim.SetTrigger("endLaserTrigger");
            laserParticle_ground.SetActive(false);
            laserParticle_strim.SetActive(false);
            hitbox.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (laserStart && !laserEnd)
        {
            if (transform.position.x < target.transform.position.x) transform.Translate(moveSpeed * Time.deltaTime * Vector2.right);
            else transform.Translate(moveSpeed * Time.deltaTime * Vector2.left);
        }
    }

    private IEnumerator Reset_Lens()
    {
        float timer = 0;

        while (timer <= 1)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            target.GetComponentInParent<Player_Health_Manager>().Get_CurrentCam().m_Lens.OrthographicSize = Mathf.Lerp(16f, 18, timer);
        }
    }

    public void End_Laser()
    {
        Destroy(gameObject);
    }

    public Vector2 Get_Laser_Ground_Point()
    {
        return Physics2D.Raycast(groundCehckPos.position, Vector2.down, groundCheckDistance, whatIsGround).point;
    }

    public void Set_LaserParameter(float laserTurm, float laserDuration, GameObject laserTarget, float laserMoveSpeed)
    {
        startTurm = laserTurm;
        duration = laserDuration;
        target = laserTarget;
        moveSpeed = laserMoveSpeed;
    }

    public void Set_LaserDuration(float laserTime)
    {
        duration = laserTime;
    }

    public void Set_LaserStartTurm(float laserTurm)
    {
        startTurm = laserTurm;
    }

    public void Set_LaserTarget(GameObject target)
    {
        this.target = target;
    }

    public void Print_VFX()
    {
        Instantiate(laserVFX, VFXPos.position, Quaternion.identity);
    }
}
