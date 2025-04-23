using System.Collections.Generic;
using UnityEngine;

public class Enemy_Projectile_Manager : MonoBehaviour
{
    private GameObject target;
    private Transform TF;
    [SerializeField] private List<GameObject> projectile;
    [SerializeField] private Transform projectilePos;

    [SerializeField] private bool instanceToPlayer;

    [SerializeField] private bool lookPlayer;


    private void Start()
    {
        //target = GameObject.Find("Player(Clone)").gameObject;
        TF = gameObject.transform.GetComponent<Transform>();

    }

    private void Update()
    {
        if (target == null) target = GameObject.Find("Player(Clone)").transform.Find("TargetCenter").gameObject; //�׽�Ʈ�� �׽�Ʈ�� �ڵ�
        if (instanceToPlayer) projectilePos = target.transform;
    }

    public void Instance_Projectile(int projectileNum)
    {
        GameObject projectile = Instantiate(this.projectile[projectileNum], projectilePos.position, Quaternion.identity);

        projectile.GetComponent<Enemy_projectile_Hitbox>().Set_Parent_Transform(TF); // ����ü ���� �θ��� transform �� ����ü�� ���� (����ü �и� ����Ʈ ��¿�)
    }

    public void Instance_Projectile_Back(int projectileNum)
    {
        GameObject projectile;
        if (lookPlayer)
        {
            Vector2 direction = target.transform.position - gameObject.transform.position;
            if (transform.position.x > target.transform.position.x)
            {
                if (transform.position.y > target.transform.position.y) projectile = Instantiate(this.projectile[projectileNum], projectilePos.position, Quaternion.Euler(0, 0, 180 - Vector2.Angle(Vector2.right, direction)));
                else projectile = Instantiate(this.projectile[projectileNum], projectilePos.position, Quaternion.Euler(0, 0, 180 + Vector2.Angle(Vector2.right, direction)));
            }
            else
            {
                if (transform.position.y > target.transform.position.y) projectile = Instantiate(this.projectile[projectileNum], projectilePos.position, Quaternion.Euler(0, 0, 180 + Vector2.Angle(Vector2.left, direction)));
                else projectile = Instantiate(this.projectile[projectileNum], projectilePos.position, Quaternion.Euler(0, 0, 180 - Vector2.Angle(Vector2.left, direction)));
            }
        }
        else
        {
            projectile = Instantiate(this.projectile[projectileNum], projectilePos.position, Quaternion.identity);
        }
        projectile.transform.localScale = new(-TF.localScale.x, TF.localScale.y, TF.localScale.z);
        projectile.GetComponent<Enemy_projectile_Hitbox>().Set_Parent_Transform(TF); // ����ü ���� �θ��� transform �� ����ü�� ���� (����ü �и� ����Ʈ ��¿�)
    }
}
