using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Destroy_VFX : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    private float destroyTimer;

    private void Start()
    {
        destroyTimer = destroyTime;
    }
    private void Update()
    {
        destroyTimer -= Time.deltaTime;
        if (destroyTimer <= 0) Destroy(gameObject);
    }
}
