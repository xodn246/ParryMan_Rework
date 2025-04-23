using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Addforce : MonoBehaviour
{
    private Rigidbody2D rigid;

    [SerializeField] private Vector2 forceDir;
    [SerializeField] private float spinPower;

    private void Awake()
    {
        rigid = transform.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rigid.AddForce(forceDir, ForceMode2D.Impulse);
        rigid.AddTorque(spinPower, ForceMode2D.Impulse);
    }
}
