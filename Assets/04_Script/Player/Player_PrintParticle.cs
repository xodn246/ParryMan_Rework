using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_PrintParticle : MonoBehaviour
{
    [SerializeField] private GameObject parryParticle;
    [SerializeField] private GameObject purpleParticle;

    [SerializeField] private Transform particlePos;

    public void Print_Particle()
    {
        if (transform.GetComponent<Player_Health_Manager>().Get_CurrentHitObcjetTag() == "Enemy_BounceAttack" || transform.GetComponent<Player_Health_Manager>().Get_CurrentHitObcjetTag() == "Enemy_PurpleProjectile") Instantiate(purpleParticle, particlePos.position, Quaternion.identity);
        else Instantiate(parryParticle, particlePos.position, Quaternion.identity);
    }
}
