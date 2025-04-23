using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class UI_Dialogue_Change_Portrait : MonoBehaviour
{
    [SerializeField] private Actor currentActor;
    [SerializeField] private List<Sprite> portrait;

    public void Change_Portrait(int portraitIndex)
    {
        currentActor.spritePortrait = portrait[portraitIndex];
    }
}
