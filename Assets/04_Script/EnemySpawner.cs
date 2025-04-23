using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnEnemyData
{
    public GameObject enemyPrefab;
    public bool isFlip;
    public bool isFixed;

    public SpawnEnemyData(SpawnEnemyData enemyData)
    {
        enemyPrefab = enemyData.enemyPrefab;
        isFlip = enemyData.isFlip;
        isFixed = enemyData.isFixed;
    }
}

[System.Serializable]
public class SpawnWave
{
    public List<SpawnEnemyData> enemy;
    public List<Transform> spawnPos;

    public SpawnWave(SpawnWave spawnWave)
    {
        enemy = spawnWave.enemy;
        spawnPos = spawnWave.spawnPos;
    }
}

public class EnemySpawner : MonoBehaviour
{
    private GameManager manager;
    [SerializeField] private List<SpawnWave> spawnWaveData;

    [Space(10f)]
    [SerializeField] private float waveDelay;   // ���̺� ������

    [SerializeField] private float spawnDelay;  // ��ȯ ������

    public bool alreadySpawn = false; //�÷��̾� ����� �������� �ʱ�ȭ

    private IEnumerator enemySpawn;
    private IEnumerator waveSpawn;

    private void Awake()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (!alreadySpawn)
            {
                alreadySpawn = true;

                StartCoroutine(EnemyWaveSpawn(waveDelay));
            }
        }
    }

    private IEnumerator EnemyWaveSpawn(float waveDelay) // �� ��ȯ ���̺� �ڷ�ƾ
    {
        for (int i = 0; i < spawnWaveData.Count; i++)
        {
            if (!manager.PlayerDie) // �÷��̾ ����������� ��ȯ ����
            {
                StartCoroutine(EnemySpawn(i, spawnDelay));
                yield return new WaitForSeconds(waveDelay);
            }
            else       // �÷��̾ �׾��ٸ� ��ȯ��������
            {
                break;
            }
        }
    }

    private IEnumerator EnemySpawn(int currentWave, float spawnDelay)   // �� ��ȯ �ڷ�ƾ
    {
        for (int i = 0; i < spawnWaveData[currentWave].enemy.Count; i++)
        {
            if (!manager.PlayerDie) //�÷��̾ ����������� ��ȯ ����
            {
                GameObject enemy = Instantiate(spawnWaveData[currentWave].enemy[i].enemyPrefab, spawnWaveData[currentWave].spawnPos[i].position, Quaternion.identity);

                if (spawnWaveData[currentWave].enemy[i].isFlip)
                {
                    enemy.GetComponent<Enemy_Manager>().Flip();

                    Vector3 flipEnemy = enemy.GetComponent<Transform>().localScale;
                    flipEnemy = new(flipEnemy.x * -1, flipEnemy.y, 0);
                    enemy.GetComponent<Transform>().localScale = flipEnemy;
                }

                if (spawnWaveData[currentWave].enemy[i].isFixed)
                {
                    enemy.GetComponent<Enemy_Manager>().Set_AttackType();
                }

                yield return new WaitForSeconds(spawnDelay);
            }
            else   // �÷��̾ �׾��ٸ� ��ȯ��������
            {
                break;
            }
        }
    }

    // ������ �ʱ�ȭ
    public void Reset_Spawner()
    {
        StopAllCoroutines();
        alreadySpawn = false;
    }
}