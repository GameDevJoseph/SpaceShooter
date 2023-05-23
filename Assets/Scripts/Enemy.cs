using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _enemySpeed = 4f;

    Animator _animator;
    Player _player;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_animator == null)
            Debug.LogError("Enemy Animator is Null");

        if (_player == null)
            Debug.LogError("Player is Null");
    }
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

            _animator.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0f;
            collision.GetComponent<Player>().Damage();
            Destroy(this.gameObject, 2.8f);
        }

        if (collision.CompareTag("Laser"))
        {
            _animator.SetTrigger("OnEnemyDeath");
            Destroy(collision.gameObject);
            
            if(_player != null)
                _player.AddToScore(10);

            _enemySpeed = 0;
            Destroy(this.gameObject, 2.8f);
        }
    }
}
