using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;


public class PauseMenu : MonoBehaviour, IPointerEnterHandler
{
    private GameManager manager;
    private DataManager dataManager;
    private Player_Health_Manager playerManager;
    private PlayerInput playerInput;
    private SettingsMenu settingMenu;
    private Input_Player inputPlayerAction;
    private EventSystem eventSystem;
    [SerializeField] private UI_KeyRebinding uiKeyBind;


    public static bool GameIsPaused = false;

    public GameObject pauseMenuCanvas;
    public GameObject pauseMenu;
    public GameObject optionMenu;
    public GameObject keybindMenu;
    public static PauseMenu instance;

    [Space(10f)]
    [SerializeField] private GameObject titleFirstUIObject;
    [SerializeField] private GameObject titleOptionUIObject;
    [SerializeField] private GameObject puaseFirstUIObject;
    [SerializeField] private GameObject optionFirstUIobject;
    [SerializeField] private GameObject keybindFirstUIobject;
    [SerializeField] private GameObject keybindFirstUIobject_Controller;

    [Space(10f)]
    [SerializeField] private List<Image> slideSelect;
    [SerializeField] private List<GameObject> pauseIndicator;
    [SerializeField] private List<GameObject> optionIndicator;
    [SerializeField] private List<Image> keybindSelect;
    [SerializeField] private List<GameObject> keybindIndicator;


    [Space(10f)]
    [Header("Title Button Setting")]
    [SerializeField] private List<Button> titleButton;

    private string currentScene;

    public bool inputPause; // ���� �Է¿� �Ұ�
    public bool doPause = false;

    private void Awake()
    {
        #region �̱���

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        #endregion �̱���

        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        inputPlayerAction = new Input_Player();
        settingMenu = transform.Find("UI_OptionMenu").GetComponent<SettingsMenu>();
    }


    private void Update()
    {
        if (eventSystem == null)
        {
            eventSystem = GameObject.FindObjectOfType<EventSystem>().GetComponent<EventSystem>();
            eventSystem.firstSelectedGameObject = puaseFirstUIObject;
        }

        if (playerInput == null) playerInput = GameObject.FindObjectOfType<PlayerInput>().GetComponent<PlayerInput>();

        currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "Title" && currentScene != "TeamLogo" && !currentScene.Contains("CutScene"))
        {
            if (playerManager == null) playerManager = GameObject.Find("Player(Clone)").GetComponent<Player_Health_Manager>();
        }

        //Debug.Log("GameIsPaused : " + GameIsPaused);

        if (!manager.LoadScene)
        {
            if (!manager.noOptionScene)
            {
                if (!FindObjectOfType<GameManager>().Active_Dialogue && inputPause)
                {
                    if (GameIsPaused && !doPause)
                    {
                        doPause = true;
                        Resume();
                    }
                    else if (!GameIsPaused && !doPause)
                    {
                        doPause = true;
                        Pause();
                    }
                }
            }
        }
        else
        {
            if (!FindObjectOfType<GameManager>().Active_Dialogue && inputPause)
            {
                Debug.Log("������ ������ �󰡸���");
            }
        }

        Set_SelectObject_Input(); // �̰� ������ D-Pad �۵��� �ȵ� �Ƹ���...

        Pirnt_PauseIndicator();

        Print_OptionIndicator();
        SliderSelect_ColorChange();

        Print_KeybindIndicator();
        KeybindSelect_ColorChange();
    }

    public void OnPointerEnter(PointerEventData eventData) //���콺 �ø� ������Ʈ�� ���õ� ������Ʈ�� �������ִ� �Լ�  \^p^/  \^q^/
    {
        if (eventData.pointerCurrentRaycast.gameObject.CompareTag("UI_Button"))
        {
            eventData.pointerCurrentRaycast.gameObject.GetComponent<Button>().Select();
            GameManager.instance.Set_LastSelectedUI(eventData.pointerCurrentRaycast.gameObject);
        }
        else if (eventData.pointerCurrentRaycast.gameObject.CompareTag("UI_Slider"))
        {
            eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slider>().Select();
            GameManager.instance.Set_LastSelectedUI(eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slider>().gameObject);
        }
        else if (eventData.pointerCurrentRaycast.gameObject.CompareTag("UI_Dropdown"))
        {
            eventData.pointerCurrentRaycast.gameObject.GetComponent<TMP_Dropdown>().Select();
            GameManager.instance.Set_LastSelectedUI(eventData.pointerCurrentRaycast.gameObject);
        }
        else if (eventData.pointerCurrentRaycast.gameObject.CompareTag("UI_Toggle"))
        {
            eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Toggle>().Select();
            GameManager.instance.Set_LastSelectedUI(eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Toggle>().gameObject);
        }
        else return;


        Debug.Log(eventData.pointerCurrentRaycast.gameObject);
    }

    private void Set_SelectObject_Input()
    {
        if (inputPlayerAction.Menu.Navigate.ReadValue<Vector2>() != Vector2.zero) // Ű���� �Է½� �̺�Ʈ�ý��ۿ� ���õ� ������Ʈ�� ������� ���� �ֱ� ���õǾ��� ������Ʈ �־��ֱ�
        {
            if (eventSystem.currentSelectedGameObject == null)
            {
                eventSystem.SetSelectedGameObject(GameManager.instance.Get_LastSelectedUI());
            }

            GameManager.instance.Set_LastSelectedUI(eventSystem.currentSelectedGameObject);
        }
    }

    public void Resume()
    {
        playerInput.SwitchCurrentActionMap("Player");
        inputPlayerAction.Menu.Disable();
        inputPlayerAction.Player.Enable();
        pauseMenu.SetActive(false);
        optionMenu.SetActive(false);
        keybindMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        dataManager.SaveData();
    }

    private void Pause()
    {
        if (currentScene != "Title" && !currentScene.Contains("CutScene"))
        {
            if (manager.doHitstop) System_HitStop.instance.StopHitstop();//playerManager.StopHitstop();
        }
        playerInput.SwitchCurrentActionMap("Menu");
        inputPlayerAction.Menu.Enable();
        inputPlayerAction.Player.Disable();
        Debug.Log(playerInput.currentActionMap.name);
        eventSystem.SetSelectedGameObject(puaseFirstUIObject);
        GameManager.instance.Set_LastSelectedUI(eventSystem.currentSelectedGameObject);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    #region Puase_Menu

    public void Pirnt_PauseIndicator()
    {
        switch (GameManager.instance.Get_LastSelectedUI_Name())
        {
            case "Button_Continue":
                UI_Print_Indicator(pauseIndicator, 0);
                break;

            case "Button_Option":
                UI_Print_Indicator(pauseIndicator, 1);
                break;

            case "Button_Exit":
                UI_Print_Indicator(pauseIndicator, 2);
                break;

            default:
                UI_Print_Indicator(pauseIndicator, -1);
                break;
        }
    }

    public void Puase_OptionButton()
    {
        if (currentScene == "Title")  //Ÿ��Ʋ ȭ�鿡�� �ɼ� ������� Ÿ��Ʋ ȭ���� ��ư�� �����ɽ�Ʈ ��Ȱ��ȭ
        {
            GameManager.instance.activateOptionUI = true;
            for (int i = 0; i < titleButton.Count; i++)
            {
                titleButton[i].GetComponent<Image>().raycastTarget = false;
            }
        }

        pauseMenu.SetActive(false);
        optionMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(optionFirstUIobject);
        GameManager.instance.Set_LastSelectedUI(eventSystem.currentSelectedGameObject);
    }

    public void Pause_ExitButton(string sceneName) //Ÿ��Ʋ ȭ������ �����鼭 ���� ���� ����
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");  // �޴��� ������ �����ϴ� �� ����
        for (var i = 0; i < enemies.Length; i++)
        {
            Destroy(enemies[i]);
        }

        GameIsPaused = false;
        SceneLoader.Instance.LoadScene("Title");
        manager.noOptionScene = true;
        Destroy(GameObject.Find("SoundManager").gameObject);
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    #endregion Puase_Menu

    #region Option_Menu

    public void SliderSelect_ColorChange()
    {
        Color defaultColor = new Color(0.8705882f, 0.8705882f, 0.8705882f, 1f);
        Color selectColor = new Color(0f, 0.772727f, 1f, 1f);

        switch (GameManager.instance.Get_LastSelectedUI_Name())
        {
            case "Slider_master":
                UI_ChangeColor_Image(slideSelect, 0);
                break;

            case "Slider_music":
                UI_ChangeColor_Image(slideSelect, 1);
                break;

            case "Slider_sfx":
                UI_ChangeColor_Image(slideSelect, 2);
                break;

            default:
                UI_ChangeColor_Image(slideSelect, -1);
                break;
        }
    }

    public void Print_OptionIndicator()
    {
        switch (GameManager.instance.Get_LastSelectedUI_Name())
        {
            case "Button_keysetting":
                UI_Print_Indicator(optionIndicator, 0);
                break;

            case "Button_back":
                UI_Print_Indicator(optionIndicator, 1);
                break;

            case "Button_initialize":
                UI_Print_Indicator(optionIndicator, 2);
                break;

            default:
                UI_Print_Indicator(optionIndicator, -1);
                break;
        }
    }

    public void Option_KeybindButton()
    {
        optionMenu.SetActive(false);
        keybindMenu.SetActive(true);

        uiKeyBind.Set_Key_Image();
        uiKeyBind.OnEnable();

        if (playerInput.currentControlScheme == "Keyboard&Mouse") //Ű����� ���۹� ���Խ�
        {
            eventSystem.SetSelectedGameObject(keybindFirstUIobject);
            GameManager.instance.Set_LastSelectedUI(eventSystem.currentSelectedGameObject);
        }
        else   //��Ʈ�ѷ��� ���۹� ���Խ�
        {
            eventSystem.SetSelectedGameObject(keybindFirstUIobject_Controller);
            GameManager.instance.Set_LastSelectedUI(eventSystem.currentSelectedGameObject);
            uiKeyBind.Set_Input_Gamepad();
        }
    }

    public void Option_BackButton() //���� ������ ����
    {
        if (currentScene == "Title") //Ÿ��Ʋ ȭ�鿡�� �ڷΰ��� ������� Ÿ��Ʋ ȭ���� ��ư�� �����ɽ�Ʈ Ȱ��ȭ
        {
            GameManager.instance.activateOptionUI = false;
            for (int i = 0; i < titleButton.Count; i++)
            {
                titleButton[i].GetComponent<Image>().raycastTarget = true;
            }

            optionMenu.SetActive(false);
            eventSystem.SetSelectedGameObject(titleOptionUIObject); // Ÿ��Ʋ�� option ��ư�� �ʱ��ư���� ���� : option ��ư���� �����߱⶧���� �װ� �������ֱ�����
            GameManager.instance.Set_LastSelectedUI(eventSystem.currentSelectedGameObject);
        }
        else
        {
            pauseMenu.SetActive(true);
            optionMenu.SetActive(false);
            eventSystem.SetSelectedGameObject(puaseFirstUIObject);
            GameManager.instance.Set_LastSelectedUI(eventSystem.currentSelectedGameObject);
        }
        dataManager.SaveData();
    }

    #endregion Option_Menu

    #region Keybind_Menu

    public void Keybind_BackButton()
    {
        optionMenu.SetActive(true);
        keybindMenu.SetActive(false);

        //Ű���� ���� >> datamanager �� ���� �߰� �ʿ�

        eventSystem.SetSelectedGameObject(optionFirstUIobject);
        GameManager.instance.Set_LastSelectedUI(eventSystem.currentSelectedGameObject);
    }


    public void Keybind_ResetButton()
    {
        // Ű���� �ʱ�ȭ ��ư >> datamanager �� ���� �߰� �ʿ�
    }

    public void KeybindSelect_ColorChange()
    {


        switch (GameManager.instance.Get_LastSelectedUI_Name())
        {
            case "Button_Left_Key":
                UI_ChangeColor_Image(keybindSelect, 0);
                break;

            case "Button_Right_Key":
                UI_ChangeColor_Image(keybindSelect, 1);
                break;

            case "Button_Down_Key":
                UI_ChangeColor_Image(keybindSelect, 2);
                break;

            case "Button_Parry_Key":
                UI_ChangeColor_Image(keybindSelect, 3);
                break;

            case "Button_Jump_Key":
                UI_ChangeColor_Image(keybindSelect, 4);
                break;

            case "Button_Reset_Key":
                UI_ChangeColor_Image(keybindSelect, 5);
                break;

            default:
                UI_ChangeColor_Image(keybindSelect, -1);
                break;
        }
    }

    public void Print_KeybindIndicator()
    {
        switch (GameManager.instance.Get_LastSelectedUI_Name())
        {
            case "Button_Keybind_Back":
                UI_Print_Indicator(keybindIndicator, 0);
                break;

            case "Button_Keybind_Initialize":
                UI_Print_Indicator(keybindIndicator, 1);
                break;

            default:
                UI_Print_Indicator(keybindIndicator, -1);
                break;
        }
    }

    #endregion Keybind_Menu


    private void UI_ChangeColor_Image(List<Image> selectImage, int selectNum)
    {
        Color defaultColor = new Color(0.8705882f, 0.8705882f, 0.8705882f, 1f);
        Color selectColor = new Color(0f, 0.772727f, 1f, 1f);

        for (int i = 0; i < selectImage.Count; i++)
        {
            if (selectNum == i) selectImage[i].color = selectColor;
            else selectImage[i].color = defaultColor;
        }
    }

    private void UI_Print_Indicator(List<GameObject> selectObject, int selectNum)
    {
        for (int i = 0; i < selectObject.Count; i++)
        {
            if (selectNum == i) selectObject[i].SetActive(true);
            else selectObject[i].SetActive(false);
        }
    }
}