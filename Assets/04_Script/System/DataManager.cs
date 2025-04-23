using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PixelCrushers.DialogueSystem;
using UnityEngine.InputSystem;

public class GameData
{
    // 죽은 횟수, 경과 시간, 마지막 입장 씬, 저장 위치(번호), 해상도, 전체화면 상태, 언어, 마스터볼륨, 음악볼륨, 효과음, 서포트모드
    public int deathCount;
    public int parryCount;

    public float elapsedTime;
    public string sceneName;
    public int savePos; // 저장 위치를 번호로 표시
    public int resolution;
    public bool fullScreen;
    public int languageIndex;

    public float masterVol;
    public float masterVolMixer;

    public float musicVol;
    public float musicVolMixer;

    public float sfxVol;
    public float sfxVolMixer;

    [Space(10f)]
    [Header("KeyBinding")]
    [Header("Keyboard")]
    public string moveLeft_keyboard;

    public string moveRight_keyboard;
    public string lookDown_keyboard;
    public string jump_keyboard;
    public string parry_keyboard;
    public string reset_keyboard;

    [Space(5f)]
    [Header("PS")]
    public string jump_PS;

    public string parry_PS;
    public string reset_PS;

    [Space(5f)]
    [Header("XBOX")]
    public string jump_XBOX;

    public string parry_XBOX;
    public string reset_XBOX;

    public bool supportMode;

    // ====================== 보스 컷씬 관련 저장 요소 ( 대화 완료 유뮤 ) ===========================

    public bool boss01_Dialogue;
    public bool boss02_Dialogue;
    public bool boss03_Dialogue;
    public bool boss04_Cutscene;
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    // =================================== 키 바인딩 데이터 =======================================

    public InputActionAsset actions;
    public SerializableDictionary<string, Sprite> buttonSprite_Keyboard;
    public SerializableDictionary<string, Sprite> buttonSprite_PS;
    public SerializableDictionary<string, Sprite> buttonSprite_XBOX;

    // ===========================================================================================

    public GameData nowData = new GameData();

    #region InitialzeData

    private int d_deathCount = 0;
    private int d_parryCount = 0;
    private float d_elapsedTime = 0;
    private string d_sceneName = "";
    private int d_savePos = 0;
    private int d_resolution = 0;
    private bool d_fullScreen = true;
    private int d_languageIndex = 1; // 0 : 한국어 ,, 1 : 영어
    private float d_masterVol = 0.7f;
    private float d_masterVolMixer = Mathf.Log10(0.7f) * 20;
    private float d_musicVol = 0.7f;
    private float d_musicVolMixer = Mathf.Log10(0.7f) * 20;
    private float d_sfxVol = 0.7f;
    private float d_sfxVolMixer = Mathf.Log10(0.7f) * 20;

    //== KeyBoard ==
    private string d_moveLeft_keyboard = "<Keyboard>/leftArrow";

    private string d_moveRight_keyboard = "<Keyboard>/rightArrow";
    private string d_lookDown_keyboard = "<Keyboard>/downArrow";
    private string d_jump_keyboard = "<Keyboard>/c";
    private string d_parry_keyboard = "<Keyboard>/z";
    private string d_reset_keyboard = "<Keyboard>/r";

    //== PS ==
    private string d_jump_PS = "<Gamepad>/buttonSouth";

    private string d_parry_PS = "<Gamepad>/rightTrigger";
    private string d_reset_PS = "<Gamepad>/buttonNorth";

    //== XBOX ==
    private string d_jump_XBOX = "<Gamepad>/buttonSouth";

    private string d_parry_XBOX = "<Gamepad>/rightTrigger";
    private string d_reset_XBOX = "<Gamepad>/buttonNorth";

    private bool d_supportMode = false;
    private bool d_Boss01_Dialogeu = false;
    private bool d_Boss02_Dialogeu = false;
    private bool d_Boss03_Dialogeu = false;
    private bool d_Boss04_Cutscene = false;

    #endregion InitialzeData

    private string path;
    private string fileName = "save";

    private void Awake()
    {
        #region 싱글톤

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

        #endregion 싱글톤

        path = Application.persistentDataPath + "/";

        if (!File.Exists(path + fileName))
        {
            nowData.languageIndex = d_languageIndex;
            InitializeData();
            Initialize_Keybinding();
            Debug.Log("파일 생성");
        }

        OnEnable();
        LoadData();
    }

    private void Update()
    {
        SetLanguage();
        //if (Input.GetKeyDown(KeyCode.F10)) Initialize_Developer(); // 초기화 버튼에 할당 > 키보드 세팅에서는 제거
    }

    public void SaveData()
    {
        string data = JsonUtility.ToJson(nowData);
        File.WriteAllText(path + fileName, data);
        //Debug.Log(path + fileName);
        //Debug.Log(data);
    }

    public void LoadData()
    {
        string data = File.ReadAllText(path + fileName);
        nowData = JsonUtility.FromJson<GameData>(data);
        Debug.Log("데이터 불러오기");
    }

    public void InitializeData()
    {
        nowData.sceneName = d_sceneName;
        nowData.boss01_Dialogue = d_Boss01_Dialogeu;
        nowData.boss02_Dialogue = d_Boss02_Dialogeu;
        nowData.boss03_Dialogue = d_Boss03_Dialogeu;
        nowData.boss04_Cutscene = d_Boss04_Cutscene;
        nowData.savePos = d_savePos;
        nowData.deathCount = d_deathCount;
        nowData.parryCount = d_parryCount;
        nowData.elapsedTime = d_elapsedTime;
        nowData.resolution = d_resolution;
        nowData.fullScreen = d_fullScreen;
        nowData.masterVol = d_masterVol;
        nowData.masterVolMixer = d_masterVolMixer;
        nowData.musicVol = d_musicVol;
        nowData.musicVolMixer = d_musicVolMixer;
        nowData.sfxVol = d_sfxVol;
        nowData.sfxVolMixer = d_sfxVolMixer;
        nowData.supportMode = d_supportMode;

        SaveData();
    }

    public void Initialize_Keybinding()
    {
        nowData.moveLeft_keyboard = d_moveLeft_keyboard;
        nowData.moveRight_keyboard = d_moveRight_keyboard;
        nowData.lookDown_keyboard = d_lookDown_keyboard;
        nowData.jump_keyboard = d_jump_keyboard;
        nowData.parry_keyboard = d_parry_keyboard;
        nowData.reset_keyboard = d_reset_keyboard;

        nowData.jump_PS = d_jump_PS;
        nowData.parry_PS = d_parry_PS;
        nowData.reset_PS = d_reset_PS;

        nowData.jump_XBOX = d_jump_XBOX;
        nowData.parry_XBOX = d_parry_XBOX;
        nowData.reset_XBOX = d_reset_XBOX;
        SaveData();
    }

    public void Initialize_Developer()
    {
        InitializeData();
        Initialize_Keybinding();
        nowData.sceneName = d_sceneName;
        nowData.boss01_Dialogue = d_Boss01_Dialogeu;
        nowData.boss02_Dialogue = d_Boss02_Dialogeu;
        nowData.boss03_Dialogue = d_Boss03_Dialogeu;
        nowData.boss04_Cutscene = d_Boss04_Cutscene;
        nowData.savePos = d_savePos;
        nowData.languageIndex = d_languageIndex;
    }

    public void OnEnable() //키세팅 불러오기
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);
    }

    private void SetLanguage()
    {
        switch (nowData.languageIndex)
        {
            case 0:
                DialogueManager.SetLanguage("");
                break;

            case 1:
                DialogueManager.SetLanguage("en");
                break;

            case 2:
                DialogueManager.SetLanguage("ja");
                break;

            case 3:
                DialogueManager.SetLanguage("cn");
                break;

            case 4:
                DialogueManager.SetLanguage("fr");
                break;

            case 5:
                DialogueManager.SetLanguage("it");
                break;

            case 6:
                DialogueManager.SetLanguage("es");
                break;

            case 7:
                DialogueManager.SetLanguage("de");
                break;
        }
    }
}