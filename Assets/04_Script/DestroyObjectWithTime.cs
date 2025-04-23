using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectWithTime : MonoBehaviour
{
    [SerializeField] private float destroyTime;

    private void Start()
    {
        Invoke("Destroy_Object_WithTime", destroyTime);
    }

    private void Destroy_Object_WithTime()
    {
        Destroy(gameObject);
    }
}
