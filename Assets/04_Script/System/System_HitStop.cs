using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_HitStop : MonoBehaviour
{
    public static System_HitStop instance;
    private GameManager gameManager;

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

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private IEnumerator Hitstop()
    {
        gameManager.doHitstop = true;
        Time.timeScale = 0f;

        float timer = 0f;

        while (timer <= 1f)
        {
            yield return null;

            timer += Time.unscaledDeltaTime * 4f;
            Time.timeScale = Mathf.Lerp(0, 1, timer);
        }

        gameManager.doHitstop = false;
    }

    public void StartHitstop()
    {
        StartCoroutine("Hitstop");
    }

    public void StopHitstop()
    {
        StopCoroutine("Hitstop");
    }
}