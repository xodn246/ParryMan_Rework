using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    private GameManager manager;
    private DataManager dataManager;
    private SoundManager soundManager;
    [SerializeField] private string sceneName;
    [SerializeField] private bool destroyBGM;
    [SerializeField] private bool LoadNoOptionScene;

    [Space(10f)]
    [SerializeField] private bool saveNextScene;
    [SerializeField] private bool neverSaveScene;

    [SerializeField] private string saveAnotherScene;
    [SerializeField] private bool isButton;

    [Space(10f)]
    [SerializeField] private bool isLogo;
    private bool loadOneTime = false;
    private float verticalValue;

    [Space(10f)]
    [SerializeField] private bool isStartButton;

    private void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        verticalValue = Input.GetAxisRaw("Vertical");

        if (collider.CompareTag("Player"))
        {
            if (!loadOneTime && verticalValue == 1)
            {
                loadOneTime = true;
                SceneLoader.Instance.LoadScene(sceneName);


                if (destroyBGM) Destroy(soundManager.gameObject);

                if (LoadNoOptionScene) manager.noOptionScene = true;
                else manager.noOptionScene = false;

                dataManager.nowData.savePos = 0;
                dataManager.SaveData();

                if (neverSaveScene) return;
                else if (saveNextScene) dataManager.nowData.sceneName = sceneName;
                else dataManager.nowData.sceneName = saveAnotherScene;
                dataManager.SaveData();
            }
        }
    }

    public void ChangeSceneWith_Function()
    {
        if (isButton) transform.GetComponent<Button>().interactable = false;

        if (!isLogo) dataManager.nowData.savePos = 0;
        SceneLoader.Instance.LoadScene(sceneName);

        if (isStartButton)
        {
            dataManager.nowData.boss01_Dialogue = false;
            dataManager.nowData.boss02_Dialogue = false;
            dataManager.nowData.boss03_Dialogue = false;
        }

        if (destroyBGM) Destroy(soundManager.gameObject);

        if (LoadNoOptionScene) manager.noOptionScene = true;
        else manager.noOptionScene = false;

        if (manager.Active_Dialogue) manager.Active_Dialogue = false;
        dataManager.SaveData();

        if (neverSaveScene) return;
        else if (saveNextScene) dataManager.nowData.sceneName = sceneName;
        else dataManager.nowData.sceneName = saveAnotherScene;
        dataManager.SaveData();
    }

    public void ChengeSceneToSave()
    {
        if (isButton) transform.GetComponent<Button>().interactable = false;
        SceneLoader.Instance.LoadScene(dataManager.nowData.sceneName);

        if (destroyBGM) Destroy(soundManager.gameObject);

        if (LoadNoOptionScene) manager.noOptionScene = true;
        else manager.noOptionScene = false;
    }

    private void OnEnable()
    {
        // Make the functions available to Lua: (Replace these lines with your own.)
        Lua.RegisterFunction("ChangeSceneWith_Function", this, SymbolExtensions.GetMethodInfo(() => ChangeSceneWith_Function()));
    }

    private void OnDisable()
    {
        // Remove the functions from Lua: (Replace these lines with your own.)
        Lua.UnregisterFunction("ChangeSceneWith_Function");
    }
}