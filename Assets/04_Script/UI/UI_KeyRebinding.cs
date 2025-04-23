using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.IO;
using System.ComponentModel;
using Unity.VisualScripting;

public class UI_KeyRebinding : MonoBehaviour
{
    private EventSystem eventSystem;
    private PlayerInput playerInput;
    private DataManager dataManager;
    public InputActionAsset actions;

    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference parryAction;
    [SerializeField] private InputActionReference lookdownAction;
    [SerializeField] private InputActionReference resetAction;
    private InputActionReference currentSelectAction;
    private string currentPath = null;
    public int moveIndex;

    [SerializeField] private List<Button> navigationButton;

    [Space(10f)]
    [SerializeField] private List<Button> moveKey;

    [SerializeField] private List<GameObject> keyboardKey;
    [SerializeField] private List<GameObject> PSKey;
    [SerializeField] private List<GameObject> XBoxKey;

    [Space(10f)]
    [SerializeField] private List<GameObject> Selected_BindKey;

    private int keyCount = 6;

    [SerializeField] private GameObject firstObjectGamepad;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Awake()
    {
        dataManager = GameObject.FindObjectOfType<DataManager>().GetComponent<DataManager>();
    }

    private void Update()
    {
        if (eventSystem == null) eventSystem = GameObject.FindObjectOfType<EventSystem>().GetComponent<EventSystem>();
        if (playerInput == null) playerInput = GameObject.FindObjectOfType<PlayerInput>().GetComponent<PlayerInput>();

        //Debug.Log(playerInput.currentControlScheme);

        Change_Key_Image();
    }

    private void Change_Key_Image()
    {
        switch (playerInput.currentControlScheme)
        {
            case "Keyboard&Mouse":
                for (int i = 0; i < keyCount; i++)  // 현재 입력장치에 따라 키세팅 표기 변경
                {
                    keyboardKey[i].SetActive(true);
                    PSKey[i].SetActive(false);
                    XBoxKey[i].SetActive(false);
                }
                for (int i = 0; i < moveKey.Count; i++) // 현재 입력장치에 따라 키세팅 불가능한 버튼 비활성화 or 활성화
                {
                    moveKey[i].interactable = true;
                }

                Set_Input_KeyboardMouse();
                break;

            case "PS":
                for (int i = 0; i < keyCount; i++)
                {
                    keyboardKey[i].SetActive(false);
                    PSKey[i].SetActive(true);
                    XBoxKey[i].SetActive(false);
                }
                for (int i = 0; i < moveKey.Count; i++)
                {
                    moveKey[i].interactable = false;
                }

                Set_Input_Gamepad(); // 현재 선택된 버튼이 방향키 일경우 패링키로 변경 아니라면 그대로 유지
                break;

            case "XBOX":
                for (int i = 0; i < keyCount; i++)
                {
                    keyboardKey[i].SetActive(false);
                    PSKey[i].SetActive(false);
                    XBoxKey[i].SetActive(true);
                }
                for (int i = 0; i < moveKey.Count; i++)
                {
                    moveKey[i].interactable = false;
                }

                Set_Input_Gamepad();
                break;

            default:
                for (int i = 0; i < keyCount; i++)
                {
                    keyboardKey[i].SetActive(true);
                    PSKey[i].SetActive(false);
                    XBoxKey[i].SetActive(false);
                }
                for (int i = 0; i < moveKey.Count; i++)
                {
                    moveKey[i].interactable = true;
                }

                Set_Input_KeyboardMouse();
                break;
        }
    }

    public void Set_Input_Gamepad()
    {
        if (eventSystem.currentSelectedGameObject.name == "Button_Left_Key" || eventSystem.currentSelectedGameObject.name == "Button_Right_Key" || eventSystem.currentSelectedGameObject.name == "Button_Down_Key")  // 현재 선택된 버튼이 방향키 일경우 패링키로 변경 아니라면 그대로 유지
        {
            eventSystem.SetSelectedGameObject(firstObjectGamepad);
            GameManager.instance.Set_LastSelectedUI(eventSystem.currentSelectedGameObject);
        }
        else return;

        Navigation newNav01 = new Navigation();
        Navigation newNav02 = new Navigation();
        Navigation newNav03 = new Navigation();

        newNav01.mode = Navigation.Mode.Explicit;
        newNav02.mode = Navigation.Mode.Explicit;
        newNav03.mode = Navigation.Mode.Explicit;

        newNav01.selectOnUp = navigationButton[6];
        newNav01.selectOnDown = navigationButton[4];

        newNav02.selectOnUp = navigationButton[5];
        newNav02.selectOnDown = navigationButton[3];
        newNav02.selectOnLeft = navigationButton[7];
        newNav02.selectOnRight = navigationButton[7];

        newNav03.selectOnUp = navigationButton[5];
        newNav03.selectOnDown = navigationButton[3];
        newNav03.selectOnLeft = navigationButton[6];
        newNav03.selectOnRight = navigationButton[6];

        navigationButton[3].navigation = newNav01;
        navigationButton[6].navigation = newNav02;
        navigationButton[7].navigation = newNav03;
    }

    private void Set_Input_KeyboardMouse()
    {
        Navigation newNav01 = new Navigation();
        Navigation newNav02 = new Navigation();
        Navigation newNav03 = new Navigation();

        newNav01.mode = Navigation.Mode.Explicit;
        newNav02.mode = Navigation.Mode.Explicit;
        newNav03.mode = Navigation.Mode.Explicit;

        newNav01.selectOnUp = navigationButton[2];
        newNav01.selectOnDown = navigationButton[4];

        newNav02.selectOnUp = navigationButton[5];
        newNav02.selectOnDown = navigationButton[0];
        newNav02.selectOnLeft = navigationButton[7];
        newNav02.selectOnRight = navigationButton[7];

        newNav03.selectOnUp = navigationButton[5];
        newNav03.selectOnDown = navigationButton[0];
        newNav03.selectOnLeft = navigationButton[6];
        newNav03.selectOnRight = navigationButton[6];

        navigationButton[3].navigation = newNav01;
        navigationButton[6].navigation = newNav02;
        navigationButton[7].navigation = newNav03;
    }

    public void Set_Key_Image()
    {
        //setKeyboard
        keyboardKey[0].transform.GetComponent<Image>().sprite = dataManager.buttonSprite_Keyboard[dataManager.nowData.moveLeft_keyboard];
        keyboardKey[1].transform.GetComponent<Image>().sprite = dataManager.buttonSprite_Keyboard[dataManager.nowData.moveRight_keyboard];
        keyboardKey[2].transform.GetComponent<Image>().sprite = dataManager.buttonSprite_Keyboard[dataManager.nowData.lookDown_keyboard];
        keyboardKey[3].transform.GetComponent<Image>().sprite = dataManager.buttonSprite_Keyboard[dataManager.nowData.parry_keyboard];
        keyboardKey[4].transform.GetComponent<Image>().sprite = dataManager.buttonSprite_Keyboard[dataManager.nowData.jump_keyboard];
        keyboardKey[5].transform.GetComponent<Image>().sprite = dataManager.buttonSprite_Keyboard[dataManager.nowData.reset_keyboard];

        //setPS
        PSKey[3].transform.GetComponent<Image>().sprite = dataManager.buttonSprite_PS[dataManager.nowData.parry_PS];
        PSKey[4].transform.GetComponent<Image>().sprite = dataManager.buttonSprite_PS[dataManager.nowData.jump_PS];
        PSKey[5].transform.GetComponent<Image>().sprite = dataManager.buttonSprite_PS[dataManager.nowData.reset_PS];

        //setXBOX
        XBoxKey[3].transform.GetComponent<Image>().sprite = dataManager.buttonSprite_XBOX[dataManager.nowData.parry_XBOX];
        XBoxKey[4].transform.GetComponent<Image>().sprite = dataManager.buttonSprite_XBOX[dataManager.nowData.jump_XBOX];
        XBoxKey[5].transform.GetComponent<Image>().sprite = dataManager.buttonSprite_XBOX[dataManager.nowData.reset_XBOX];
    }

    public void Start_Rebinding_PlayerMove(int Index)
    {
        // Debug.Log("키 재설정 시작");
        // foreach (var binding in moveAction.action.bindings)
        // {
        //     Debug.Log("바인딩 전 : " + binding);
        // }
        currentSelectAction = moveAction;
        moveIndex = Index;
        Debug.Log("선택된 moveIndex : " + moveIndex);
        playerInput.SwitchCurrentActionMap("KeyBind");

        Set_DuplicatePath_Move();

        rebindingOperation = playerInput.actions["Move"].PerformInteractiveRebinding(Index)
                        .WithControlsExcluding("Mouse")
                        .WithControlsExcluding("Gamepad")
                        .WithControlsExcluding("Joystick")
                        .WithControlsExcluding("<Keyboard>/escape")
                        .WithControlsExcluding("<Keyboard>/backslash")
                        .WithControlsExcluding("<Keyboard>/f1")
                        .WithControlsExcluding("<Keyboard>/f2")
                        .WithControlsExcluding("<Keyboard>/f3")
                        .WithControlsExcluding("<Keyboard>/f4")
                        .WithControlsExcluding("<Keyboard>/f5")
                        .WithControlsExcluding("<Keyboard>/f6")
                        .WithControlsExcluding("<Keyboard>/f7")
                        .WithControlsExcluding("<Keyboard>/f8")
                        .WithControlsExcluding("<Keyboard>/f9")
                        .WithControlsExcluding("<Keyboard>/f10")
                        .WithControlsExcluding("<Keyboard>/f11")
                        .WithControlsExcluding("<Keyboard>/f12")
                        .WithControlsExcluding("<Keyboard>/minus")
                        .WithControlsExcluding("<Keyboard>/equals")
                        .WithControlsExcluding("<Keyboard>/anyKey")
                        .OnMatchWaitForAnother(0.1f)
                        .OnComplete(opteration => Rebind_Complete())
                        .Start();
    }

    public void Start_Rebinding(string actionName)
    {
        playerInput.SwitchCurrentActionMap("KeyBind");

        if (actionName == "Parry") currentSelectAction = parryAction;
        else if (actionName == "Jump") currentSelectAction = jumpAction;
        else if (actionName == "LookDown") currentSelectAction = lookdownAction;
        else if (actionName == "Reset") currentSelectAction = resetAction;

        // 컨트롤러 input 제한
        switch (playerInput.currentControlScheme)
        {
            case "Keyboard&Mouse":
                Set_DuplicatePath();

                // Debug.Log("바인딩 유무? " + currentSelectAction.action.bindings[0].hasOverrides);
                // Debug.Log("초기 경로? " + currentSelectAction.action.bindings[0].path);
                // Debug.Log("바인딩 경로? " + currentSelectAction.action.bindings[0].overridePath);
                Debug.Log("키마");
                Debug.Log(actionName);
                rebindingOperation = playerInput.actions[actionName].PerformInteractiveRebinding(0)
                        .WithCancelingThrough("<Keyboard>/escape")
                        .WithControlsExcluding("Mouse")
                        .WithControlsExcluding("Gamepad")
                        .WithControlsExcluding("Joystick")
                        .WithControlsExcluding("<Keyboard>/escape")
                        .WithControlsExcluding("<Keyboard>/backslash")
                        .WithControlsExcluding("<Keyboard>/f1")
                        .WithControlsExcluding("<Keyboard>/f2")
                        .WithControlsExcluding("<Keyboard>/f3")
                        .WithControlsExcluding("<Keyboard>/f4")
                        .WithControlsExcluding("<Keyboard>/f5")
                        .WithControlsExcluding("<Keyboard>/f6")
                        .WithControlsExcluding("<Keyboard>/f7")
                        .WithControlsExcluding("<Keyboard>/f8")
                        .WithControlsExcluding("<Keyboard>/f9")
                        .WithControlsExcluding("<Keyboard>/f10")
                        .WithControlsExcluding("<Keyboard>/f11")
                        .WithControlsExcluding("<Keyboard>/f12")
                        .WithControlsExcluding("<Keyboard>/minus")
                        .WithControlsExcluding("<Keyboard>/equals")
                        .WithControlsExcluding("<Keyboard>/anyKey")
                        .OnMatchWaitForAnother(0.2f)
                        .OnComplete(opteration => Rebind_Complete())
                        .Start();
                break;

            case "XBOX":
                Set_DuplicatePath();

                Debug.Log("액박");
                rebindingOperation = playerInput.actions[actionName].PerformInteractiveRebinding(1)
                        .WithControlsExcluding("Mouse")
                        .WithControlsExcluding("Keyboard")
                        .WithControlsExcluding("Joystick")
                        .WithControlsExcluding("<Gamepad>/leftStick/left")
                        .WithControlsExcluding("<Gamepad>/leftStick/right")
                        .WithControlsExcluding("<Gamepad>/leftStick/down")
                        .WithControlsExcluding("<Gamepad>/leftStick/up")
                        .WithControlsExcluding("<Gamepad>/leftStickPress")
                        .WithControlsExcluding("<Gamepad>/rightStick/left")
                        .WithControlsExcluding("<Gamepad>/rightStick/right")
                        .WithControlsExcluding("<Gamepad>/rightStick/down")
                        .WithControlsExcluding("<Gamepad>/rightStick/up")
                        .WithControlsExcluding("<Gamepad>/rightStickPress")
                        .WithControlsExcluding("<Gamepad>/dpad/left")
                        .WithControlsExcluding("<Gamepad>/dpad/right")
                        .WithControlsExcluding("<Gamepad>/dpad/down")
                        .WithControlsExcluding("<Gamepad>/dpad/up")
                        .WithControlsExcluding("<Gamepad>/select")
                        .WithControlsExcluding("<Gamepad>/start")
                        .OnMatchWaitForAnother(0.2f)
                        .OnComplete(opteration => Rebind_Complete())
                        .Start();
                break;

            case "PS":
                Set_DuplicatePath();

                Debug.Log("플스");
                rebindingOperation = playerInput.actions[actionName].PerformInteractiveRebinding(2)
                        .WithControlsExcluding("Mouse")
                        .WithControlsExcluding("Keyboard")
                        .WithControlsExcluding("Joystick")
                        .WithControlsExcluding("<Gamepad>/leftStick/left")
                        .WithControlsExcluding("<Gamepad>/leftStick/right")
                        .WithControlsExcluding("<Gamepad>/leftStick/down")
                        .WithControlsExcluding("<Gamepad>/leftStick/up")
                        .WithControlsExcluding("<Gamepad>/leftStickPress")
                        .WithControlsExcluding("<Gamepad>/rightStick/left")
                        .WithControlsExcluding("<Gamepad>/rightStick/right")
                        .WithControlsExcluding("<Gamepad>/rightStick/down")
                        .WithControlsExcluding("<Gamepad>/rightStick/up")
                        .WithControlsExcluding("<Gamepad>/rightStickPress")
                        .WithControlsExcluding("<Gamepad>/dpad/left")
                        .WithControlsExcluding("<Gamepad>/dpad/right")
                        .WithControlsExcluding("<Gamepad>/dpad/down")
                        .WithControlsExcluding("<Gamepad>/dpad/up")
                        .WithControlsExcluding("<Gamepad>/select")
                        .WithControlsExcluding("<Gamepad>/start")
                        .OnMatchWaitForAnother(0.2f)
                        .OnComplete(opteration => Rebind_Complete())
                        .Start();
                break;
        }
    }

    private void Rebind_Complete() //바인딩 완료시  실행  >>  바인딩된 정보에 맞는
    {
        if (currentSelectAction.name == "Player/Move") Duplicate_Processing_Move();
        Duplicate_Processing();

        Desplay_Slected_Function(-1);

        Save_Jump_Rebinding();
        Save_Move_Rebinding();
        Save_Parry_Rebinding();
        Save_Reset_Rebinding();
        Save_LookDown_Rebinding();

        dataManager.SaveData();

        Set_Key_Image();

        OnDisable();

        rebindingOperation.Dispose();

        playerInput.SwitchCurrentActionMap("Menu");
    }

    private void Duplicate_Processing_Move()
    {
        InputBinding leftKeyboardBinding = currentSelectAction.action.bindings[0];
        InputBinding rightKeyboardBinding = currentSelectAction.action.bindings[1];

        if (moveIndex == 0)
        {
            if (moveAction.action.bindings[1].effectivePath == leftKeyboardBinding.effectivePath)
            {
                currentSelectAction.action.ApplyBindingOverride(0, currentPath);
                //Debug.Log("Duplicate move binding found in keyboard : " + leftKeyboardBinding.effectivePath);
            }
        }
        else if (moveIndex == 1)
        {
            if (moveAction.action.bindings[0].effectivePath == rightKeyboardBinding.effectivePath)
            {
                currentSelectAction.action.ApplyBindingOverride(1, currentPath);
                //Debug.Log("Duplicate move binding found in keyboard : " + rightKeyboardBinding.effectivePath);
            }
        }
        else
        {
            //Debug.Log("아무일도 없었다");
        }
    }

    private void Duplicate_Processing()
    {
        switch (playerInput.currentControlScheme)
        {
            case "Keyboard&Mouse":
                InputBinding keyboardBinding = currentSelectAction.action.bindings[0];

                foreach (InputBinding binding in currentSelectAction.action.actionMap.bindings)
                {
                    if (binding.action == keyboardBinding.action)
                    {
                        continue;
                    }

                    if (binding.groups == "PS") continue;
                    if (binding.groups == "XBOX") continue;

                    if (binding.effectivePath == keyboardBinding.effectivePath)
                    {
                        currentSelectAction.action.ApplyBindingOverride(0, currentPath);
                        //Debug.Log("Duplicate binding found in keyboard : " + keyboardBinding.effectivePath);
                    }
                }
                break;

            case "XBOX":
                InputBinding xboxBinding = currentSelectAction.action.bindings[1];

                foreach (InputBinding binding in currentSelectAction.action.actionMap.bindings)
                {
                    if (binding.action == xboxBinding.action) continue;

                    if (binding.groups == "Keyboard&Mouse") continue;
                    if (binding.groups == "PS") continue;

                    if (binding.effectivePath == xboxBinding.effectivePath)
                    {
                        currentSelectAction.action.ApplyBindingOverride(1, currentPath);
                        Debug.Log("Duplicate binding found in xbox : " + xboxBinding.effectivePath);
                    }
                }
                break;

            case "PS":
                InputBinding psBinding = currentSelectAction.action.bindings[2];

                foreach (InputBinding binding in currentSelectAction.action.actionMap.bindings)
                {
                    Debug.Log(binding);
                    if (binding.action == psBinding.action) continue;

                    if (binding.groups == "Keyboard&Mouse") continue;
                    if (binding.groups == "XBOX") continue;

                    if (binding.effectivePath == psBinding.effectivePath)
                    {
                        currentSelectAction.action.ApplyBindingOverride(2, currentPath);
                        Debug.Log("Duplicate binding found in ps : " + psBinding.effectivePath);
                    }
                }
                break;
        }
    }

    private void Set_DuplicatePath_Move()
    {
        if (moveIndex == 0)
        {
            InputBinding binding = currentSelectAction.action.bindings[0];

            if (!binding.hasOverrides) currentPath = currentSelectAction.action.bindings[0].path;
            else currentPath = currentSelectAction.action.bindings[0].overridePath;
        }
        else
        {
            InputBinding binding = currentSelectAction.action.bindings[1];

            if (!binding.hasOverrides) currentPath = currentSelectAction.action.bindings[1].path;
            else currentPath = currentSelectAction.action.bindings[1].overridePath;
        }
    }

    private void Set_DuplicatePath()
    {
        switch (playerInput.currentControlScheme)
        {
            case "Keyboard&Mouse":
                InputBinding keyboardBinding = currentSelectAction.action.bindings[0];

                if (!keyboardBinding.hasOverrides) currentPath = currentSelectAction.action.bindings[0].path;
                else currentPath = currentSelectAction.action.bindings[0].overridePath;
                break;

            case "XBOX":
                InputBinding xboxBinding = currentSelectAction.action.bindings[1];

                if (!xboxBinding.hasOverrides) currentPath = currentSelectAction.action.bindings[1].path;
                else currentPath = currentSelectAction.action.bindings[1].overridePath;
                break;

            case "PS":
                InputBinding psBinding = currentSelectAction.action.bindings[2];

                if (!psBinding.hasOverrides) currentPath = currentSelectAction.action.bindings[2].path;
                else currentPath = currentSelectAction.action.bindings[2].overridePath;
                break;
        }

        Debug.Log("기존 경로 : " + currentPath);
    }

    public void Display_Select_BindKey(string actionName)
    {
        switch (actionName)
        {
            case "Left":
                Desplay_Slected_Function(0);
                break;

            case "Right":
                Desplay_Slected_Function(1);
                break;

            case "Down":
                Desplay_Slected_Function(2);
                break;

            case "Parry":
                Desplay_Slected_Function(3);
                break;

            case "Jump":
                Desplay_Slected_Function(4);
                break;

            case "Reset":
                Desplay_Slected_Function(5);
                break;

            default:
                Desplay_Slected_Function(-1);
                break;
        }
    }

    private void Desplay_Slected_Function(int index)
    {
        for (int i = 0; i < Selected_BindKey.Count; i++)
        {
            if (i == index) Selected_BindKey[i].SetActive(true);
            else Selected_BindKey[i].SetActive(false);
        }
    }

    private void Save_Move_Rebinding()
    {
        var binding = moveAction.action.bindings;
        dataManager.nowData.moveLeft_keyboard = binding[0].effectivePath;
        dataManager.nowData.moveRight_keyboard = binding[1].effectivePath;
    }

    private void Save_Parry_Rebinding()
    {
        var binding = parryAction.action.bindings;
        dataManager.nowData.parry_keyboard = binding[0].effectivePath;
        dataManager.nowData.parry_XBOX = binding[1].effectivePath;
        dataManager.nowData.parry_PS = binding[2].effectivePath;
    }

    private void Save_Jump_Rebinding()
    {
        var binding = jumpAction.action.bindings;
        dataManager.nowData.jump_keyboard = binding[0].effectivePath;
        dataManager.nowData.jump_XBOX = binding[1].effectivePath;
        dataManager.nowData.jump_PS = binding[2].effectivePath;
    }

    private void Save_Reset_Rebinding()
    {
        var binding = resetAction.action.bindings;
        dataManager.nowData.reset_keyboard = binding[0].effectivePath;
        dataManager.nowData.reset_XBOX = binding[1].effectivePath;
        dataManager.nowData.reset_PS = binding[2].effectivePath;
    }

    private void Save_LookDown_Rebinding()
    {
        var binding = lookdownAction.action.bindings;
        dataManager.nowData.lookDown_keyboard = binding[0].effectivePath;
    }

    public void OnEnable()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);
    }

    public void OnDisable()
    {
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

    public void ResetAllBindings()
    {
        foreach (InputActionMap map in actions.actionMaps)
            map.RemoveAllBindingOverrides();

        //datamanager 에서 키 경로 디폴트로 조정
        dataManager.Initialize_Keybinding();
        dataManager.SaveData();

        PlayerPrefs.DeleteKey("rebinds");

        Set_Key_Image();
    }
}