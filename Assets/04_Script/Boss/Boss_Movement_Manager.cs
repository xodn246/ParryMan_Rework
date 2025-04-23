using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Movement_Manager : MonoBehaviour
{
    private Rigidbody2D rigid;

    private Transform target;


    [SerializeField] float initialAngle;
    [SerializeField] private List<Vector2> moveDir = new();

    [Space(5f)]
    [SerializeField] private List<Vector2> rejectionDir = new();

    private void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (target == null) target = GameObject.Find("Player(Clone)").GetComponent<Transform>();
    }

    public void AttackMovement(int dirNum)
    {
        Vector2 result;
        rigid.velocity = Vector2.zero;

        if (rigid.transform.localScale.x == 1) result = moveDir[dirNum];
        else result = new(-moveDir[dirNum].x, moveDir[dirNum].y);

        rigid.AddForce(result, ForceMode2D.Impulse);
    }

    public void AtteckRejection(int reNum)
    {
        Vector2 result;

        if (rigid.transform.localScale.x == -1) result = rejectionDir[reNum];
        else result = new(-rejectionDir[reNum].x, rejectionDir[reNum].y);

        rigid.AddForce(result, ForceMode2D.Impulse);
    }

    public void Movement_sample()
    {
        Vector3 p = target.transform.position;

        float gravity = Physics.gravity.magnitude;
        // Selected angle in radians
        float angle = initialAngle * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(p.x, 0, p.z);
        Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);

        // Distance along the y axis between objects
        float yOffset = transform.position.y - p.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        // Fire!
        //rigid.velocity = finalVelocity;

        // Alternative way:
        rigid.AddForce(finalVelocity * rigid.mass * rigid.gravityScale, ForceMode2D.Impulse);
    }
}