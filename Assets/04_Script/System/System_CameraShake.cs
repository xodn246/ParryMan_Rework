using System.Collections;
using Cinemachine;
using UnityEngine;

public class System_CameraShake : MonoBehaviour
{
    public static System_CameraShake instance;
    private CinemachineVirtualCamera virtualCam;

    private float shakeTime = 0;
    private float shakeTimeTotal;
    private float shakeIntensity = 0;

    private IEnumerator shakeCoroutine;

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

    private void Update()
    {
        if (virtualCam == null)
            virtualCam = GameObject.FindObjectOfType<Player_Health_Manager>().GetComponent<Player_Health_Manager>().Get_CurrentCam();
        else if (virtualCam.name != GameObject.FindObjectOfType<Player_Health_Manager>().GetComponent<Player_Health_Manager>().Get_CurrentCam().name)
            virtualCam = GameObject.FindObjectOfType<Player_Health_Manager>().GetComponent<Player_Health_Manager>().Get_CurrentCam();
    }

    private IEnumerator CameraShake(float intensity, float time)
    {
        Debug.Log("ƒ´∏ﬁ∂Û Ω¶¿Ã≈©");
        CinemachineBasicMultiChannelPerlin shake = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        shake.m_AmplitudeGain = intensity;

        yield return new WaitForSeconds(time);

        while (shakeTime > 0)
        {
            yield return null;
            shakeTime -= Time.deltaTime;
            shake.m_AmplitudeGain = Mathf.Lerp(shakeIntensity, 0, 1 - (shakeTime / shakeTimeTotal));
        }
    }

    public void Start_Shake_Camera(float intensity, float time)
    {
        shakeIntensity = intensity;
        shakeTime = time;
        shakeTimeTotal = time;
        StartCoroutine(CameraShake(intensity, time));
    }
}
