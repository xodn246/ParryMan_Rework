using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectWithAnimator : MonoBehaviour
{
    public void Destory_Object_Trigger()
    {
        Destroy(gameObject);
    }

    public void Destory_Parent_Trigger()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
