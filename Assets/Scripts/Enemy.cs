using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Base Data")]
    [SerializeField] int _enemyID;
    [SerializeField] float _enemySpeed;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _explosionAudio;

    [Header("Normal Ship Data")]
    [SerializeField] GameObject _laserPrefab;

    [Header("Bomb Ship Data")]
    [SerializeField] GameObject _bombPrefab;

    [Header("Charged Laser Ship Data")]
    [SerializeField] GameObject _chargedLaser;
    [SerializeField] AudioClip _chargedLaserPowerOn;
    [SerializeField] AudioClip _chargedLaserPowerOff;
    [SerializeField] GameObject _telegraphLaserBeam;

    [Header("Shield Ship Data")]
    [SerializeField] GameObject _enemyShield;
    [SerializeField] bool _isEnemyShielded = false;
    [SerializeField] AudioClip _shieldPowerDownAudio;

    [Header("ZigZag Ship Data")]
    [SerializeField] float _zigzagSpeed;
    [SerializeField] GameObject _boosters;
    [SerializeField] GameObject _telegraphBoostArea;

    [Header("Shooting Backwards Ship Data")]
    [SerializeField] GameObject _backwardLaserShot;
    [SerializeField] float _maxXDistance;
    [SerializeField] float _minXDistance;
    [SerializeField] float _distanceBehindPlayer;
    [SerializeField] Vector3 _laserOffset;


    bool _hasFiredShotBackwards = false;
    bool _isNearPlayer;

    Animator _animator;
    Player _player;
    float _fireRate = 3.0f;
    float _canFire = -1f;
    bool _canFireLasers = false;
    SpawnManager _spawnManager;
    bool _isChargedLaserFiring = false;

    bool _hasObtainedPlayerPos;
    bool _hasLockedOntoPlayer;
    public int EnemyID { get { return _enemyID; } }

    void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _canFireLasers = true;

        if (_spawnManager == null)
            Debug.LogError("Spawn Manager is null");

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
        switch (_enemyID)
        {
            case 0: NormalShipBehavior(); FireLasers(); break;
            case 1: BlockadeShipBehavior(); break;
            case 2: LeftSideSwipperShipBehavior(); FireBombs(); break;
            case 3: RightSideSwipperShipBehavior(); FireBombs(); break;
            case 4: ChargeLaserShipBehavior(); FireChargedLaser(); break;
            case 5: ZigZagShipBehavior(); ChargeAtPlayer(); break;
            case 6: BackwardShootingShipBehavior(); break;
        }
    }
    void FireLasers()
    {
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;

            if (!_canFireLasers)
                return;

            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }
    void FireBombs()
    {
        StartCoroutine(StartBombing());
    }
    void FireChargedLaser()
    {
        if (_enemySpeed == 0)
        {
            if (_chargedLaser == null)
                return;

            if (_isChargedLaserFiring)
                return;

            _isChargedLaserFiring = true;
            StartCoroutine(ChargedLaserCoolDown());

        }
    }
    void ChargeAtPlayer()
    {
        if (_player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        if (distanceToPlayer < 4 && !_hasLockedOntoPlayer)
        {

            _enemySpeed = 0;
            _zigzagSpeed = 0;
            _hasLockedOntoPlayer = true;
            StartCoroutine(BashPlayer());
        }
        else if (_hasLockedOntoPlayer && !_hasObtainedPlayerPos)
        {
            if (_player != null)
            {
                Vector2 distance = _player.transform.position - transform.position;
                distance.Normalize();
                float rotateAngle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg + 90;
                Quaternion angle = Quaternion.AngleAxis(rotateAngle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, angle, 3f * Time.deltaTime);
            }
        }
    }
    IEnumerator ShootingBackwards()
    {
        _enemySpeed = 1;
        yield return new WaitForSeconds(0.3f);
        if (!_hasFiredShotBackwards)
        {
            _hasFiredShotBackwards = true;
            _laserOffset = new Vector3(0, 1.75f, 0);
            GameObject enemyLaser = Instantiate(_backwardLaserShot, transform.position + _laserOffset, Quaternion.identity);
        }
        yield return new WaitForSeconds(.2f);
        _enemySpeed = 4f;
    }
    IEnumerator BashPlayer()
    {
        yield return new WaitForSeconds(3f);
        _telegraphBoostArea.SetActive(true);
        yield return new WaitForSeconds(2f);
        _hasObtainedPlayerPos = true;
        _boosters.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        _enemySpeed = 30f;
    }
    IEnumerator EnemyShieldRecharge()
    {
        yield return new WaitForSeconds(8f);
        _isEnemyShielded = true;
        _enemyShield.gameObject.SetActive(true);
    }
    IEnumerator StartBombing()
    {
        yield return new WaitForSeconds(3f);
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(6f, 10f);
            _canFire = Time.time + _fireRate;
            Instantiate(_bombPrefab, transform.position, Quaternion.identity);
        }
    }
    IEnumerator ChargedLaserCoolDown()
    {
        while (true)
        {
            _telegraphLaserBeam.SetActive(true);
            yield return new WaitForSeconds(2f);

            if (_chargedLaser != null)
                _chargedLaser.gameObject.SetActive(true);

            _audioSource.PlayOneShot(_chargedLaserPowerOn);
            _animator.SetBool("ShootChargedLaser", true);
            _telegraphLaserBeam.SetActive(false);
            yield return new WaitForSeconds(10f);
            _animator.SetBool("ShootChargedLaser", false);

            if (_chargedLaser != null)
                _chargedLaser.gameObject.SetActive(false);

            yield return new WaitForSeconds(10f);
            _isChargedLaserFiring = false;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player == null)
                return;

            if (_isEnemyShielded)
            {
                EnemyShieldCollision();
                collision.GetComponent<Player>().Damage();
                return;
            }
            EnemyCollision();
            collision.GetComponent<Player>().Damage();
            Destroy(this.gameObject, 2.8f);
        }
        if (collision.CompareTag("Laser"))
        {
            if (_isEnemyShielded)
            {
                EnemyShieldCollision();
                Destroy(collision.gameObject);
                Destroy(collision.transform.parent.gameObject);
                return;
            }
            Destroy(collision.gameObject);

            if (_chargedLaser != null)
                Destroy(_chargedLaser);

            EnemyCollision();
            Destroy(this.gameObject, 2.8f);
        }
    }
    public void MineExplosion()
    {
        if (_isEnemyShielded)
        {
            EnemyShieldCollision();
            return;
        }
        EnemyCollision();
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
    void LeftSideSwipperShipBehavior()
    {
        transform.Translate(Vector3.right * _enemySpeed * Time.deltaTime);
        if (transform.position.x > 12)
        {
            float randomY = Random.Range(2, 5);
            transform.position = new Vector3(-12, randomY, 0f);
        }
    }
    void RightSideSwipperShipBehavior()
    {
        transform.Translate(Vector3.left * _enemySpeed * Time.deltaTime);
        if (transform.position.x < -12)
        {
            float randomY = Random.Range(2, 5);
            transform.position = new Vector3(12, randomY, 0f);
        }
    }
    void ChargeLaserShipBehavior()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        if (transform.position.y <= 5)
        {
            _enemySpeed = 0;
        }
    }
    void ZigZagShipBehavior()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        float zigPercentage = Mathf.PingPong(Time.time, 1);
        Vector3 maxLeftMovement = new Vector3(-.05f * _zigzagSpeed, 0, 0);
        Vector3 maxRightMovement = new Vector3(.05f * _zigzagSpeed, 0, 0);
        transform.position = Vector3.Slerp(transform.position + maxLeftMovement, transform.position + maxRightMovement, zigPercentage);
        if (transform.position.y < -5.5f || transform.position.y > 14 || transform.position.x > 12 || transform.position.x < -12)
        {
            _enemySpeed = 2f;
            _zigzagSpeed = 0.5f;
            _boosters.SetActive(false);
            _hasObtainedPlayerPos = false;
            _telegraphBoostArea.SetActive(false);
            _hasLockedOntoPlayer = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            StopAllCoroutines();
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 12f, 0f);
        }
    }
    void BackwardShootingShipBehavior()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        _laserOffset = new Vector3(0, 0, 0);
        FireLasers();

        if (_player == null)
            return;

        Vector2 distance = transform.position - _player.transform.position;
        if (distance.x < _maxXDistance && distance.x > _minXDistance && distance.y <= _distanceBehindPlayer)
            _isNearPlayer = true;

        if (_isNearPlayer)
        {
            _canFireLasers = false;
            if (!_hasFiredShotBackwards)
                StartCoroutine(ShootingBackwards());
        }

        if (transform.position.y < -6f)
        {
            StopAllCoroutines();
            _hasFiredShotBackwards = false;
            _canFireLasers = true;
            _isNearPlayer = false;
            int randomX = Random.Range(-8, 8);
            transform.position = new Vector3(randomX, 12, 0);
        }
    }
    void EnemyShieldCollision()
    {
        _isEnemyShielded = false;
        _enemyShield.gameObject.SetActive(false);
        _audioSource.PlayOneShot(_shieldPowerDownAudio);
        StartCoroutine(EnemyShieldRecharge());
    }
    void EnemyCollision()
    {
        if (_animator != null)
            _animator.SetTrigger("OnEnemyDeath");

        if (_boosters != null)
            _boosters.SetActive(false);

        if (_telegraphBoostArea != null)
            _telegraphBoostArea.SetActive(false);

        if (_telegraphLaserBeam != null)
            _telegraphLaserBeam.SetActive(false);

        _spawnManager.OnEnemyDeath();

        if (_player != null)
            _player.AddToScore(10);

        StopAllCoroutines();
        _audioSource.Play();
        _enemySpeed = 0f;
        _zigzagSpeed = 0f;
        _canFireLasers = false;

        this.GetComponent<Collider2D>().enabled = false;
    }
}
