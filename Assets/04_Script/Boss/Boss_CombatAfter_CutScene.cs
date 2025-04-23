using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_CombatAfter_CutScene : MonoBehaviour
{
    private Boss01_StartCutScene boss01CutScene;
    private Boss_Health_Manager bossHealth;

    [SerializeField] private bool clearDialogue_direct;

    private void Awake()
    {
        boss01CutScene = GameObject.Find("Boss01_CutScene").GetComponent<Boss01_StartCutScene>();
        bossHealth = transform.GetComponent<Boss_Health_Manager>();
    }

    private void Update()
    {
        if (bossHealth.currentHealth <= 0)
        {
            if (clearDialogue_direct) boss01CutScene.Clear_Boss_Dialogue();
            else
            {
                boss01CutScene.Clear_Boss01();
                Debug.Log(gameObject.name + " : 여기있었네");
            }
        }
    }
}
