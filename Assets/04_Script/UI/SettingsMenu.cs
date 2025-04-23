using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private DataManager dataManager;

    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_Dropdown languageDropdown;
    public TMP_Dropdown resolutionDropdown;

    public UnityEngine.UI.Toggle fullScreenToggle;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolution;

    [SerializeField] private List<string> Language = new();

    [Space(10f)]
    [Header("Option Parameter")]
    private int currentRefreshRate;

    private int currentResolutionIndex = 0;
    public int saveResolutionIndex;
    public bool saveFullScreen;
    public bool supportMode;

    private void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();

        resolutions = Screen.resolutions;
        Screen.fullScreen = dataManager.nowData.fullScreen;
        masterSlider.value = dataManager.nowData.masterVol;
        musicSlider.value = dataManager.nowData.musicVol;
        sfxSlider.value = dataManager.nowData.sfxVol;
        languageDropdown.value = dataManager.nowData.languageIndex;
        fullScreenToggle.isOn = dataManager.nowData.fullScreen;

        #region resolution

        filteredResolution = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = (int)Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < resolutions.Length; i++) { Debug.Log(resolutions[i]); }

        for (int i = 0; i < resolutions.Length; i++)
        {
            if ((int)resolutions[i].refreshRateRatio.value == currentRefreshRate && (int)resolutions[i].width <= 1920)
            {
                filteredResolution.Add(resolutions[i]);
            }
        }

        List<string> option = new List<string>();
        for (int i = 0; i < filteredResolution.Count; i++)
        {
            string resolutionOption = filteredResolution[i].width + "x" + filteredResolution[i].height;
            option.Add(resolutionOption);

            if (filteredResolution[i].width == Screen.width && filteredResolution[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(option);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        #endregion resolution
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);

        dataManager.nowData.masterVol = masterSlider.value;
        dataManager.nowData.masterVolMixer = Mathf.Log10(volume) * 20;
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);

        dataManager.nowData.musicVol = musicSlider.value;
        dataManager.nowData.musicVolMixer = Mathf.Log10(volume) * 20;
    }

    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("Effect", Mathf.Log10(volume) * 20);

        dataManager.nowData.sfxVol = sfxSlider.value;
        dataManager.nowData.sfxVolMixer = Mathf.Log10(volume) * 20;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        if (isFullscreen)
            saveFullScreen = true;
        else
            saveFullScreen = false;

        dataManager.nowData.fullScreen = isFullscreen;
    }

    public void SetResoution(int resolutionIndex)
    {
        Resolution resolution = filteredResolution[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, dataManager.nowData.fullScreen);
        dataManager.nowData.resolution = resolutionIndex;
    }

    public void Option_SetLanguage(int languageIndex)
    {
        dataManager.nowData.languageIndex = languageIndex;
    }
}