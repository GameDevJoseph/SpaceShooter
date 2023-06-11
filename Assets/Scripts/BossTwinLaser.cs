using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTwinLaser : MonoBehaviour
{
    Player player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            player = collision.GetComponent<Player>();
            player.Damage();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<Player>();
            StartCoroutine(DamageOverTime());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();
        }
    }
    IEnumerator DamageOverTime()
    {
        yield return new WaitForSeconds(3f);
        if (player != null)
            player.Damage();

        StopAllCoroutines();
    }
}
