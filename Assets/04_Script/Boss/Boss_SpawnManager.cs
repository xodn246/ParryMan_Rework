using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Boss_SpawnManager : MonoBehaviour
{
    private Scene scene;

    [SerializeField] private GameObject bossPrefab;

    [Space(10f)]
    [SerializeField] private Transform beforedialugePos;

    [SerializeField] private Transform afterDialoguePos;

    private void Awake()
    {
        scene = SceneManager.GetActiveScene();
    }

    private void Start()
    {
        SpawnBoss();
    }

    public void SpawnBoss()
    {
        switch (scene.name)
        {
            case "Bamboo_Boss":
                if (!GameManager.instance.dialogue_boss01)
                {
                    Instantiate(bossPrefab, beforedialugePos.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(bossPrefab, afterDialoguePos.position, Quaternion.identity);
                }
                break;

            case "Sakura_Boss":
                if (!GameManager.instance.dialogue_boss02)
                {
                    Instantiate(bossPrefab, beforedialugePos.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(bossPrefab, afterDialoguePos.position, Quaternion.identity);
                }
                break;

            case "Beach_Boss":
                if (!GameManager.instance.dialogue_boss03)
                {
                    Instantiate(bossPrefab, beforedialugePos.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(bossPrefab, afterDialoguePos.position, Quaternion.identity);
                }
                break;

            case "Master_Boss":
                if (!GameManager.instance.cutscene_boss04)
                {
                    break;
                }
                else
                {
                    Instantiate(bossPrefab, afterDialoguePos.position, Quaternion.identity);
                }
                break;

            default:
                Instantiate(bossPrefab, afterDialoguePos.position, Quaternion.identity);
                break;
        }
    }

    public void SpanwBoss_External(Transform spawnPos)
    {
        Instantiate(bossPrefab, spawnPos.position, Quaternion.identity);
    }
}