using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class FindComponent : MonoBehaviour
{
    public GameObject findObject;

    private void Start()
    {
        findObject = GameObject.FindObjectOfType<DialogueSystemTrigger>().gameObject;
    }
}
