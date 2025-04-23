using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Cinemachine;

public class MapObject_SavePoint : MonoBehaviour
{
    private GameManager manager;
    private DataManager dataManager;
    private Animator anim;
    private GameObject falgCollider;
    private GameObject saveWall;
    private GameObject saveParticle;


    private bool saveAlready;
    [SerializeField] private List<GameObject> enemy;

    [SerializeField] private GameObject saveActiveParticle;
    [SerializeField] private Transform particlePos;

    [Space(10f)]
    [SerializeField] private int savePointNum;

    [Space(10f)]
    [SerializeField] private CinemachineVirtualCamera priviousCam;

    [SerializeField] private CinemachineVirtualCamera savePointCam;

    [Space(10f)]
    [SerializeField] private bool tutorialObject;

    private void Awake()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        anim = transform.GetComponent<Animator>();
        falgCollider = transform.Find("saveCollider").gameObject;
        saveWall = transform.Find("saveWall").gameObject;
        saveParticle = transform.Find("SavePointParticle").gameObject;
        saveAlready = false;
    }

    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            if (player.GetComponentInParent<Player_Manager>().readyParry && !manager.PlayerDie && !saveAlready)
            {
                player.GetComponent<Player_Health_Manager>().Player_TakeDamage(transform, 0, tag);
                anim.SetTrigger("isLoosen");
                saveWall.SetActive(true);
                falgCollider.SetActive(false);
                saveParticle.SetActive(false);

                if (!tutorialObject)
                {
                    priviousCam.Priority = 0;
                    savePointCam.Priority = 10;
                    player.GetComponent<Player_Health_Manager>().Set_CurrentCam(savePointCam);

                    enemy.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
                    for (int i = 0; i < enemy.Count; i++)
                    {
                        if (enemy[i].GetComponent<Enemy_Manager>().isDead) Destroy(enemy[i]);
                        else continue;
                    }

                    Instantiate(saveActiveParticle, particlePos.position, Quaternion.identity);

                }

                Scene scene = SceneManager.GetActiveScene();
                dataManager.nowData.sceneName = scene.name;
                dataManager.nowData.savePos = savePointNum;
                dataManager.SaveData();
            }
        }
    }

    public void Set_SaveAlready()
    {
        saveAlready = true;
    }
}