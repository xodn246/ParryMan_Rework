using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_SetDrag : MonoBehaviour
{

    private Rigidbody2D rigid;

    private float defaultDrag;

    private void Start()
    {
        rigid = transform.GetComponent<Rigidbody2D>();
        defaultDrag = rigid.drag;
    }

    public void Set_Drag(float dragScale)
    {
        rigid.drag = dragScale;
    }

    public void Reset_Drag()
    {
        rigid.drag = defaultDrag;
    }
}
