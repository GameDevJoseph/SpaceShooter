using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
   
    [SerializeField] float _laserSpeed = 8f;

    [SerializeField] bool _isEnemyLaser = false;

    void Update()
    {
        if (!_isEnemyLaser)
            MoveUp();
        else
            MoveDown();
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _laserSpeed * Time.deltaTime);

        if (transform.position.y < -8)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);

            Destroy(this.gameObject);
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _laserSpeed * Time.deltaTime);

        if (transform.position.y > 8)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);

            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser() => _isEnemyLaser = true;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && _isEnemyLaser == true)
        {
            Player player = collision.GetComponent<Player>();

            if (player == null)
                return;

            player.Damage();
            Destroy(this.gameObject);
        }
    }
}
