using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Tutorial_Text_Manager : MonoBehaviour
{
    private enum Tutorial_type
    {
        move,
        jump,
        parry,
        lookdown,
        reset,
        skip,
        other
    }

    private Animator anim;
    private PlayerInput playerInput;
    private GameManager gameManager;
    private DataManager dataManager;

    [SerializeField] private Tutorial_type tutorialType;
    [SerializeField] private Image tutorial_image01;
    [SerializeField] private Image tutorial_image02;

    private void Awake()
    {
        anim = transform.GetComponent<Animator>();
        gameManager = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();
        dataManager = GameObject.FindObjectOfType<DataManager>().GetComponent<DataManager>();
    }

    private void Update()
    {
        if (playerInput == null) playerInput = GameObject.FindObjectOfType<PlayerInput>().GetComponent<PlayerInput>();
        Set_Icon_With_Keybinding();
    }

    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.CompareTag("PlayerCheck"))
        {
            anim.SetTrigger("Apear");
        }
    }

    private void OnTriggerExit2D(Collider2D player)
    {
        if (player.CompareTag("PlayerCheck"))
        {
            anim.SetTrigger("Disapear");
        }
    }

    private void Set_Icon_With_Keybinding() // 해당되는 버튼 또는 자판 아이콘 출력용 함수
    {
        switch (gameManager.Get_CurrentControlScheme())
        {
            case "Keyboard&Mouse":
                switch (tutorialType)
                {
                    case Tutorial_type.move:
                        tutorial_image01.sprite = dataManager.buttonSprite_Keyboard[playerInput.actions["Move"].bindings[0].effectivePath];
                        tutorial_image02.sprite = dataManager.buttonSprite_Keyboard[playerInput.actions["Move"].bindings[1].effectivePath];
                        break;

                    case Tutorial_type.lookdown:
                        tutorial_image02.transform.gameObject.SetActive(false);
                        tutorial_image01.sprite = dataManager.buttonSprite_Keyboard[playerInput.actions["LookDown"].bindings[0].effectivePath];
                        break;

                    case Tutorial_type.jump:
                        tutorial_image01.sprite = dataManager.buttonSprite_Keyboard[playerInput.actions["Jump"].bindings[0].effectivePath];
                        break;

                    case Tutorial_type.parry:
                        tutorial_image01.sprite = dataManager.buttonSprite_Keyboard[playerInput.actions["Parry"].bindings[0].effectivePath];
                        break;

                    case Tutorial_type.reset:
                        tutorial_image01.sprite = dataManager.buttonSprite_Keyboard[playerInput.actions["Reset"].bindings[0].effectivePath];
                        break;

                    case Tutorial_type.skip:
                        tutorial_image01.sprite = dataManager.buttonSprite_Keyboard[playerInput.actions["Skip"].bindings[0].effectivePath];
                        break;
                }
                break;

            case "XBOX":
                switch (tutorialType)
                {
                    case Tutorial_type.move:
                        tutorial_image01.sprite = dataManager.buttonSprite_XBOX["Tutorial_Leftstick"];
                        tutorial_image02.sprite = dataManager.buttonSprite_XBOX["Tutorial_Dpad"];
                        break;

                    case Tutorial_type.lookdown:
                        tutorial_image02.transform.gameObject.SetActive(true);
                        tutorial_image02.sprite = dataManager.buttonSprite_XBOX["Tutorial_Down_Leftstic"];
                        tutorial_image01.sprite = dataManager.buttonSprite_XBOX["Tutorial_Down_Dpad"];
                        break;

                    case Tutorial_type.jump:
                        tutorial_image01.sprite = dataManager.buttonSprite_XBOX[playerInput.actions["Jump"].bindings[1].effectivePath];
                        break;

                    case Tutorial_type.parry:
                        tutorial_image01.sprite = dataManager.buttonSprite_XBOX[playerInput.actions["Parry"].bindings[1].effectivePath];
                        break;

                    case Tutorial_type.reset:
                        tutorial_image01.sprite = dataManager.buttonSprite_XBOX[playerInput.actions["Reset"].bindings[1].effectivePath];
                        break;

                    case Tutorial_type.skip:
                        tutorial_image01.sprite = dataManager.buttonSprite_XBOX[playerInput.actions["Skip"].bindings[1].effectivePath];
                        break;
                }
                break;

            case "PS":
                switch (tutorialType)
                {
                    case Tutorial_type.move:
                        tutorial_image01.sprite = dataManager.buttonSprite_PS["Tutorial_Leftstick"];
                        tutorial_image02.sprite = dataManager.buttonSprite_PS["Tutorial_Dpad"];
                        break;

                    case Tutorial_type.lookdown:
                        tutorial_image02.transform.gameObject.SetActive(true);
                        tutorial_image02.sprite = dataManager.buttonSprite_PS["Tutorial_Down_Leftstic"];
                        tutorial_image01.sprite = dataManager.buttonSprite_PS["Tutorial_Down_Dpad"];
                        break;

                    case Tutorial_type.jump:
                        tutorial_image01.sprite = dataManager.buttonSprite_PS[playerInput.actions["Jump"].bindings[2].effectivePath];
                        break;

                    case Tutorial_type.parry:
                        tutorial_image01.sprite = dataManager.buttonSprite_PS[playerInput.actions["Parry"].bindings[2].effectivePath];
                        break;

                    case Tutorial_type.reset:
                        tutorial_image01.sprite = dataManager.buttonSprite_PS[playerInput.actions["Reset"].bindings[2].effectivePath];
                        break;

                    case Tutorial_type.skip:
                        tutorial_image01.sprite = dataManager.buttonSprite_PS[playerInput.actions["Skip"].bindings[2].effectivePath];
                        break;
                }
                break;
        }
    }
}