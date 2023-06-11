using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] Sprite[] _livesSprites;
    [SerializeField] Image _livesImage;
    [SerializeField] TMP_Text _gameoverText;
    [SerializeField] TMP_Text _restartText;
    [SerializeField] GameManager _gameManager;
    [SerializeField] Slider _thrustSlider;
    [SerializeField] TMP_Text _ammoText;
    [SerializeField] bool _canThrust = false;
    [SerializeField] float _refillThrusterSpeed = 0.1f;

    [SerializeField] Slider _bossHealthDisplay;
    [SerializeField] Image _bossHealthFillAreaColor;

    
    public bool CanThrust { get { return _canThrust; } }
    private void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameoverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (_gameManager == null)
            Debug.LogError("Game Manager is Null");

        
        ThrustOn();
        
    }

    void Update()
    {
        if (_thrustSlider.value <= 0)
            ThrustOff();

        if(!_canThrust)
            StartCoroutine(ThrustRefill());

    }
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateAmmoText(int currentAmmo, int maxAmmo)
    {
        _ammoText.text = "Ammo:" + currentAmmo.ToString() + "/" + maxAmmo.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives < 0)
            return;
        _livesImage.sprite = _livesSprites[currentLives];

        if (currentLives <= 0)
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameoverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlicker());
    }

    IEnumerator GameOverFlicker()
    {
        while (true)
        {
            _gameoverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameoverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ThrustExhaustion()
    {
        if (!_canThrust)
            return;

       if (_thrustSlider.value > _thrustSlider.minValue)
           _thrustSlider.value -= 0.1f * Time.deltaTime;

       if (_thrustSlider.value <= _thrustSlider.minValue)
           _thrustSlider.value = _thrustSlider.minValue;
    }

    public void ThrustOn() => _canThrust = true;
    public void ThrustOff() => _canThrust = false;



    IEnumerator ThrustRefill()
    {
        yield return new WaitForSeconds(5f);
        while (!_canThrust)
        {
            _thrustSlider.value += _refillThrusterSpeed * Time.deltaTime;
            yield return new WaitForSeconds(0.5f);

            if (_thrustSlider.value >= _thrustSlider.maxValue)
            {
                _thrustSlider.value = _thrustSlider.maxValue;
                ThrustOn();
                StopCoroutine(ThrustRefill());
            }
        }
    }


    public void SetMaxHealth(int maxHealth)
    {
        _bossHealthDisplay.maxValue = maxHealth;
        _bossHealthDisplay.value = maxHealth;
    }

    public void UpdateMaxHealth(int currentHealth)
    {
        _bossHealthDisplay.value = currentHealth;

        float healthPercentage = (_bossHealthDisplay.value / _bossHealthDisplay.maxValue) * 100;

        if (healthPercentage >= 75)
            _bossHealthFillAreaColor.color = Color.green;
        else if(healthPercentage < 75 && healthPercentage >= 50)
            _bossHealthFillAreaColor.color = Color.yellow;
        else
            _bossHealthFillAreaColor.color = Color.red;
    }

    public void DisplayHealthVisual(bool v)
    {
        _bossHealthDisplay.gameObject.SetActive(v);
    }

}
