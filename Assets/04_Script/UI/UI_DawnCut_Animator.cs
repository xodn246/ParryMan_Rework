using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class UI_DawnCut_Animator : MonoBehaviour
{
    private Animator backAnim;
    private Animator roseAnim;
    private Animator smashAnim;
    private Animator HAnim;
    private Animator shotAnim;
    private Animator keyAnim;

    private void Awake()
    {
        backAnim = gameObject.transform.Find("BackGround").GetComponent<Animator>();
        roseAnim = gameObject.transform.Find("Dialogu_Rosmary").GetComponent<Animator>();
        smashAnim = gameObject.transform.Find("Dialogu_Smasher").GetComponent<Animator>();
        HAnim = gameObject.transform.Find("Dialogu_H").GetComponent<Animator>();
        shotAnim = gameObject.transform.Find("Dialogu_Shotgun").GetComponent<Animator>();
        keyAnim = gameObject.transform.Find("Dialogu_Key").GetComponent<Animator>();
    }

    public void SetBackGround_On()
    {
        backAnim.SetTrigger("isIntro");
    }
    public void SetBackGround_Off()
    {
        backAnim.SetTrigger("isOuttro");
    }


    public void SetRose_On()
    {
        roseAnim.SetTrigger("isGetin");
    }
    public void SetRose_Off()
    {
        roseAnim.SetTrigger("isGetout");
    }


    public void SetSmash_On()
    {
        smashAnim.SetTrigger("isGetin");
    }
    public void SetSmash_Off()
    {
        smashAnim.SetTrigger("isGetout");
    }


    public void SetH_On()
    {
        HAnim.SetTrigger("isGetin");
    }
    public void SetH_Off()
    {
        HAnim.SetTrigger("isGetout");
    }


    public void SetShot_On()
    {
        shotAnim.SetTrigger("isGetin");
    }
    public void SetShot_Off()
    {
        shotAnim.SetTrigger("isGetout");
    }


    public void SetKey_On()
    {
        keyAnim.SetTrigger("isGetin");
    }
    public void SetKey_Off()
    {
        keyAnim.SetTrigger("isGetout");
    }


    void OnEnable()
    {
        // Make the functions available to Lua: (Replace these lines with your own.)
        Lua.RegisterFunction("SetBackGround_On", this, SymbolExtensions.GetMethodInfo(() => SetBackGround_On()));
        Lua.RegisterFunction("SetBackGround_Off", this, SymbolExtensions.GetMethodInfo(() => SetBackGround_Off()));

        Lua.RegisterFunction("SetRose_On", this, SymbolExtensions.GetMethodInfo(() => SetRose_On()));
        Lua.RegisterFunction("SetRose_Off", this, SymbolExtensions.GetMethodInfo(() => SetRose_Off()));

        Lua.RegisterFunction("SetSmash_On", this, SymbolExtensions.GetMethodInfo(() => SetSmash_On()));
        Lua.RegisterFunction("SetSmash_Off", this, SymbolExtensions.GetMethodInfo(() => SetSmash_Off()));

        Lua.RegisterFunction("SetH_On", this, SymbolExtensions.GetMethodInfo(() => SetH_On()));
        Lua.RegisterFunction("SetH_Off", this, SymbolExtensions.GetMethodInfo(() => SetH_Off()));

        Lua.RegisterFunction("SetShot_On", this, SymbolExtensions.GetMethodInfo(() => SetShot_On()));
        Lua.RegisterFunction("SetShot_Off", this, SymbolExtensions.GetMethodInfo(() => SetShot_Off()));

        Lua.RegisterFunction("SetKey_On", this, SymbolExtensions.GetMethodInfo(() => SetKey_On()));
        Lua.RegisterFunction("SetKey_Off", this, SymbolExtensions.GetMethodInfo(() => SetKey_Off()));
    }

    void OnDisable()
    {
        // Remove the functions from Lua: (Replace these lines with your own.)
        Lua.UnregisterFunction("SetBackGround_On");
        Lua.UnregisterFunction("SetBackGround_Off");

        Lua.UnregisterFunction("SetRose_On");
        Lua.UnregisterFunction("SetRose_Off");

        Lua.UnregisterFunction("SetSmash_On");
        Lua.UnregisterFunction("SetSmash_Off");

        Lua.UnregisterFunction("SetH_On");
        Lua.UnregisterFunction("SetH_Off");

        Lua.UnregisterFunction("SetShot_On");
        Lua.UnregisterFunction("SetShot_Off");

        Lua.UnregisterFunction("SetKey_On");
        Lua.UnregisterFunction("SetKey_Off");
    }
}
