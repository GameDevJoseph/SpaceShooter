using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _enemySpeed = 4f;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _explosionAudio;
    [SerializeField] GameObject _laserPrefab;
    Animator _animator;
    Player _player;
    float _fireRate = 3.0f;
    float _canFire = -1f;
    bool _canFireLasers = false;

    [SerializeField] int _enemyID;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _canFireLasers = true;
        if (_animator == null)
            Debug.LogError("Enemy Animator is Null");

        if (_player == null)
            Debug.LogError("Player is Null");

        if (_audioSource == null)
            Debug.LogError("Audio Source is null");
        else
            _audioSource.clip = _explosionAudio;
    }
    void Update()
    {
        CalculateMovement();

        if(Time.time > _canFire)
        {
            
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            if (!_canFireLasers)
                return;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            
            for(int i = 0; i < lasers.Length; i++ )
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    private void CalculateMovement()
    {
        switch(_enemyID)
        {
            case 0: NormalShipBehavior();
                break;
            case 1:
                BlockadeShipBehavior();
                break;
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
            _canFireLasers = false;
            this.GetComponent<Collider2D>().enabled = false;
            _audioSource.Play();
            collision.GetComponent<Player>().Damage();
            Destroy(this.gameObject, 2.8f);
        }

        if (collision.CompareTag("Laser"))
        {
            _animator.SetTrigger("OnEnemyDeath");
            _audioSource.Play();
            Destroy(collision.gameObject);
            
            if(_player != null)
                _player.AddToScore(10);

            _enemySpeed = 0;
            _canFireLasers = false;
            this.GetComponent<Collider2D>().enabled = false;
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
    }

    public void MineExplosion()
    {
        _animator.SetTrigger("OnEnemyDeath");
        _enemySpeed = 0;
        _audioSource.Play();
        Destroy(this.gameObject, 2.8f);
    }

    void NormalShipBehavior()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        if (transform.position.y < -5.5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 8f, 0f);
        }
    }

    void BlockadeShipBehavior()
    {
        _canFireLasers = false;
        if (_player != null)
        {
            transform.Translate(Vector2.down * _enemySpeed * Time.deltaTime);
            Vector2 distance = _player.transform.position - transform.position;
            distance.Normalize();
            float rotateAngle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg + 90;
            Quaternion angle = Quaternion.AngleAxis(rotateAngle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, angle, 3f * Time.deltaTime);

            if (transform.position.y <= 1.5f)
                transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
        }
    }

}
