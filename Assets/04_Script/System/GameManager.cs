using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PixelCrushers.DialogueSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool Active_Dialogue = false;

    public bool Stage01_Clear = false;

    public bool PlayerDie = false;
    public bool playerMove = false;

    public bool noOptionScene = true;

    public bool doHitstop;
    public bool LoadScene = false;
    public bool camMoveCheck = false;

    [SerializeField] private float cursorInvisibleTime;
    private bool cursorMove;
    private float cursorTimer;

    private EventSystem eventSystem;
    private GameObject LastSelectedUI;

    public bool dialogue_boss01 = false;
    public bool dialogue_boss02 = false;
    public bool dialogue_boss03 = false;
    public bool cutscene_boss04 = false;

    private string currentControllerScheme = "Keyboard&Mouse";
    public bool activateOptionUI;

    private void Awake()
    {
        #region ΩÃ±€≈Ê

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        #endregion ΩÃ±€≈Ê
    }

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        eventSystem = GameObject.FindObjectOfType<EventSystem>().GetComponent<EventSystem>();
        LastSelectedUI = eventSystem.firstSelectedGameObject;

        dialogue_boss01 = DataManager.instance.nowData.boss01_Dialogue;
        dialogue_boss02 = DataManager.instance.nowData.boss02_Dialogue;
        dialogue_boss03 = DataManager.instance.nowData.boss03_Dialogue;
        cutscene_boss04 = DataManager.instance.nowData.boss04_Cutscene;
    }

    private void Update()
    {

        if (eventSystem == null)
        {
            eventSystem = GameObject.FindObjectOfType<EventSystem>().GetComponent<EventSystem>();
            if (LastSelectedUI == null) LastSelectedUI = eventSystem.firstSelectedGameObject;
        }
        //Debug.Log("¥Î»≠ »∞º∫»≠ : " + Active_Dialogue);

        if (!cursorMove) cursorTimer -= Time.deltaTime;
        else cursorTimer = cursorInvisibleTime;


        if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {
            cursorMove = true;
        }
        else
        {
            cursorMove = false;
        }

        if (cursorTimer <= 0)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }

        if (dialogue_boss01 != DataManager.instance.nowData.boss01_Dialogue) dialogue_boss01 = DataManager.instance.nowData.boss01_Dialogue;
        if (dialogue_boss02 != DataManager.instance.nowData.boss02_Dialogue) dialogue_boss02 = DataManager.instance.nowData.boss02_Dialogue;
        if (dialogue_boss03 != DataManager.instance.nowData.boss03_Dialogue) dialogue_boss03 = DataManager.instance.nowData.boss03_Dialogue;
        if (cutscene_boss04 != DataManager.instance.nowData.boss04_Cutscene) cutscene_boss04 = DataManager.instance.nowData.boss04_Cutscene;


    }

    //====================== UI ¡¶æÓøÎ «‘ºˆ ===========================
    public void Set_CurrentControlScheme(string controlName)
    {
        currentControllerScheme = controlName;
    }

    public string Get_CurrentControlScheme()
    {
        return currentControllerScheme;
    }

    public void Set_LastSelectedUI(GameObject lastObject)
    {
        LastSelectedUI = lastObject;
    }
    public GameObject Get_LastSelectedUI()
    {
        return LastSelectedUI;
    }

    public string Get_LastSelectedUI_Name()
    {
        if (LastSelectedUI == null) return null;
        else return LastSelectedUI.name;
    }
    //=================================================================


    public void Activate()
    {
        Active_Dialogue = true;
    }

    public void DeActivate()
    {
        Active_Dialogue = false;
    }

    private void OnEnable()
    {
        // Make the functions available to Lua: (Replace these lines with your own.)
        Lua.RegisterFunction("Activate", this, SymbolExtensions.GetMethodInfo(() => Activate()));
        Lua.RegisterFunction("Deactivate", this, SymbolExtensions.GetMethodInfo(() => DeActivate()));
    }

    private void OnDisable()
    {
        // Remove the functions from Lua: (Replace these lines with your own.)
        Lua.UnregisterFunction("Activate");
        Lua.UnregisterFunction("Deactivate");
    }
}