using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_SoundManager : MonoBehaviour
{
    [SerializeField] private SerializableDictionary<string, AudioClip> soundClipDictionary = new();

    [Space(15f)]
    [SerializeField] private int max_randRnage;

    public AudioClip Get_AudioClip(string SoundKey)
    {
        return soundClipDictionary[SoundKey];
    }

    // 애니메이션에서 사운드 호출할때 사용하는 함수
    public void Get_AudioClip_Animation(string SoundKey)
    {
        SoundManager.instance.SFXPlayer(/*"Player_Animation",*/ soundClipDictionary[SoundKey], gameObject.transform);
    }

    public void Get_AudioClip_Animation_Far(string SoundKey)
    {
        SoundManager.instance.SFXPlayer_Far(/*"Player_Animation",*/ soundClipDictionary[SoundKey], gameObject.transform);
    }

    public void Get_AudioClip_Animation_UI(string SoundKey)
    {
        SoundManager.instance.SFXPlayer_UI(/*"Player_Animation",*/ soundClipDictionary[SoundKey], gameObject.transform);
    }


    //랜덤사운드 애니메이션에서 호출할때 사용하는 함수
    public void Get_Random_AudioClip_Animation(string SoundKey)
    {
        int rand = Random.Range(0, max_randRnage);
        SoundManager.instance.SFXPlayer(soundClipDictionary[SoundKey + rand.ToString()], gameObject.transform);
    }
}