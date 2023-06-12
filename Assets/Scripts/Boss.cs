using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Boss Data")]
    [SerializeField] int _bossID;
    [SerializeField] int _bossCurrentHP = 50;
    [SerializeField] int _bossMaxHP = 100;
    [SerializeField] float _bossZigZagSpeed;
    

    [Header("Boss Visual Data")]
    [SerializeField] GameObject[] _deathExplosions;
    [SerializeField] GameObject _twinLaserTelegraph;
    
    [Header("Boss Weapon Data")]
    [SerializeField] GameObject _missiles;
    [SerializeField] GameObject _twinLaserPrefab;
    [SerializeField] Vector3[] _posToSpawnMissiles;
    [SerializeField] GameObject _laserPrefab;
    [SerializeField] Vector3 _laserOffset;

    float _fireRate = 3f;
    float _canFire = 0.1f;
    float _bossSpeed = 2f;
    bool _inSetPosition = false;
    int _direction = 1;
    float _specialAttackTimer;

    SpawnManager _spawnManager;
    UIManager _uiManager;
    Collider2D _collider2D;
    Player _player;
    

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("UI Manager").GetComponent<UIManager>();
        _collider2D = GetComponent<Collider2D>();
        if (_uiManager == null)
            Debug.LogError("UI Manager is null");

        if (_spawnManager == null)
            Debug.LogError("Spawn Manager is null");

        if(_player == null)
            Debug.LogError("Player is null");

        _bossCurrentHP = _bossMaxHP;

        
        _uiManager.SetMaxHealth(_bossMaxHP);
        _uiManager.UpdateMaxHealth(_bossCurrentHP);

        _specialAttackTimer = 15f;

        if (_collider2D != null)
            _collider2D.enabled = false;

    }

    private void Update()
    {
        transform.Translate(Vector2.down * _bossSpeed * Time.deltaTime);
        if(transform.position.y <= 5)
        {
            transform.position = new Vector3(transform.position.x, 5, 0);
            _bossSpeed = 0f;
            _uiManager.DisplayHealthVisual(true);
            _inSetPosition = true;
        }

        if (_bossCurrentHP > 0 && _inSetPosition)
        {
            FireLasers();
            transform.Translate(Vector3.left * _direction * _bossZigZagSpeed * Time.deltaTime);

            if (_collider2D != null)
                _collider2D.enabled = true;

            RandomAttack();
            if (transform.position.x >= 6)
            {
                transform.position = new Vector3(6, transform.position.y, 0);
                _direction *= -1;
            }
            else if (transform.position.x <= -6)
            {
                transform.position = new Vector3(-6, transform.position.y, 0);
                _direction *= -1;
            }
                
        }
        if (_bossCurrentHP <= 0)
        {
            StartCoroutine(BossDeathExplosions());
        }
    }
    
    private void FireLasers()
    {
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, new Vector3(transform.position.x, 5f,0), Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }
    void FiringBigLasers()
    {
        _twinLaserTelegraph.SetActive(true);
        StartCoroutine(TwinLasers());
    }

    void MissileFiring()
    {
        for(int i = 0; i < _posToSpawnMissiles.Length; i++)
        {
            GameObject missile = Instantiate(_missiles, transform.position + _posToSpawnMissiles[i], Quaternion.identity);
            missile.transform.parent = transform;
        }
    }

    IEnumerator TwinLasers()
    {
        yield return new WaitForSeconds(3f);
        _twinLaserTelegraph.SetActive(false);
        _twinLaserPrefab.SetActive(true);
        yield return new WaitForSeconds(10f);
        _twinLaserPrefab.SetActive(false);
    }
    IEnumerator BossDeathExplosions()
    {
        _uiManager.DisplayHealthVisual(false);
        for (int i = 0; i < _deathExplosions.Length; i++)
        {
            _deathExplosions[i].SetActive(true);
            yield return new WaitForSeconds(0.3f);
        }
        Destroy(this.gameObject, 1.5f);
    }

    void RandomAttack()
    {
        if (_specialAttackTimer <= 0)
        {
            int value = Random.Range(0, 2);
            switch (value)
            {
                case 0: FiringBigLasers(); _specialAttackTimer = 30f; break;
                case 1: MissileFiring();  _specialAttackTimer = 15f; break;
            }
        }else
        {
            _specialAttackTimer -= Time.deltaTime;
        }
    }


    public void Damage(int damage)
    {
        _bossCurrentHP -= damage;
        _uiManager.UpdateMaxHealth(_bossCurrentHP);
        if (_bossCurrentHP <= 0)
        {
            if (_player != null)
                _player.AddToScore(1000);

            _spawnManager.OnEnemyDeath();
        }
    }
}
