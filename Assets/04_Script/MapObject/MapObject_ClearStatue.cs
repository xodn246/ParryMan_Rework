using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject_ClearStatue : MonoBehaviour
{
    private GameManager manager;
    private DataManager dataManager;

    [SerializeField] private GameObject wholeStatue;
    [SerializeField] private List<GameObject> brokeStatue;
    [SerializeField] private GameObject statueCollider;
    [SerializeField] private GameObject statueActivePrticle;

    [Space(10f)]
    [SerializeField] private GameObject statueParticle;

    [SerializeField] private Transform particlePos;

    [Space(10f)]
    [SerializeField] private List<GameObject> enemy;

    [Space(10f)]
    [SerializeField] private string nextScene;

    [SerializeField] private bool saveAnotherScene;
    [SerializeField] private string anotherScene;

    [Space(10f)]
    [SerializeField] private bool loadNoOptionScene;

    [Space(10f)]
    [SerializeField] private bool killSoundManager;

    [Space(10f)]
    [SerializeField] private bool initailizeDialogue;

    private void Awake()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            if (player.GetComponentInParent<Player_Manager>().readyParry && !manager.PlayerDie)
            {
                player.GetComponentInChildren<CapsuleCollider2D>().enabled = false;
                player.GetComponent<Player_Health_Manager>().Player_TakeDamage(transform, 0, tag);
                statueCollider.SetActive(false);
                statueActivePrticle.SetActive(false);

                enemy.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
                for (int i = 0; i < enemy.Count; i++)
                {
                    Destroy(enemy[i]);
                }

                Instantiate(statueParticle, particlePos.position, Quaternion.identity);

                wholeStatue.SetActive(false);
                for (int i = 0; i < brokeStatue.Count; i++)
                {
                    brokeStatue[i].SetActive(true);
                }

                if (loadNoOptionScene) manager.noOptionScene = true;

                if (killSoundManager) Destroy(GameObject.Find("SoundManager").gameObject);

                // 모든 대화 기록 초기화시 여기에 괄호 넣고 추가
                if (initailizeDialogue)
                {
                    dataManager.nowData.boss01_Dialogue = false;
                    dataManager.nowData.boss02_Dialogue = false;
                    dataManager.nowData.boss03_Dialogue = false;
                }

                // 다음씬 로드
                SceneLoader.Instance.LoadScene(nextScene);
                if (!saveAnotherScene)
                {
                    dataManager.nowData.sceneName = nextScene;
                }
                else
                {
                    dataManager.nowData.sceneName = anotherScene;
                }
                dataManager.nowData.savePos = 0;
                dataManager.SaveData();
            }
        }
    }
}