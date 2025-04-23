using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class UI_Epilogue_Manager : MonoBehaviour
{
    private GameManager manager;
    private DataManager dataManager;

    private SceneChanger sceneChanger;

    [SerializeField] private DialogueSystemTrigger epilogueDialogueTrigger;
    [SerializeField] private GameObject epilogueTrigger;
    [SerializeField] private List<GameObject> epilogueImage;

    [Space(10f)]
    [SerializeField] private float startDelay;
    [SerializeField] private float dialogueDelay;

    [Space(10f)]
    [SerializeField] private CanvasGroup fadeCanvas;

    private void Awake()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        sceneChanger = GameObject.Find("SceneChanger").GetComponent<SceneChanger>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(epilogueTrigger);
            manager.Active_Dialogue = true;

            StartCoroutine(StartEpilogue());
        }
    }

    private IEnumerator StartEpilogue()
    {
        yield return new WaitForSeconds(startDelay);
        StartCoroutine(Dark_Canvas_FadeOut());

        yield return new WaitForSeconds(dialogueDelay);
        epilogueDialogueTrigger.OnUse();
    }


    private IEnumerator Dark_Canvas_FadeIn()
    {
        float timer = 0f;

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 2f;
            fadeCanvas.alpha = Mathf.Lerp(0, 1, timer);
        }
    }

    private IEnumerator Dark_Canvas_FadeOut()
    {
        float timer = 0f;

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 2f;
            fadeCanvas.alpha = Mathf.Lerp(1, 0, timer);
        }
    }

    private IEnumerator Epilogue_Image_FadeOut(Image image)
    {
        float timer = 0f;

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 2f;
            image.color = new(1, 1, 1, Mathf.Lerp(1, 0, timer));
        }
    }

    public void ChangeSceneToTitle()
    {
        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(1f);
        sceneChanger.ChangeSceneWith_Function();
    }

    private void Disable_Object(GameObject other)
    {
        other.SetActive(false);
    }

    public void Fadein_Epilogue()
    {
        StartCoroutine(Dark_Canvas_FadeIn());
    }

    public void Fadeout_Epilogue()
    {
        StartCoroutine(Dark_Canvas_FadeOut());
    }

    public void Destroy_Epilogue01()
    {
        Disable_Object(epilogueImage[0]);
    }

    public void Fadeout_Epilogue02()
    {
        StartCoroutine(Epilogue_Image_FadeOut(epilogueImage[1].GetComponent<Image>()));
    }

    private void OnEnable()
    {
        // Make the functions available to Lua: (Replace these lines with your own.)
        Lua.RegisterFunction("Fadein_Epilogue", this, SymbolExtensions.GetMethodInfo(() => Fadein_Epilogue()));
        Lua.RegisterFunction("Fadeout_Epilogue", this, SymbolExtensions.GetMethodInfo(() => Fadeout_Epilogue()));
        Lua.RegisterFunction("Destroy_Epilogue01", this, SymbolExtensions.GetMethodInfo(() => Destroy_Epilogue01()));
        Lua.RegisterFunction("Fadeout_Epilogue02", this, SymbolExtensions.GetMethodInfo(() => Fadeout_Epilogue02()));
    }

    private void OnDisable()
    {
        // Remove the functions from Lua: (Replace these lines with your own.)
        Lua.UnregisterFunction("Fadein_Epilogue");
        Lua.UnregisterFunction("Fadeout_Epilogue");
        Lua.UnregisterFunction("Destroy_Epilogue01");
        Lua.UnregisterFunction("Fadeout_Epilogue02");
    }
}