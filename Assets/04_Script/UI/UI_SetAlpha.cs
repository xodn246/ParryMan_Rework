using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class UI_SetAlpha : MonoBehaviour
{
    [SerializeField] private Image image;

    public void Set_Alpha(float amount)
    {
        Color col = image.color;
        col.a = amount;
        image.color = col;
    }
}
