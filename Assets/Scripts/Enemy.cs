using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _enemySpeed = 4f;
    
    void Update()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (transform.position.y < -5.5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 8f, 0f);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player == null)
                return;

            collision.GetComponent<Player>().Damage();
            Destroy(this.gameObject);
        }

        if (collision.CompareTag("Laser"))
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }
}
