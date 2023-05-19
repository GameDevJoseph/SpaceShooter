using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] int _lives = 3;
    [SerializeField] float _moveSpeed = 3.5f;

    [Header("Player Input")]
    [SerializeField] PlayerInput _playerInput;

    [Header("Player Boundaries")]
    [SerializeField] float _leftBoundary = -11.5f;
    [SerializeField] float _rightBounary = 11.5f;
    [SerializeField] float _bottomBoundary = -3.5f;

    [Header("Laser Data")]
    [SerializeField] GameObject _laserPrefab;
    [SerializeField] Vector3 _laserSpawnOffset;
    [SerializeField] float _fireRate = 0.5f;


    SpawnManager _spawnManager;
    float _canFire = -1f;
    Vector3 newPosition;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
            Debug.LogError("The Spawn Manager is null");
    }

    // Start is called before the first frame update
    void Start()
    {
        
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            FireLaser();
        
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        Instantiate(_laserPrefab, transform.position + _laserSpawnOffset, Quaternion.identity);
    }

    void CalculateMovement()
    {
        
        var direction = _playerInput.actions["Move"].ReadValue<Vector2>();

        
        transform.Translate(direction * _moveSpeed * Time.deltaTime);


        if (transform.position.x > 11.5f)
            SetNewPosition(new Vector3(_leftBoundary, transform.position.y, transform.position.z));

        if (transform.position.x < -11.5f)
            SetNewPosition(new Vector3(_rightBounary, transform.position.y, transform.position.z));

        if (transform.position.y <= -3.5f)
            SetNewPosition(new Vector3(transform.position.x, _bottomBoundary, transform.position.z));
    }

    Vector3 SetNewPosition(Vector3 pos)
    {
        newPosition = pos;
        transform.position = newPosition;
        return newPosition;
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
}
