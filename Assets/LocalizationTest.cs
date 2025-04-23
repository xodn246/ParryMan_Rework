using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class LocalizationTest : MonoBehaviour
{
    public void Change_English()
    {
        DialogueManager.SetLanguage("en");
    }

    public void Change_Korean()
    {
        DialogueManager.SetLanguage("");
    }
}
