using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;
    public AudioMixerGroup mixer;

    [Range(.1f, 3f)]
    public float pitch = 1f;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;


    private DataManager dataManager;
    private GameObject soundManagerCheck;

    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;

    [Space(10f)]

    public AudioMixerGroup groupSFX;

    [SerializeField] private Sound[] sounds;

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

        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.mixer;

            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        masterSlider = GameObject.Find("UI_Pause").transform.Find("UI_OptionMenu").transform.Find("optionWin").transform.Find("Slider_master").transform.GetComponent<Slider>();
        BGMSlider = GameObject.Find("UI_Pause").transform.Find("UI_OptionMenu").transform.Find("optionWin").transform.Find("Slider_music").transform.GetComponent<Slider>();
        SFXSlider = GameObject.Find("UI_Pause").transform.Find("UI_OptionMenu").transform.Find("optionWin").transform.Find("Slider_sfx").transform.GetComponent<Slider>();

        Play("Theme");
        Play("Atmo");
    }

    private void Start()
    {
        mainMixer.SetFloat("Master", dataManager.nowData.masterVolMixer);
        mainMixer.SetFloat("BGM", dataManager.nowData.musicVolMixer);
        mainMixer.SetFloat("Effect", dataManager.nowData.sfxVolMixer);
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Play();
    }

    public void SFXPlayer(/*string sfxNmae,*/ AudioClip clip, Transform transform)
    {
        GameObject sfx = new GameObject("SFX_Sound");
        AudioSource audioSource = sfx.AddComponent<AudioSource>();
        sfx.transform.SetParent(transform);
        sfx.transform.position = transform.position;
        audioSource.outputAudioMixerGroup = groupSFX;
        audioSource.clip = clip;

        audioSource.spatialBlend = 1;
        audioSource.dopplerLevel = 0;
        audioSource.spread = 180;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 10;
        audioSource.maxDistance = 40;

        audioSource.Play();

        Destroy(sfx, clip.length);
    }

    public void SFXPlayer_Far(/*string sfxNmae,*/ AudioClip clip, Transform transform)
    {
        GameObject sfx = new GameObject("SFX_Sound");
        AudioSource audioSource = sfx.AddComponent<AudioSource>();
        sfx.transform.SetParent(transform);
        sfx.transform.position = transform.position;
        audioSource.outputAudioMixerGroup = groupSFX;
        audioSource.clip = clip;

        audioSource.spatialBlend = 1;
        audioSource.dopplerLevel = 0;
        audioSource.spread = 180;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 5;
        audioSource.maxDistance = 60;

        audioSource.Play();

        Destroy(sfx, clip.length);
    }

    public void SFXPlayer_UI(/*string sfxNmae,*/ AudioClip clip, Transform transform)
    {
        GameObject sfx = new GameObject("SFX_Sound");
        AudioSource audioSource = sfx.AddComponent<AudioSource>();
        sfx.transform.SetParent(transform);
        sfx.transform.position = transform.position;
        audioSource.outputAudioMixerGroup = groupSFX;
        audioSource.clip = clip;

        audioSource.Play();

        Destroy(sfx, clip.length);
    }

    public void SFXPlayer_DestroySetTime(/*string sfxNmae,*/ AudioClip clip, Transform transform, float destroyTime)
    {
        GameObject sfx = new GameObject("SFX_Sound");
        AudioSource audioSource = sfx.AddComponent<AudioSource>();
        sfx.transform.SetParent(transform);
        sfx.transform.position = transform.position;
        audioSource.outputAudioMixerGroup = groupSFX;
        audioSource.clip = clip;

        audioSource.loop = true;
        audioSource.spatialBlend = 1;
        audioSource.dopplerLevel = 0;
        audioSource.spread = 180;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 10;
        audioSource.maxDistance = 40;

        audioSource.Play();

        Destroy(sfx, destroyTime);
    }

    public void SFXPlayer_Loop(/*string sfxNmae,*/ AudioClip clip, Transform transform)
    {
        GameObject sfx = new GameObject("SFX_Sound");
        AudioSource audioSource = sfx.AddComponent<AudioSource>();
        sfx.transform.SetParent(transform);
        sfx.transform.position = transform.position;
        audioSource.outputAudioMixerGroup = groupSFX;
        audioSource.clip = clip;

        audioSource.loop = true;
        audioSource.spatialBlend = 1;
        audioSource.dopplerLevel = 0;
        audioSource.spread = 180;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 10;
        audioSource.maxDistance = 40;

        audioSource.Play();
    }
}