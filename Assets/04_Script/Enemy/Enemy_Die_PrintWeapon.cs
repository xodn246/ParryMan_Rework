using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Die_PrintWeapon : MonoBehaviour
{
    [SerializeField] private GameObject enemyWeaponProjectile;
    [SerializeField] private Transform printPos;

    private Vector2 moveDir;
    private float spinPower;

    private void Start()
    {
        moveDir = new(Random.Range(15f, 20f), Random.Range(20f, 35f));
        spinPower = Random.Range(7f, 15f);
    }

    public void Print_Die_Weapon()
    {
        Vector2 resultDir;
        float resultTorque;

        GameObject weapon = Instantiate(enemyWeaponProjectile, printPos.position, Quaternion.identity);

        if (transform.localScale.x == 1)
        {
            resultDir = new(-moveDir.x, moveDir.y);
            resultTorque = spinPower;
        }
        else
        {
            resultDir = moveDir;
            resultTorque = -spinPower;
        }

        weapon.GetComponent<Rigidbody2D>().AddForce(resultDir, ForceMode2D.Impulse);
        weapon.GetComponent<Rigidbody2D>().AddTorque(resultTorque, ForceMode2D.Impulse);
    }
}
