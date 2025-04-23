using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elite_Spawn_Setup : MonoBehaviour
{
    private EliteSpawner enemySpawner;
    private Boss_Health_Manager healthManager;

    private bool bossCountCheck = false;

    private void Awake()
    {
        healthManager = transform.GetComponent<Boss_Health_Manager>();
    }

    private void Update()
    {
        if (healthManager.Boss_Defeat_Check() && !bossCountCheck)
        {
            bossCountCheck = true;
            Set_SpawnCount();
        }
    }
    private void Set_SpawnCount()
    {
        enemySpawner.Decrease_Elite_Count();
    }

    // 부모 스포너 할당을 위한 함수
    public void Set_SpawnParent(EliteSpawner spawner)
    {
        enemySpawner = spawner;
    }

}
