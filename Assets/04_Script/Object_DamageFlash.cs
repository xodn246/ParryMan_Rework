using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_DamageFlash : MonoBehaviour
{
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashTime = 0.25f;

    private SpriteRenderer spriteRenderer;
    private Material material;

    private Coroutine damageFlashCoroutine;

    private void Awake()
    {
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }

    public void Call_DamageFlash()
    {
        damageFlashCoroutine = StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        Set_FlashColor();

        float currentFlashAmount = 0f;
        float elapsetime = 0f;

        while (elapsetime < flashTime)
        {
            elapsetime += Time.deltaTime;

            currentFlashAmount = Mathf.Lerp(1f, 0f, elapsetime / flashTime);
            Set_FlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    private void Set_FlashColor()
    {
        material.SetColor("_Color", flashColor);

    }


    private void Set_FlashAmount(float amount)
    {
        material.SetFloat("_Amount", amount);
    }
}
