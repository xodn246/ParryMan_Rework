using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Projecteil_Lifetime : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private float lifeTime;

    private void Start()
    {
        anim = gameObject.transform.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            anim.SetTrigger("destroy");
        }
    }
}
