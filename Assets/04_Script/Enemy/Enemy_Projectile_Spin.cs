using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Projectile_Spin : MonoBehaviour
{
    private Transform trans;
    [SerializeField] private float spinSpeed;

    // Start is called before the first frame update
    void Start()
    {
        trans = gameObject.transform.GetComponent<Transform>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        trans.Rotate(0, 0, -spinSpeed * Time.deltaTime);
    }

    public void Set_SpinSpeed(float speed)
    {
        spinSpeed = speed;
    }
}
