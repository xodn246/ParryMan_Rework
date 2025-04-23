using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Boss_PrintVFX : MonoBehaviour
{
    [SerializeField] private List<Transform> VFXPos;
    [SerializeField] private List<GameObject> VFX = new();
    [SerializeField] private List<float> size = new();

    public void Print_VFX(int VFX_Num)
    {
        GameObject attackVFX = Instantiate(VFX[VFX_Num], VFXPos[VFX_Num].position, quaternion.identity);
        if (transform.localScale.x == -1)
        {
            attackVFX.transform.localScale = new(1 * size[VFX_Num], 1 * size[VFX_Num], 1);
        }
        else
        {
            attackVFX.transform.localScale = new(-1 * size[VFX_Num], 1 * size[VFX_Num], 1);
        }
    }

    public void Print_VFX_Back(int VFX_Num)
    {
        GameObject attackVFX = Instantiate(VFX[VFX_Num], VFXPos[VFX_Num].position, quaternion.identity);
        if (transform.localScale.x == -1)
        {
            attackVFX.transform.localScale = new(-1 * size[VFX_Num], 1 * size[VFX_Num], 1);
        }
        else
        {
            attackVFX.transform.localScale = new(1 * size[VFX_Num], 1 * size[VFX_Num], 1);
        }
    }

    public void Print_Projectile(int VFX_Num)
    {
        GameObject attackVFX = Instantiate(VFX[VFX_Num], VFXPos[VFX_Num].position, quaternion.identity);
        attackVFX.GetComponent<Enemy_projectile_Hitbox>().Set_Parent_Transform(transform);
        if (transform.localScale.x == -1)
        {
            attackVFX.transform.localScale = new(1 * size[VFX_Num], 1 * size[VFX_Num], 1);
        }
        else
        {
            attackVFX.transform.localScale = new(-1 * size[VFX_Num], 1 * size[VFX_Num], 1);
        }
    }
}
