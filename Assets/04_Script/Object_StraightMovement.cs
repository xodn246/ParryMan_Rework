using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_StraightMovement : MonoBehaviour
{
    [SerializeField] private Transform checkPoint01;
    [SerializeField] private Transform checkPoint02;

    [Space(10f)]
    [SerializeField] private float moveSpeed;
    private float currentSpeed;
    private Vector2 moveDir01;
    private Vector2 moveDir02;

    // Start is called before the first frame update
    void Start()
    {
        moveDir01 = checkPoint01.position - checkPoint02.position;
        moveDir02 = checkPoint02.position - checkPoint01.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
