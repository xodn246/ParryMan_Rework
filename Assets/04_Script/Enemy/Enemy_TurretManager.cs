using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_TurretManager : MonoBehaviour
{
    private enum Turret_Type
    {
        Normal,
        Laser
    }

    [SerializeField] private Object_SoundManager soundManager;
    [SerializeField] private Turret_Type turret_Type;


    [Space(10f)]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform laserWallCheck;
    [SerializeField] private float wallCheckDistance;


    [Space(10f)]
    [SerializeField] Transform projectilePos;
    [SerializeField] Transform muzzlePos;

    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shotDelay;
    private float shotTimer;

    [SerializeField] private SerializableDictionary<string, GameObject> projectilePrefab;



    private void Update()
    {
        shotTimer -= Time.deltaTime;

        if (shotTimer <= 0) Print_Turret_Projectile();

    }

    private Vector2 LaserWallCheck_Left()
    {
        return Physics2D.Raycast(laserWallCheck.position, transform.TransformDirection(Vector3.left), wallCheckDistance, whatIsGround).point;
    }

    private Vector2 LaserWallCheck_Right()
    {
        return Physics2D.Raycast(laserWallCheck.position, transform.TransformDirection(Vector3.right), wallCheckDistance, whatIsGround).point;
    }


    public void Print_Turret_Projectile()
    {
        if (turret_Type == Turret_Type.Normal)
        {
            GameObject normal = Instantiate(projectilePrefab["Normal"], projectilePos.position, Quaternion.identity);
            GameObject muzzle = Instantiate(projectilePrefab["Normal_Muzzle"], projectilePos.position, Quaternion.identity);

            if (transform.localScale.x > 0) normal.GetComponent<SpriteRenderer>().size = new(Vector2.Distance(projectilePos.position, LaserWallCheck_Left()), 0.6f);
            else normal.GetComponent<SpriteRenderer>().size = new(Vector2.Distance(projectilePos.position, LaserWallCheck_Right()), 0.6f);

            normal.transform.rotation = transform.rotation;
            normal.transform.localScale = transform.localScale;
            normal.GetComponent<Enemy_projectile>().Set_BulletSpeed(bulletSpeed);
            normal.GetComponent<Enemy_projectile_Hitbox>().Set_Parent_Transform(transform);

            muzzle.transform.rotation = transform.rotation;
            muzzle.transform.localScale = transform.localScale;

            SoundManager.instance.SFXPlayer(soundManager.Get_AudioClip("NormalShot"), gameObject.transform);
            shotTimer = shotDelay;
        }

        else if (turret_Type == Turret_Type.Laser)
        {
            GameObject laser = Instantiate(projectilePrefab["Laser"], projectilePos.position, Quaternion.identity);
            GameObject muzzle = Instantiate(projectilePrefab["Laser_Muzzle"], projectilePos.position, Quaternion.identity);
            Instantiate(projectilePrefab["Laser_End"], new(LaserWallCheck_Left().x, LaserWallCheck_Left().y), Quaternion.identity);

            if (transform.localScale.x > 0) laser.GetComponent<SpriteRenderer>().size = new(Vector2.Distance(projectilePos.position, LaserWallCheck_Left()), 0.6f);
            else laser.GetComponent<SpriteRenderer>().size = new(Vector2.Distance(projectilePos.position, LaserWallCheck_Right()), 0.6f);

            laser.transform.rotation = transform.rotation;
            laser.transform.localScale = new(1, 1, 1);

            muzzle.transform.rotation = transform.rotation;
            muzzle.transform.localScale = transform.localScale;

            SoundManager.instance.SFXPlayer(soundManager.Get_AudioClip("LaserShot"), gameObject.transform);
            shotTimer = shotDelay;
        }
    }
}
