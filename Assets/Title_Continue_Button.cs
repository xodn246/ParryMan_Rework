using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Title_Continue_Button : MonoBehaviour
{
    private DataManager dataManager;
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI continueText;

    private void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    private void Update()
    {
        if (dataManager.nowData.sceneName == "" || GameManager.instance.activateOptionUI)
        {
            continueButton.GetComponent<Image>().raycastTarget = false;
            continueButton.interactable = false;
        }
        else
        {
            continueButton.GetComponent<Image>().raycastTarget = true;
            continueButton.interactable = true;
        }
    }
}
