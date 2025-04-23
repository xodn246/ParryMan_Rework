using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DronLookPlayer : MonoBehaviour
{
    private GameObject target;

    // Update is called once per frame
    void Update()
    {
        if (target == null) target = GameObject.Find("Player(Clone)").transform.Find("TargetCenter").gameObject; //�׽�Ʈ�� �׽�Ʈ�� �ڵ�

        // �׽�Ʈ�� ���߿� ������
        Vector2 direction = target.transform.position - gameObject.transform.position;
        float angle;
        if (direction.normalized.x <= 0)
        {
            angle = Vector2.Angle(Vector2.up, direction);
            transform.localScale = new(1, 1, 1);
            transform.rotation = Quaternion.Euler(0, 0, angle + 180f);
        }
        else
        {
            angle = Vector2.Angle(Vector2.down, direction);
            transform.localScale = new(-1, 1, 1);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        //Debug.Log("����ü�� �÷��̾� ���̰� : " + cosSeta);
    }
}
