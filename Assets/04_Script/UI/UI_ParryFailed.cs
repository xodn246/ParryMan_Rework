using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ParryFailed : MonoBehaviour
{
    private GameManager manager;
    private Player_SpawnManager playerSpawnManager;
    private Object_SoundManager soundManager;
    private Animator anim;
    [SerializeField] private CanvasGroup canvasGroup;

    private bool canRespawn = false;

    private void Awake()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerSpawnManager = GameObject.FindObjectOfType<Player_SpawnManager>().GetComponent<Player_SpawnManager>();
        soundManager = transform.GetComponentInParent<Object_SoundManager>();
        anim = transform.GetComponent<Animator>();
    }

    private void Update()
    {
        if (manager.PlayerDie && canRespawn)
        {
            if (Input.anyKey)
            {
                canRespawn = false;
                playerSpawnManager.Respawn_Function();
            }
        }
    }

    public void Normal_Failed()
    {
        StartCoroutine(Fade_Panel());
        anim.SetTrigger("Normal");
    }

    public void Boss01_Failed()
    {
        StartCoroutine(Fade_Panel());
        anim.SetTrigger("Boss01");
    }

    public void Boss02_Failed()
    {
        StartCoroutine(Fade_Panel());
        anim.SetTrigger("Boss02");
    }

    public void Boss03_Failed()
    {
        StartCoroutine(Fade_Panel());
        anim.SetTrigger("Boss03");
    }

    public void Boss04_Failed()
    {
        StartCoroutine(Fade_Panel());
        anim.SetTrigger("Boss04");
    }

    private IEnumerator Fade_Panel()
    {
        float timer = 0;

        while (timer <= 1)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer);
        }
    }

    public void Set_CanRespawn()
    {
        canRespawn = true;
    }

    public void Get_AudioClip_Animation(string SoundKey)
    {
        SoundManager.instance.SFXPlayer_UI(/*"Player_FailedCut",*/ soundManager.Get_AudioClip(SoundKey), gameObject.transform);
    }
}