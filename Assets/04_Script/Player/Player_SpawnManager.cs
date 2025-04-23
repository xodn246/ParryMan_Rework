using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_SpawnManager : MonoBehaviour
{
    private GameManager manager;
    private DataManager dataManager;
    private Player_Health_Manager playerHealthManager;
    private Boss_SpawnManager bossSpawnManager;
    private PlayerInput playerInput;

    private Input_Player inputPlayerAction;

    public bool inputRespawn = false;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private SerializableDictionary<int, Transform> respawnPos;
    [SerializeField] private List<CinemachineVirtualCamera> cinemachine;

    [Space(10f)]
    [SerializeField] private List<EnemySpawner> enemySpawner; // 스포너 초기화용
    [SerializeField] private List<EliteSpawner> eliteSpawner; // 스포너 초기화용

    [SerializeField] private List<GameObject> enemy;

    [Space(10f)]
    [SerializeField] private CanvasGroup fadeCanvas;

    [SerializeField] private float spawnDelay;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private bool bossStage;
    [SerializeField] private bool doRespawn = false;

    private void Awake()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        if (bossStage) bossSpawnManager = GameObject.Find("BossSpawnManager").GetComponent<Boss_SpawnManager>();
    }

    private void Start()
    {
        SpawnPlayer();
    }

    private void Update()
    {
        if (!GameManager.instance.LoadScene && !GameManager.instance.Active_Dialogue)
        {
            if (!doRespawn && inputRespawn)
            {
                doRespawn = true;
                Respawn_Function(); // 캐끼탈
            }
        }
    }

    // 외부에서 리스폰 참조시 사용
    public void Respawn_Function()
    {
        StartCoroutine(RespawnPlayer(spawnDelay));
    }

    private void SpawnPlayer()
    {
        manager.PlayerDie = false;
        if (playerHealthManager != null) Destroy(playerHealthManager.transform.gameObject);

        GameObject player = Instantiate(playerPrefab, respawnPos[dataManager.nowData.savePos].position, Quaternion.identity);
        playerHealthManager = player.transform.GetComponent<Player_Health_Manager>();
        if (playerInput == null) playerInput = player.transform.GetComponent<PlayerInput>();

        for (int i = 0; i < cinemachine.Count; i++)
        {
            //보스맵 아닐때만 작동
            cinemachine[i].Follow = player.transform;
            if (i == dataManager.nowData.savePos)
            {
                cinemachine[i].Priority = 10;
                player.GetComponent<Player_Health_Manager>().Set_CurrentCam(cinemachine[i]);
            }
            else
            {
                cinemachine[i].Priority = 0;
            }
        }
    }

    private IEnumerator RespawnPlayer(float respawnDelay)
    {
        float timer = 0f;

        enemy.AddRange(GameObject.FindGameObjectsWithTag("Enemy")); // 잔존 적 제거
        enemy.AddRange(GameObject.FindGameObjectsWithTag("Enemy_Dron")); // 잔존 드론 제거
        enemy.AddRange(GameObject.FindGameObjectsWithTag("Boss"));  // 보스 제거
        enemy.AddRange(GameObject.FindGameObjectsWithTag("EnemyProjectile")); //잔존 투사체 제거
        enemy.AddRange(GameObject.FindGameObjectsWithTag("Enemy_PurpleProjectile")); // 적 보라 투사체 제거 해야하는데 보라색 오브젝트들이 태그를 공유하고있음 추가적인 태그 작업으로 분리 필요함
        enemy.AddRange(GameObject.FindGameObjectsWithTag("Enemy_ProjectileDestroyDelaied")); // 적 검기 투사체 제거
        for (int i = 0; i < enemy.Count; i++) Destroy(enemy[i]);

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * fadeSpeed;
            fadeCanvas.alpha = Mathf.Lerp(0, 1, timer); // fade out
        }

        yield return new WaitForSeconds(respawnDelay);
        SpawnPlayer();
        if (bossStage) bossSpawnManager.SpawnBoss();

        // 일반 적 스포너 초기화
        for (int i = 0; i < enemySpawner.Count; i++)
        {
            enemySpawner[i].Reset_Spawner();
        }

        // 엘리트 스포너 초기화
        for (int i = 0; i < eliteSpawner.Count; i++)
        {
            eliteSpawner[i].Reset_Spawner();
        }

        yield return new WaitForSeconds(respawnDelay);

        timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * fadeSpeed;
            fadeCanvas.alpha = Mathf.Lerp(1, 0, timer); // fade in
        }

        doRespawn = false;
    }
}