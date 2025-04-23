using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[System.Serializable]
public class SpawnEliteData
{
    public GameObject enemyPrefab;
    public bool isFlip;

    public SpawnEliteData(SpawnEliteData enemyData)
    {
        enemyPrefab = enemyData.enemyPrefab;
        isFlip = enemyData.isFlip;
    }
}

[System.Serializable]
public class SpawnEliteWave
{
    public List<SpawnEliteData> enemy;
    public List<Transform> spawnPos;

    public SpawnEliteWave(SpawnEliteWave SpawnElite)
    {
        enemy = SpawnElite.enemy;
        spawnPos = SpawnElite.spawnPos;
    }
}

public class EliteSpawner : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private List<SpawnEliteWave> spawnEliteData;

    [Space(10f)]
    [SerializeField] private CinemachineVirtualCamera eliteCam; // 엘리트존 안에서 바인딩할 카메라
    private CinemachineVirtualCamera playerCam; // 플레이어 진입시 카메라 저장

    [Space(10f)]
    [SerializeField] private GameObject boundaryWall;
    [SerializeField] private float waveDelay;
    [SerializeField] private float spawnDelay;

    private bool alreadySpawn = false;
    private bool endCombat = false;
    private int eliteCount = 0;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("플레이어 접촉");
            if (!alreadySpawn)
            {
                alreadySpawn = true;
                player = collider.transform.gameObject;

                // 엘리트 구역 카메라 바인딩
                playerCam = collider.GetComponent<Player_Health_Manager>().Get_CurrentCam();
                playerCam.Priority = 0;

                collider.GetComponent<Player_Health_Manager>().Set_CurrentCam(eliteCam);
                eliteCam.Follow = collider.transform;
                eliteCam.Priority = 10;

                // 엘리트 구역 벽 생성
                boundaryWall.SetActive(true);

                // 엘리트 생성
                StartCoroutine(EliteWaveSpawn(waveDelay));
            }
        }
    }

    private IEnumerator EliteWaveSpawn(float waveDelay)
    {
        for (int i = 0; i < spawnEliteData.Count; i++)
        {
            StartCoroutine(EnemySpawn(i, spawnDelay));
            yield return new WaitForSeconds(waveDelay);
        }
    }

    private IEnumerator EnemySpawn(int currentWave, float spawnDelay)
    {
        for (int i = 0; i < spawnEliteData[currentWave].enemy.Count; i++)
        {
            GameObject enemy = Instantiate(spawnEliteData[currentWave].enemy[i].enemyPrefab, spawnEliteData[currentWave].spawnPos[i].position, Quaternion.identity);
            enemy.transform.GetComponent<Elite_Spawn_Setup>().Set_SpawnParent(transform.GetComponent<EliteSpawner>()); // 소환된 엘리트의 카운트를 위한 파라미터 할당
            eliteCount++; // 소환된 수만큼 엘리트 카운트++
            Debug.Log("현재 엘리트 수 : " + eliteCount);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // 엘리트 스포너 초기화
    public void Reset_Spawner()
    {
        alreadySpawn = false;
        endCombat = false;
        boundaryWall.SetActive(false);
        eliteCount = 0;
    }

    // 엘리트 사망시 카운트 줄이면서 모든 엘리트가 사망했는지 체크 > 모든 엘리트 처치시 다음으로 진행 위한 콜라이더 및 카메라 처리
    public void Decrease_Elite_Count()
    {
        eliteCount--;
        Debug.Log("현재 엘리트 수 : " + eliteCount);
        Check_Endcombat();
    }

    public void Check_Endcombat()
    {
        if (!endCombat && eliteCount == 0)
        {
            endCombat = true;
            boundaryWall.SetActive(false);
            player.GetComponent<Player_Health_Manager>().Set_CurrentCam(playerCam);
            playerCam.Priority = 10;
            eliteCam.Priority = 0;
        }
    }
}
