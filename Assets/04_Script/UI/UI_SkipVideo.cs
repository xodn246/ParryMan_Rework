using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class UI_SkipVideo : MonoBehaviour
{
    private GameManager manager;
    private DataManager dataManager;
    private VideoPlayer cutScenePlayer;
    private PlayerInput playerInput;

    [SerializeField] private string nextScene;

    [Space(10f)]
    [SerializeField] private CanvasGroup skipCanvas;

    [SerializeField] private float canSkipTime;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float volumeFadeSpeed;
    private float volume;
    private bool inputSkip = false;
    private float inputAxis;

    [Space(10f)]
    [SerializeField] private float videoRuntime;

    private float skipTimer = -1f;

    private bool activeSkipUI = false;
    private bool canSkip = false;

    private void Awake()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        cutScenePlayer = GameObject.Find("CutScene_Video").GetComponent<VideoPlayer>();
        playerInput = transform.GetComponent<PlayerInput>();
    }

    private void Start()
    {
        StartCoroutine(End_Video(videoRuntime));
    }

    private void Update()
    {
        skipTimer -= Time.deltaTime;

        if (!manager.LoadScene)
        {
            if ((Input.anyKeyDown || CheckSkipInput()) && !activeSkipUI)
            {
                skipTimer = canSkipTime;
                activeSkipUI = true;
                Invoke("Skip_Delay", 0.1f);
                StartCoroutine(Canvas_FadeIn());
            }

            if (canSkip)
            {
                if (skipTimer > 0 && inputSkip)
                {
                    Scene_Load();
                }
                else if (skipTimer <= 0)
                {
                    canSkip = false;
                    StartCoroutine(Canvas_FadeOut());
                }
            }
        }
    }

    public void ActionSkip(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            inputSkip = true;
            StartCoroutine(End_PressSkip());
        }
    }

    public void ActionAxis(InputAction.CallbackContext context)
    {
        inputAxis = context.ReadValue<float>();
    }

    private bool CheckSkipInput()
    {
        if (inputAxis != 0) return true;
        else return false;
    }
    private IEnumerator End_PressSkip()
    {
        yield return new WaitForSeconds(0.01f);
        inputSkip = false;
    }

    private IEnumerator Canvas_FadeIn()
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * fadeSpeed;
            skipCanvas.alpha = Mathf.Lerp(0, 1, timer);
        }

        yield return new WaitForSeconds(0.5f);
        canSkip = true;
    }

    private IEnumerator Canvas_FadeOut()
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * fadeSpeed;
            skipCanvas.alpha = Mathf.Lerp(1, 0, timer);
        }
        activeSkipUI = false;
    }

    private IEnumerator Volume_FadeOut()
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * volumeFadeSpeed;
            volume = Mathf.Lerp(1, 0, timer);
            cutScenePlayer.SetDirectAudioVolume(0, volume);
        }
    }

    private void Skip_Delay()
    {
        canSkip = true;
    }

    private IEnumerator End_Video(float videoRuntime)
    {
        yield return new WaitForSeconds(videoRuntime);
        Scene_Load();
    }

    private void Scene_Load()
    {
        Destroy(gameObject);
        StartCoroutine(Volume_FadeOut());
        SceneLoader.Instance.LoadScene(nextScene);
        manager.noOptionScene = false;
        dataManager.nowData.savePos = 0;
        dataManager.SaveData();
    }


    public void OnControlsChanged()
    {
        GameManager.instance.Set_CurrentControlScheme(playerInput.currentControlScheme);
        //Debug.Log("ControlChanged : " + playerinput.currentControlScheme);
    }
}