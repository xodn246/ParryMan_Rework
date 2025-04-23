using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elite_Farmer_ThornManager : MonoBehaviour
{
    private Elite_Farmer_Manager farmerManager;
    private int thornPosNum;

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance;

    private void Awake()
    {
        farmerManager = GameObject.FindObjectOfType<Elite_Farmer_Manager>();
    }

    public void Set_ThornPosNum(int posNum)
    {
        thornPosNum = posNum;
    }

    public void Destroy_Thorn()
    {
        farmerManager.Attack01_Destroy_Thorn(thornPosNum);
    }

    public Vector2 CheckGroundPoint()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround).point;
    }
}
