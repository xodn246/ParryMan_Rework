using System.Collections;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss04_CustScene_Manager : MonoBehaviour
{
    private GameManager manager;
    private DataManager dataManager;
    private Boss_SpawnManager bossSpawnManager;
    private GameObject player;
    private SceneChanger sceneChanger;
    [SerializeField] private Object_SoundManager soundManager;
    [SerializeField] private Animator anim;

    private Boss_Health_Manager bossHealth;
    private Scene scene;

    [Space(10f)]
    [SerializeField] private CanvasGroup clearCanvas;

    [SerializeField] private GameObject cutsceneTrigger;

    [SerializeField] private CinemachineVirtualCamera spawnCam;
    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera eventCam;

    [Space(10f)]
    [SerializeField] private GameObject shockPrefab;
    [SerializeField] private GameObject shockCameraObject;
    [SerializeField] private Transform shockStartPos;
    [SerializeField] private float shockStartDelay;
    [SerializeField] private float shockDelay;
    [SerializeField] private float shockTransformTurm;
    [SerializeField] private int shockCount;


    [Space(10f)]
    [SerializeField] private float clearCutDelay;
    [SerializeField] private float clearDelay;
    [SerializeField] private float sceneChangeDelay;

    private bool printClearCutscene = false;


    private void Awake()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        scene = SceneManager.GetActiveScene();
        bossSpawnManager = GameObject.FindObjectOfType<Boss_SpawnManager>().GetComponent<Boss_SpawnManager>();
        sceneChanger = GameObject.Find("SceneChanger").GetComponent<SceneChanger>();
    }

    private void Update()
    {
        if (player == null) player = GameObject.Find("Player(Clone)").gameObject;

        if (bossHealth == null)
        {
            if (scene.name == "Bamboo_Boss") bossHealth = GameObject.Find("Boss01(Clone)").transform.GetComponent<Boss_Health_Manager>();
            else if (scene.name == "Sakura_Boss") bossHealth = GameObject.Find("Boss02(Clone)").transform.GetComponent<Boss_Health_Manager>();
            else if (scene.name == "Beach_Boss") bossHealth = GameObject.Find("Boss03(Clone)").transform.GetComponent<Boss_Health_Manager>();
            else if (scene.name == "Master_Boss") bossHealth = GameObject.Find("Boss04(Clone)").transform.GetComponent<Boss_Health_Manager>();
        }

        if (!printClearCutscene && bossHealth.Boss_Defeat_Check())
        {
            printClearCutscene = true;
            Clear_Boss();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!dataManager.nowData.boss04_Cutscene)
            {
                manager.Active_Dialogue = true;
                anim.SetTrigger("cutsceneStart");
            }
            Destroy(cutsceneTrigger);
        }
    }

    public void Clear_Boss()
    {
        StartCoroutine(Clear_Boss_Coroutine());
    }

    public IEnumerator Clear_Boss_Coroutine()
    {
        manager.Active_Dialogue = true;
        yield return new WaitForSeconds(clearCutDelay);
        SoundManager.instance.SFXPlayer_UI(soundManager.Get_AudioClip("Clear"), gameObject.transform);
        StartCoroutine(Clear_Canvas_FadeIn());
        yield return new WaitForSeconds(clearDelay);
        StartCoroutine(Clear_Canvas_FadeOut());
        yield return new WaitForSeconds(sceneChangeDelay);

        sceneChanger.ChangeSceneWith_Function();
    }

    private IEnumerator Clear_Canvas_FadeIn()
    {
        float timer = 0f;

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 2f;
            clearCanvas.alpha = Mathf.Lerp(0, 1, timer);
            Debug.Log("페이드인 하는중이야!");
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
            Debug.Log("페이드아웃 하는중이야!");
        }
    }

    public void Start_Boss04_Cutscene_Shock()
    {
        anim.SetTrigger("shockStart");
        StartCoroutine(SpawnShock());

        spawnCam.Priority = 0;
        playerCam.Priority = 0;
        eventCam.Priority = 10;
    }

    public IEnumerator SpawnShock()
    {
        yield return new WaitForSeconds(shockStartDelay);

        for (int i = 0; i < shockCount; i++)
        {
            float resultPosX = shockStartPos.position.x - (i * shockTransformTurm);
            Vector3 resultPos = new(resultPosX, shockStartPos.position.y, shockStartPos.position.z);

            shockCameraObject.transform.position = resultPos;

            if (i == shockCount - 1) bossSpawnManager.SpanwBoss_External(shockCameraObject.transform);
            else
            {
                Instantiate(shockPrefab, resultPos, Quaternion.identity);
                SoundManager.instance.SFXPlayer_UI(soundManager.Get_AudioClip("Spawn01"), gameObject.transform);
                SoundManager.instance.SFXPlayer_UI(soundManager.Get_AudioClip("Spawn02"), gameObject.transform);
            }

            yield return new WaitForSeconds(shockDelay);

        }

        End_Boss04_Cutscene();
    }

    public void End_Boss04_Cutscene()
    {
        dataManager.nowData.boss04_Cutscene = true;
        dataManager.nowData.savePos = 1;
        dataManager.SaveData();

        player.GetComponent<Player_Health_Manager>().Set_CurrentCam(playerCam);
        spawnCam.Priority = 0;
        playerCam.Priority = 10;
        eventCam.Priority = 0;

        dataManager.nowData.boss04_Cutscene = true;
        dataManager.SaveData();

        manager.Active_Dialogue = false;
    }

}
