using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_ParryCooltimeUI : MonoBehaviour
{
    private Player_Manager playerManager;

    [SerializeField] private GameObject coolTimeObject;
    [SerializeField] private Image coolTimeBar1;
    [SerializeField] private Image coolTimeBar2;

    [SerializeField] private bool coolTime = false;


    private void Awake()
    {
        playerManager = GetComponentInParent<Player_Manager>();
    }

    private void Update()
    {
        if (!coolTime && playerManager.Get_ParryCooltime() > 0)
        {
            coolTime = true;

            if (playerManager.isParry || playerManager.isJumpParry) coolTimeObject.SetActive(false);
            else coolTimeObject.SetActive(true);

            coolTimeBar1.fillAmount = 0;
            coolTimeBar2.fillAmount = 0;
        }
        else if (playerManager.Get_ParryCooltime() <= 0)
        {
            coolTimeObject.SetActive(false);
            coolTime = false;
        }
    }

    private IEnumerator CoolTimeUI()
    {
        float timer = 0;
        while (playerManager.Get_ParryCooltime() > 0)
        {
            yield return null;
            timer += Time.deltaTime;

            float color = Mathf.Lerp(0, 1, timer);
            coolTimeBar1.fillAmount = Mathf.Lerp(0, 1, timer);
            coolTimeBar1.color = new Color(1, color, color);

            coolTimeBar2.fillAmount = Mathf.Lerp(0, 1, timer);
            coolTimeBar2.color = new Color(1, color, color);
        }
    }

    public void Print_Cooltime_UI()
    {
        StartCoroutine(CoolTimeUI());
    }
}
