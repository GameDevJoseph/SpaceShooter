using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
   
    [SerializeField] float _laserSpeed = 8f;
    [SerializeField] bool _isEnemyLaser = false;
    [SerializeField] bool _isChargedLaser = false;
    [SerializeField] bool _isEnemyBackShot = false;
    

    Player _player;
    

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
            Debug.LogError("Player is null");
    }

    void Update()
    {
        if (_player == null)
            return;

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
    public void AssignEnemyBackShotLaser() => _isEnemyBackShot = true;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && _isEnemyLaser || collision.CompareTag("Player") && _isEnemyBackShot)
        {
            Player player = collision.GetComponent<Player>();

            if (player == null)
                return;

            player.Damage();

            if (_isChargedLaser)
                this.gameObject.SetActive(false);
            else
            {
                Destroy(this.gameObject);
            }
            
        }

        if(collision.CompareTag("Powerups"))
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }
}
