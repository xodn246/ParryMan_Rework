using System.Collections;
using UnityEngine;

public class PurpleSaw_Manager : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private float stopTime;

    private void Awake()
    {
        anim = transform.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D player)
    {
        float angle = Random.Range(0, 360);
        if (player.CompareTag("PlayerParry"))
        {
            transform.rotation = Quaternion.Euler(0, 0, angle);
            anim.SetBool("isActive", false);
            StartCoroutine(Saw_Stop());
        }

    }

    private IEnumerator Saw_Stop()
    {
        yield return new WaitForSeconds(stopTime);
        anim.SetBool("isActive", true);
    }
}
