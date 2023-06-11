using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] int _bossID;
    [SerializeField] int _bossCurrentHP = 50;
    [SerializeField] int _bossMaxHP = 100;
    
    [SerializeField] float _bossZigZagSpeed;

    [SerializeField] GameObject[] _deathExplosions;
    [SerializeField] GameObject _twinLaserTelegraph;

    [SerializeField] GameObject _twinLaserPrefab;
    [SerializeField] GameObject _laserPrefab;

    [SerializeField] GameObject _missiles;
    [SerializeField] Vector3[] _posToSpawnMissiles;
    private float _fireRate = 3f;
    private float _canFire = 0.1f;
    [SerializeField] Vector3 _laserOffset;
    [SerializeField] UIManager _uiManager;
    float _bossSpeed = 2f;
    bool _inSetPosition = false;
    float _bossStartSpeed = 2f;
    int _direction = 1;

    private void Start()
    {
        _uiManager = GameObject.Find("UI Manager").GetComponent<UIManager>();
        if (_uiManager == null)
            Debug.Log("UI Manager is null");



        _bossCurrentHP = _bossMaxHP;

        _uiManager.DisplayHealthVisual(true);
        _uiManager.SetMaxHealth(_bossMaxHP);
        StartCoroutine(RandomAttack());
    }

    private void Update()
    {
        transform.Translate(Vector2.down * _bossSpeed * Time.deltaTime);
        if(transform.position.y <= 5)
        {
            transform.position = new Vector3(transform.position.x, 5, 0);
            _bossSpeed = 0f;
            _inSetPosition = true;
        }

        if (_bossCurrentHP > 0 && _inSetPosition)
        {
            FireLasers();
            _bossZigZagSpeed = 2f;
            transform.Translate(Vector3.left * _direction * _bossZigZagSpeed * Time.deltaTime);

            if (transform.position.x >= 6)
                _direction *= -1;
            else if (transform.position.x <= -6)
                _direction *= -1;
                
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Laser"))
        {
            _bossCurrentHP--;
            _uiManager.UpdateMaxHealth(_bossCurrentHP);
            Destroy(collision.gameObject);
        }
    }

    [ContextMenu("Twin Lasers")]
    void FiringBigLasers()
    {
        _twinLaserTelegraph.SetActive(true);
        StartCoroutine(TwinLasers());
    }

    [ContextMenu("Fire Missile")]
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


    IEnumerator RandomAttack()
    {
        yield return new WaitForSeconds(10f);
        int value = Random.Range(0, 2);
        switch(value)
        {
            case 0: FiringBigLasers(); break;
            case 1: MissileFiring(); break;
        }
        yield return new WaitForSeconds(10f);
    }
}
