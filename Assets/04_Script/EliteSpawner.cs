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
    [SerializeField] private CinemachineVirtualCamera eliteCam; // ����Ʈ�� �ȿ��� ���ε��� ī�޶�
    private CinemachineVirtualCamera playerCam; // �÷��̾� ���Խ� ī�޶� ����

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
            Debug.Log("�÷��̾� ����");
            if (!alreadySpawn)
            {
                alreadySpawn = true;
                player = collider.transform.gameObject;

                // ����Ʈ ���� ī�޶� ���ε�
                playerCam = collider.GetComponent<Player_Health_Manager>().Get_CurrentCam();
                playerCam.Priority = 0;

                collider.GetComponent<Player_Health_Manager>().Set_CurrentCam(eliteCam);
                eliteCam.Follow = collider.transform;
                eliteCam.Priority = 10;

                // ����Ʈ ���� �� ����
                boundaryWall.SetActive(true);

                // ����Ʈ ����
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
            enemy.transform.GetComponent<Elite_Spawn_Setup>().Set_SpawnParent(transform.GetComponent<EliteSpawner>()); // ��ȯ�� ����Ʈ�� ī��Ʈ�� ���� �Ķ���� �Ҵ�
            eliteCount++; // ��ȯ�� ����ŭ ����Ʈ ī��Ʈ++
            Debug.Log("���� ����Ʈ �� : " + eliteCount);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // ����Ʈ ������ �ʱ�ȭ
    public void Reset_Spawner()
    {
        alreadySpawn = false;
        endCombat = false;
        boundaryWall.SetActive(false);
        eliteCount = 0;
    }

    // ����Ʈ ����� ī��Ʈ ���̸鼭 ��� ����Ʈ�� ����ߴ��� üũ > ��� ����Ʈ óġ�� �������� ���� ���� �ݶ��̴� �� ī�޶� ó��
    public void Decrease_Elite_Count()
    {
        eliteCount--;
        Debug.Log("���� ����Ʈ �� : " + eliteCount);
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
