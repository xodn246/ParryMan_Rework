
using UnityEngine;

public class Enemy_Turret_LaserBeam : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private Object_SoundManager soundManager;
    [SerializeField] private GameObject soundPosition;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] private float rayMaxRange;


    private float turmTimer;   // 레이저 발사 대기시간간
    private float durationTimer;   // 레이저 발사 지속시간간
    private bool isShooting = false;

    [Space(10f)]

    [Tooltip("레이저 시작 딜레이")][SerializeField] private float startDelay;
    [Tooltip("레이저 발사 대기시간")][SerializeField] private float rayTurm; // 레이저 발사 대기시간간
    [Tooltip("레이저 지속시간간")][SerializeField] private float rayDuration;  // 레이저 발사 지속시간간

    [SerializeField] private Transform MuzzlePos;
    [SerializeField] private GameObject rayStartPrticle;

    [SerializeField] private GameObject rayParticle;

    [Space(10f)]
    [SerializeField] private bool alwaysBeam;

    private bool soundCheck = false;

    private void Awake()
    {
        anim = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startDelay >= 0)
        {
            startDelay -= Time.deltaTime;
            anim.SetBool("Active", false);
            isShooting = false;
        }

        if (startDelay <= 0)
        {
            if (!alwaysBeam)
            {
                if (!isShooting)
                {
                    turmTimer -= Time.deltaTime;

                    anim.SetBool("Active", false);
                    //currentRnage = 0f;

                    if (turmTimer <= 0)
                    {
                        isShooting = true;
                        durationTimer = rayDuration;
                    }
                }
                else
                {
                    durationTimer -= Time.deltaTime;

                    anim.SetBool("Active", true);
                    //currentRnage = rayMaxRange;

                    if (durationTimer <= 0)
                    {
                        isShooting = false;
                        turmTimer = rayTurm;
                    }
                }
            }
            else
            {
                isShooting = true;
                anim.SetBool("Active", true);
            }

            float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(-Mathf.Cos(angle), -Mathf.Sin(angle));

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, /*rayMaxRange , currentRnage*/ rayMaxRange, layersToHit);

            soundPosition.transform.position = new(hit.point.x, hit.point.y);

            if (hit.collider == null)
            {
                transform.localScale = new Vector3(/*rayMaxRange, currentRnage*/ rayMaxRange, transform.localScale.y, 1);
                return;
            }
            else
            {
                if (isShooting)
                {
                    if (!soundCheck)
                    {
                        soundCheck = true;
                        if (alwaysBeam)
                        {
                            SoundManager.instance.SFXPlayer_Loop(soundManager.Get_AudioClip("Beam"), gameObject.transform);
                            SoundManager.instance.SFXPlayer_Loop(soundManager.Get_AudioClip("Beam"), soundPosition.transform);
                        }
                        else
                        {
                            SoundManager.instance.SFXPlayer_DestroySetTime(soundManager.Get_AudioClip("Beam"), gameObject.transform, rayDuration);
                            SoundManager.instance.SFXPlayer_DestroySetTime(soundManager.Get_AudioClip("Beam"), soundPosition.transform, rayDuration);
                        }
                    }

                    transform.localScale = new Vector3(hit.distance, transform.localScale.y, 1);

                    rayStartPrticle.transform.position = MuzzlePos.position;
                    rayStartPrticle.SetActive(true);

                    rayParticle.transform.position = new(hit.point.x, hit.point.y);
                    rayParticle.SetActive(true);
                }
                else
                {
                    soundCheck = false;
                    //transform.localScale = new Vector3(0, transform.localScale.y, 1);
                    transform.localScale = new Vector3(hit.distance, transform.localScale.y, 1);

                    rayStartPrticle.transform.position = new(1800, 1800);

                    rayParticle.transform.position = new(hit.point.x, hit.point.y);
                    rayParticle.transform.position = new(1800, 1800);
                }
            }

        }
    }
}
