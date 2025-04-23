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
    [SerializeField] private float waveDelay;   // 웨이브 딜레이

    [SerializeField] private float spawnDelay;  // 소환 딜레이

    public bool alreadySpawn = false; //플레이어 사망시 리스폰후 초기화

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

    private IEnumerator EnemyWaveSpawn(float waveDelay) // 적 소환 웨이브 코루틴
    {
        for (int i = 0; i < spawnWaveData.Count; i++)
        {
            if (!manager.PlayerDie) // 플레이어가 살아있을때만 소환 지속
            {
                StartCoroutine(EnemySpawn(i, spawnDelay));
                yield return new WaitForSeconds(waveDelay);
            }
            else       // 플레이어가 죽었다면 소환하지않음
            {
                break;
            }
        }
    }

    private IEnumerator EnemySpawn(int currentWave, float spawnDelay)   // 적 소환 코루틴
    {
        for (int i = 0; i < spawnWaveData[currentWave].enemy.Count; i++)
        {
            if (!manager.PlayerDie) //플레이어가 살아있을때만 소환 지속
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
            else   // 플레이어가 죽었다면 소환하지않음
            {
                break;
            }
        }
    }

    // 스포너 초기화
    public void Reset_Spawner()
    {
        StopAllCoroutines();
        alreadySpawn = false;
    }
}