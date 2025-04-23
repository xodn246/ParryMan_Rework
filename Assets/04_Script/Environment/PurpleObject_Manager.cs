using UnityEngine;

public class PurpleObject_Manager : MonoBehaviour
{
    [SerializeField] private Vector2 bounceDir;
    [SerializeField] private bool isCrystal;
    [SerializeField] private bool isBoss;

    [SerializeField] private bool canDamage;
    [SerializeField] private int damage;

    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.name == "Player_Hurt")
        {
            if (canDamage)
            {
                if (isCrystal) player.GetComponentInParent<Player_Manager>().Set_Crystal_True();
                if (!isBoss) player.GetComponentInParent<Player_Health_Manager>().Set_BounceDir(bounceDir);
                else
                {
                    if (transform.parent.transform.localScale.x == -1)
                    {
                        player.GetComponentInParent<Player_Health_Manager>().Set_BounceDir(new(bounceDir.x, bounceDir.y));
                    }
                    else
                    {
                        player.GetComponentInParent<Player_Health_Manager>().Set_BounceDir(new(-bounceDir.x, bounceDir.y));
                    }
                }
                player.GetComponentInParent<Player_Health_Manager>().Player_TakeDamage(transform, damage, tag);
            }
            else
            {
                if (player.GetComponentInParent<Player_Manager>().readyParry)
                {
                    if (isCrystal) player.GetComponentInParent<Player_Manager>().Set_Crystal_True();
                    player.GetComponentInParent<Player_Health_Manager>().Set_BounceDir(bounceDir);
                    player.GetComponentInParent<Player_Health_Manager>().Player_TakeDamage(transform, 0, tag);
                }
            }
        }
    }
}
