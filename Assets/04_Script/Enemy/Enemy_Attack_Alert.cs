using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack_Alert : MonoBehaviour
{
    [SerializeField] private GameObject alertSky;
    [SerializeField] private GameObject alertPurple;

    [SerializeField] private Transform skyPos;
    [SerializeField] private Transform purplePos;


    public void Print_Alert_Sky()
    {
        GameObject alert = Instantiate(alertSky, skyPos.position, Quaternion.identity);
        alert.transform.SetParent(transform);
    }

    public void Print_Alert_Purple()
    {
        GameObject alert = Instantiate(alertPurple, purplePos.position, Quaternion.identity);
        alert.transform.SetParent(transform);
    }
}
