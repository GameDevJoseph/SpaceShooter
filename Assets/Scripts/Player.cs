using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class Player : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] int _lives = 3;
    [SerializeField] float _moveSpeed = 3.5f;
    [SerializeField] float _thrustSpeedMultipler = 1.5f;
    [SerializeField] float _speedMultiplier = 2;
    [SerializeField] int _score;
    [SerializeField] int _maxAmmo = 15;
    [SerializeField] PowerupDetector _powerupDetector;

    [Header("Player Boundaries")]
    [SerializeField] float _leftBoundary = -11.5f;
    [SerializeField] float _rightBoundary = 11.5f;
    [SerializeField] float _bottomBoundary = -3.5f;
    [SerializeField] float _topBoundary = 0f;

    [Header("Laser Data")]
    [SerializeField] GameObject _laserPrefab;
    [SerializeField] Vector3 _laserSpawnOffset;
    [SerializeField] float _fireRate = 0.5f;
    [SerializeField] int _laserAmmoCount = 15;

    [Header("Powerup Data")]
    [SerializeField] GameObject _tripleShotPrefab;
    [SerializeField] GameObject _shieldVisualizer;
    [SerializeField] int _shieldDurability = 3;
    [SerializeField] SpriteRenderer _shieldRenderer;
    [SerializeField] GameObject _MinePrefab;

    [Header("Engine Data")]
    [SerializeField] GameObject _rightEngine;
    [SerializeField] GameObject _leftEngine;
    [SerializeField] GameObject _backBooster;
    [SerializeField] GameObject _empStatic;

    [Header("Audio")]
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _laserAudio;
    [SerializeField] AudioClip _noLaserAmmo;

    [Header("Camera")]
    [SerializeField] Camera _mainCamera;
    [SerializeField] float _camShakeDuration = 3f;
    [SerializeField] float _camShakeMultiplier = 1f;

    [SerializeField] GameObject _missilePrefab;

    [SerializeField] bool _isMissileActive = false;
    bool _isTripleShotActive = false;
    bool _isSpeedBoostActive = false;
    bool _isShieldActive = false;
    bool _isMineActive = false;
    SpawnManager _spawnManager;
    float _canFire = -1f;
    UIManager _uiManager;
    bool _isPlayerEMPed;
    [SerializeField] PlayerMissile[] _availableMissiles;
    

    void Start()
    {
        _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("UI Manager").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _powerupDetector = GetComponentInChildren<PowerupDetector>();

        if (_powerupDetector == null)
            Debug.LogError("Powerup Detector is null");

        if (_mainCamera == null)
            Debug.LogError("Main Camera is null");

        if (_audioSource == null)
            Debug.LogError("Audio Source is null");
        else
            _audioSource.clip = _laserAudio;

        if (_spawnManager == null)
            Debug.LogError("The Spawn Manager is null");

        if (_uiManager == null)
            Debug.LogError("UIManager is null");

        transform.position = Vector3.zero;
    }
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            FireLaser();

        if (Input.GetKey(KeyCode.C))
            _powerupDetector.PowerupMagnet(this);

    }
    void FireLaser()
    {
        if (_isPlayerEMPed)
            return;

        _canFire = Time.time + _fireRate;

        if(_isMissileActive)
        {
            SpawnMissiles();
        }
        else if (_isMineActive)
        {
            Instantiate(_MinePrefab, transform.position, Quaternion.identity);
        }
        else
        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position + _laserSpawnOffset, Quaternion.identity);
        }
        else if (_laserAmmoCount > 0)
        {
            Instantiate(_laserPrefab, transform.position + _laserSpawnOffset, Quaternion.identity);
            _laserAmmoCount--;

            if (_laserAmmoCount <= 0)
                _laserAmmoCount = 0;

            _uiManager.UpdateAmmoText(_laserAmmoCount, _maxAmmo);
        }
        else if (_laserAmmoCount <= 0)
        {
            _audioSource.PlayOneShot(_noLaserAmmo);
            return;
        }
        _audioSource.Play();
    }
    public void SpawnMissiles()
    {
        Instantiate(_missilePrefab, transform.position, Quaternion.identity);
        _isMissileActive = false;
    }
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (Input.GetKey(KeyCode.LeftShift) && _uiManager.CanThrust)
        {
            transform.Translate(direction * (_moveSpeed * _thrustSpeedMultipler) * Time.deltaTime);
            _uiManager.ThrustExhaustion();
        }
        else if (!_isSpeedBoostActive)
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
        if (_isShieldActive)
        {
            _shieldRenderer = _shieldVisualizer.gameObject.GetComponent<SpriteRenderer>();
            _shieldDurability--;

            if (_shieldDurability <= 0)
                _shieldDurability = 0;

            if (_shieldRenderer == null)
                return;

            UpdateShieldVisual();
            return;
        }

        _lives--;
        StartCoroutine(CameraShake());
        CheckEngines();
        _uiManager.UpdateLives(_lives);
    }
    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }
    public void MissileActive()
    {
        _isMissileActive = true;
    }
    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeeedBoostDownRoutine());
    }
    public void ShieldsActive()
    {
        _isShieldActive = true;
        _shieldDurability = 3;
        _shieldVisualizer.SetActive(true);

        UpdateShieldVisual();

    }
    public void MinesActive()
    {
        _isMineActive = true;
        StartCoroutine(MinesPowerDownRoutine());
    }
    public void UpdateShieldVisual()
    {
        switch (_shieldDurability)
        {
            case 0: _isShieldActive = false; _shieldVisualizer.SetActive(false); break;
            case 1: _shieldRenderer.color = Color.red; break;
            case 2: _shieldRenderer.color = Color.yellow; break;
            case 3: _shieldRenderer.color = Color.green; break;
            default: break;
        }
    }
    public void EMP()
    {
        StartCoroutine(EMPWareOffRoutine());
    }
    IEnumerator EMPWareOffRoutine()
    {
        var playerBaseSpeed = _moveSpeed;
        _moveSpeed = 0;
        _isPlayerEMPed = true;

        if (_backBooster != null)
            _backBooster.SetActive(false);

        if (_empStatic != null)
            _empStatic.SetActive(true);

        yield return new WaitForSeconds(5f);

        if (_backBooster != null)
            _backBooster.SetActive(true);

        if (_empStatic != null)
            _empStatic.SetActive(false);

        _moveSpeed = playerBaseSpeed;
        _isPlayerEMPed = false;
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
    IEnumerator MinesPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isMineActive = false;
    }
    public void AddToScore(int amount)
    {
        _score += amount;
        _uiManager.UpdateScore(_score);
    }
    public void AddAmmo()
    {
        _laserAmmoCount = _maxAmmo;
        _uiManager.UpdateAmmoText(_laserAmmoCount, _maxAmmo);
    }
    public void LifeUp()
    {
        if (_lives >= 3)
            return;

        _lives++;
        _uiManager.UpdateLives(_lives);
        CheckEngines();
    }
    void CheckEngines()
    {
        switch (_lives)
        {
            case 0: _spawnManager.OnPlayerDeath(); Destroy(this.gameObject); break;
            case 1: _leftEngine.SetActive(true); break;
            case 2: _rightEngine.SetActive(true); _leftEngine.SetActive(false); break;
            case 3: _rightEngine.SetActive(false); _leftEngine.SetActive(false); break;
        }
    }
    IEnumerator CameraShake()
    {
        Vector3 camOriginalPos = _mainCamera.transform.localPosition;
        float elapsedTime = 0f;
        while (elapsedTime < _camShakeDuration)
        {
            float randomX = Random.Range(-0.25f, .25f) * _camShakeMultiplier;
            float randomY = Random.Range(-0.25f, .25f) * _camShakeMultiplier;
            _mainCamera.transform.position = new Vector3
                (camOriginalPos.x + randomX,
                camOriginalPos.y + randomY,
                camOriginalPos.z);
            elapsedTime += Time.deltaTime;
            yield return null;
            _mainCamera.transform.position = camOriginalPos;
        }
    }
}
