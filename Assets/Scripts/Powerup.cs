using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{



    [SerializeField] float _speed = 3.0f;
    [SerializeField] int _powerupID;
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -4.5f)
            Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player == null)
                return;

            switch (_powerupID)
            {
                case 0: player.TripleShotActive(); break;
                case 1: player.SpeedBoostActive(); break;
                case 2: Debug.Log("Shield"); break;
                default: break;
            }

            Destroy(this.gameObject);
        }
    }
}
