using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class System_StartDialogue_TriggerEnter : MonoBehaviour
{
    [SerializeField] private DialogueSystemTrigger dialogueTrigger;
    [SerializeField] private Collider2D triggerCollier;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogueTrigger.OnUse();
            triggerCollier.enabled = false;
            GameManager.instance.Active_Dialogue = true;
        }
    }
}