using UnityEngine;

public class Enemy_Dron_Spawned : MonoBehaviour
{
    private Boss_CEO_Manager ceoManager;
    private Enemy_DronManager dronManager;

    private bool countCheck = false;

    private void Start()
    {
        dronManager = transform.GetComponent<Enemy_DronManager>();
    }

    private void Update()
    {
        if (!countCheck && dronManager.isDead)
        {
            countCheck = true;
            ceoManager.Count_Dron_Minuse();
        }
    }


    public void Set_CEO_Manager(Boss_CEO_Manager manager)
    {
        ceoManager = manager;
    }
}
