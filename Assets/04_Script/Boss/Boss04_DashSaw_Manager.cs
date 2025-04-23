using UnityEngine;

public class Boss04_DashSaw_Manager : MonoBehaviour
{
    [SerializeField] private Object_SoundManager soundManager;

    void Awake()
    {
        SoundManager.instance.SFXPlayer_DestroySetTime(soundManager.Get_AudioClip("Saw"), gameObject.transform, 5f);
    }
}
