using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Boss01_StartCutScene : MonoBehaviour
{
    private GameManager manager;
    private DataManager dataManager;
    private Animator anim;
    [SerializeField] private EventSystem eventSystem;

    private Boss_Health_Manager bossHealth;
    private Scene scene;

    [SerializeField] private DialogueSystemTrigger tirgger;
    [SerializeField] private DialogueSystemTrigger afterTrigger;

    [SerializeField] private GameObject dialogueTrigger;

    [SerializeField] private CinemachineVirtualCamera spawnCam;
    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera eventCam;

    [Space(10f)]
    [SerializeField] private float startDelay;

    [SerializeField] private float clearDelay;
    [SerializeField] private float clearDialogueDelay;

    [Space(10f)]
    [SerializeField] private CanvasGroup cutSceneCanvas;

    [SerializeField] private CanvasGroup clearCanvas;

    [SerializeField] private CanvasGroup demoCanvas; //데모버전에서만 사용할꺼임
    [SerializeField] private GameObject demoButton;
    [SerializeField] private GameObject demoObject;
    [SerializeField] private GameObject cutSceneObject;

    [Space(10f)]
    [SerializeField] private int endDialogueSpawnPos;
    [SerializeField] private GameObject BossBattleBoundary01;
    [SerializeField] private GameObject BossBattleBoundary02;

    [Space(10f)]
    [SerializeField] private bool clearDialogue_direct;

    private bool printClearCutscene = false;

    private void Awake()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        anim = transform.GetComponent<Animator>();
        scene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        if (bossHealth == null)
        {
            if (scene.name == "Bamboo_Boss") bossHealth = GameObject.Find("Boss01(Clone)").transform.GetComponent<Boss_Health_Manager>();
            else if (scene.name == "Sakura_Boss") bossHealth = GameObject.Find("Boss02(Clone)").transform.GetComponent<Boss_Health_Manager>();
            else if (scene.name == "Beach_Boss") bossHealth = GameObject.Find("Boss03(Clone)").transform.GetComponent<Boss_Health_Manager>();
        }

        if (!printClearCutscene && bossHealth.Boss_Defeat_Check())
        {
            printClearCutscene = true;

            if (clearDialogue_direct) Clear_Boss_Dialogue();
            else Clear_Boss01();
        }

        if (manager.dialogue_boss01)
        {
            BossBattleBoundary01.SetActive(true);
            BossBattleBoundary02.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (scene.name)
        {
            case "Bamboo_Boss":
                if (!manager.dialogue_boss01)
                {
                    if (other.CompareTag("Player"))
                    {
                        Destroy(dialogueTrigger);
                        manager.Active_Dialogue = true;
                        playerCam.Follow = other.gameObject.transform;

                        spawnCam.Priority = 0;
                        playerCam.Priority = 0;
                        eventCam.Priority = 10;
                        StartCoroutine(StartDialogue());
                    }
                }
                break;

            case "Sakura_Boss":
                if (!manager.dialogue_boss02)
                {
                    if (other.CompareTag("Player"))
                    {
                        Destroy(dialogueTrigger);
                        manager.Active_Dialogue = true;
                        playerCam.Follow = other.gameObject.transform;

                        spawnCam.Priority = 0;
                        playerCam.Priority = 0;
                        eventCam.Priority = 10;
                        StartCoroutine(StartDialogue());
                    }
                }
                break;
            case "Beach_Boss":
                if (!manager.dialogue_boss03)
                {
                    if (other.CompareTag("Player"))
                    {
                        Destroy(dialogueTrigger);
                        manager.Active_Dialogue = true;
                        playerCam.Follow = other.gameObject.transform;

                        spawnCam.Priority = 0;
                        playerCam.Priority = 0;
                        eventCam.Priority = 10;
                        StartCoroutine(StartDialogue());
                    }
                }
                break;
        }

    }

    public void Clear_Boss01()
    {
        StartCoroutine(Clear_Boss01_Coroutine());
        manager.Active_Dialogue = true;
    }

    public IEnumerator Clear_Boss01_Coroutine()
    {
        StartCoroutine(Clear_Canvas_FadeIn());
        yield return new WaitForSeconds(clearDelay);
        StartCoroutine(Clear_Canvas_FadeOut());
        yield return new WaitForSeconds(clearDialogueDelay);
        afterTrigger.OnUse();
    }

    public void Clear_Boss_Dialogue()
    {
        StartCoroutine(Clear_Boss_Dialogue_Coroutine());
    }

    public IEnumerator Clear_Boss_Dialogue_Coroutine()
    {
        manager.Active_Dialogue = true;
        yield return new WaitForSeconds(clearDialogueDelay);
        afterTrigger.OnUse();
    }

    //---------------------------------------------------------- < 데모 > ---------------------------------------------------

    // public void Go_Title_Button()
    // {
    //     manager.noOptionScene = true;
    //     Destroy(demoCanvas.gameObject);
    //     Destroy(GameObject.Find("SoundManager").gameObject);
    //     SceneLoader.Instance.LoadScene("Title");
    //     dataManager.InitializeData();
    //     dataManager.SaveData();
    // }

    //-----------------------------------------------------------------------------------------------------------------------

    private IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(startDelay);
        tirgger.OnUse();
    }

    public void Start_UI_CutScene()
    {
        StartCoroutine(Start_CutScene());
    }

    private IEnumerator Start_CutScene()
    {
        manager.Active_Dialogue = true;
        yield return new WaitForSeconds(1f);
        StartCoroutine(Canvas_FadeIn());
        cutSceneObject.SetActive(true);
        anim.SetTrigger("startCutScene");
    }

    private IEnumerator Canvas_FadeIn()
    {
        float timer = 0f;

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            cutSceneCanvas.alpha = Mathf.Lerp(0, 1, timer);
        }
    }

    private IEnumerator Canvas_FadeOut()
    {
        float timer = 0f;

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 2f;
            cutSceneCanvas.alpha = Mathf.Lerp(1, 0, timer);
        }
    }

    private IEnumerator Clear_Canvas_FadeIn()
    {
        float timer = 0f;

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 2f;
            clearCanvas.alpha = Mathf.Lerp(0, 1, timer);
            //Debug.Log("페이드인 하는중이야!");
        }
    }

    private IEnumerator Clear_Canvas_FadeOut()
    {
        float timer = 0f;

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 2f;
            clearCanvas.alpha = Mathf.Lerp(1, 0, timer);
            //Debug.Log("페이드아웃 하는중이야!");
        }
    }

    //----------------------- < 데모 > ---------------------------------
    // public void DisplayDemoCanvs()
    // {
    //     StartCoroutine(Demo_Canvas_FadeIn());
    // }

    // private IEnumerator Demo_Canvas_FadeIn()
    // {
    //     float timer = 0f;

    //     while (timer <= 1f)
    //     {
    //         yield return null;
    //         timer += Time.unscaledDeltaTime * 3f;
    //         demoCanvas.alpha = Mathf.Lerp(0, 1, timer);
    //     }

    //     yield return new WaitForSeconds(1f);
    //     demoObject.SetActive(true);

    //     eventSystem.SetSelectedGameObject(demoButton);
    // }

    //-------------------------------------------------------------------



    //=================================== <스팀 업적> ===================================
    // public void Achieve_Shotgun()
    // {
    //     SteamAchievement.instance.Achieve("Clear_Shotgun");
    // }

    // public void Achieve_Edge()
    // {
    //     SteamAchievement.instance.Achieve("Clear_Edge");
    // }

    // public void Achieve_Smasher()
    // {
    //     SteamAchievement.instance.Achieve("Clear_Smasher");
    // }
    // ==================================================================================


    public void End_CutScene()      // 대화 종료시 대화 완료 여부 데이터 저장
    {
        StartCoroutine(Canvas_FadeOut());

        switch (scene.name)
        {
            case "Bamboo_Boss":
                dataManager.nowData.boss01_Dialogue = true;
                break;

            case "Sakura_Boss":
                dataManager.nowData.boss02_Dialogue = true;
                break;

            case "Beach_Boss":
                dataManager.nowData.boss03_Dialogue = true;
                break;
        }

        dataManager.nowData.savePos = endDialogueSpawnPos;
        dataManager.SaveData();

        eventCam.Priority = 0;
        playerCam.Priority = 10;
        manager.Active_Dialogue = false;
    }

    private void OnEnable()
    {
        // Make the functions available to Lua: (Replace these lines with your own.)
        Lua.RegisterFunction("Start_UI_CutScene", this, SymbolExtensions.GetMethodInfo(() => Start_UI_CutScene()));
        // Lua.RegisterFunction("Achieve_Shotgun", this, SymbolExtensions.GetMethodInfo(() => Achieve_Shotgun()));
        // Lua.RegisterFunction("Achieve_Edge", this, SymbolExtensions.GetMethodInfo(() => Achieve_Edge()));
        // Lua.RegisterFunction("Achieve_Smasher", this, SymbolExtensions.GetMethodInfo(() => Achieve_Smasher()));
    }

    private void OnDisable()
    {
        // Remove the functions from Lua: (Replace these lines with your own.)
        Lua.UnregisterFunction("Start_UI_CutScene");
        // Lua.UnregisterFunction("Achieve_Shotgun");
        // Lua.UnregisterFunction("Achieve_Edge");
        // Lua.UnregisterFunction("Achieve_Smasher");
    }
}