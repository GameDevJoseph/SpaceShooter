using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineExplosionRadius : MonoBehaviour
{
    void Start()
    {
        Destroy(this.gameObject, 1.5f);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().MineExplosion();
        }

        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().Damage();
        }

        if (collision.CompareTag("Boss"))
        {
            collision.GetComponent<Boss>().Damage(5);
        }
    }

    
}
