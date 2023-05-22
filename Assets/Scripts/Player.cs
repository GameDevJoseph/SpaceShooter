using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] int _lives = 3;
    [SerializeField] float _moveSpeed = 3.5f;
    [SerializeField] float _speedMultiplier = 2;

    [Header("Player Boundaries")]
    [SerializeField] float _leftBoundary = -11.5f;
    [SerializeField] float _rightBoundary = 11.5f;
    [SerializeField] float _bottomBoundary = -3.5f;
    [SerializeField] float _topBoundary = 0f;

    [Header("Laser Data")]
    [SerializeField] GameObject _laserPrefab;
    [SerializeField] Vector3 _laserSpawnOffset;
    [SerializeField] float _fireRate = 0.5f;

    [Header("Powerup Data")]
    [SerializeField] GameObject _tripleShotPrefab;
    [SerializeField] bool _isTripleShotActive = false;
    [SerializeField] bool _isSpeedBoostActive = false;


    SpawnManager _spawnManager;
    float _canFire = -1f;


    void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
            Debug.LogError("The Spawn Manager is null");

        transform.position = Vector3.zero;
    }


    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            FireLaser();
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive)
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        else
            Instantiate(_laserPrefab, transform.position + _laserSpawnOffset, Quaternion.identity);

    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);


        if(!_isSpeedBoostActive)
            transform.Translate(direction * _moveSpeed * Time.deltaTime);
        else
            transform.Translate(direction * (_moveSpeed * _speedMultiplier) * Time.deltaTime);


        if (transform.position.x > _rightBoundary)
            SetNewPosition(new Vector3(_leftBoundary, transform.position.y, transform.position.z));

        if (transform.position.x < _leftBoundary)
            SetNewPosition(new Vector3(_rightBoundary, transform.position.y, transform.position.z));

        if (transform.position.y <= _bottomBoundary)
            SetNewPosition(new Vector3(transform.position.x, _bottomBoundary, transform.position.z));

        if (transform.position.y > _topBoundary)
            SetNewPosition(new Vector3(transform.position.x, _topBoundary, transform.position.z));
    }

    void SetNewPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void Damage()
    {
        _lives--;

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeeedBoostDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isTripleShotActive = false;
    }

    IEnumerator SpeeedBoostDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isSpeedBoostActive = false;
    }
}
