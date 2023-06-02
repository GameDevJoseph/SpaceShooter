using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Spawn Data")]
    [SerializeField] GameObject[] _enemyPrefab;
    [SerializeField] GameObject _enemyContainer;
    [SerializeField] List<int> _enemyWaveAmount = new List<int>();

    [SerializeField] int _currentWave;
    [SerializeField] int _waveAmount = 5;
    [SerializeField] int _currentSpawnedAmount;

    [Header("Powerup Spawn Data")]
    [SerializeField] GameObject[] powerups;
    [SerializeField] GameObject minePowerup;
    [SerializeField] GameObject _powerupContainer;


    [SerializeField] int _waveEnemyCount;
    bool _stopSpawning = false;
    // Start is called before the first frame update
    void Start()
    {
        _waveEnemyCount = _enemyWaveAmount[0];
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (!_stopSpawning && _currentWave <= _enemyWaveAmount.Count)
        {
            
            if (_enemyWaveAmount[_currentWave] > 0)
            {
                if (_currentSpawnedAmount < _waveEnemyCount)
                {
                    Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0);
                    GameObject newEnemy = Instantiate(_enemyPrefab[Random.Range(0, _enemyPrefab.Length)], posToSpawn, Quaternion.identity);
                    _currentSpawnedAmount++;
                    newEnemy.transform.parent = _enemyContainer.transform;
                    yield return new WaitForSeconds(5f);
                }else
                    yield return null;
            }else
            {
                yield return new WaitForSeconds(3f);
                _currentSpawnedAmount = 0;
                _enemyWaveAmount.Add(_waveAmount += 5);
                yield return new WaitForSeconds(1f);
                _currentWave++;
                _waveEnemyCount = _enemyWaveAmount[_currentWave];
            }
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (!_stopSpawning)
        {
            GameObject newPowerup;
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0);
            int DiceRoll = Random.Range(0, 9);
            if (DiceRoll >= 3 && DiceRoll <= 4)
                newPowerup = Instantiate(minePowerup, posToSpawn, Quaternion.identity);
            else
            {
                GameObject randomPowerUp = powerups[Random.Range(0, powerups.Length)];
                newPowerup = Instantiate(randomPowerUp, posToSpawn, Quaternion.identity);
            }
            newPowerup.transform.parent = _powerupContainer.transform;
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }

    public void OnPlayerDeath() => _stopSpawning = true;

    public void OnEnemyDeath() => _enemyWaveAmount[_currentWave]--;
}
