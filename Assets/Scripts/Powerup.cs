using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] float _speed = 3.0f;
    [SerializeField] int _powerupID;
    [SerializeField] AudioClip _powerupAudio;
    
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
            AudioSource.PlayClipAtPoint(_powerupAudio, transform.position);

            if (player == null)
                return;

            switch (_powerupID)
            {
                case 0: player.TripleShotActive();  break;
                case 1: player.SpeedBoostActive(); break;
                case 2: player.ShieldsActive(); break;
                case 3: player.AddAmmo(); break;
                case 4: player.LifeUp(); break;
                case 5: player.MinesActive(); break;
                case 6: player.EMP(); break;
                default: break;
            }
            Destroy(this.gameObject);
        }
    }
}
