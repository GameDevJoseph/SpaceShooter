using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SpawnManager : MonoBehaviour
{
    [Header("Powerup Spawn Data")]
    [SerializeField] GameObject[] _commonPowerups;
    [SerializeField] GameObject[] _uncommonPowerups;
    [SerializeField] GameObject[] _rarePowerups;
    [SerializeField] GameObject[] _epicPowerups;
    [SerializeField] GameObject[] _legendaryPowerups;
    [SerializeField] GameObject _powerupContainer;

    [Header("Enemy Spawn Data")]
    [SerializeField] Enemy[] _commonEnemyPrefab;
    [SerializeField] Enemy[] _uncommonEnemyPrefab;
    [SerializeField] Enemy[] _rareEnemyPrefab;
    [SerializeField] Enemy[] _epicEnemyPrefab;
    [SerializeField] Enemy[] _legendaryEnemyPrefab;
    [SerializeField] GameObject _enemyContainer;

    [Header("Boss Spawn Data")]
    [SerializeField] Boss _bossPrefab;

    [Header("Wave Data")]
    [SerializeField] int _currentWave;
    [SerializeField] int _waveAmount = 5;
    [SerializeField] int _currentSpawnedAmount;
    [SerializeField] List<int> _enemyWaveAmount = new List<int>();
    [SerializeField] int _amountToAddToWave;
    [SerializeField] int _waveEnemyCount;


    Vector3 _posToSpawn;
    UIManager _UIManager;
    bool _stopSpawning = false;


    // Start is called before the first frame update
    void Start()
    {
        _UIManager = GameObject.Find("UI Manager").GetComponent<UIManager>();

        if (_UIManager == null)
            Debug.LogError("UI Manager is null");

        _waveEnemyCount = _enemyWaveAmount[0];
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(1f);
        while (!_stopSpawning && _currentWave <= _enemyWaveAmount.Count)
        {
            if (_enemyWaveAmount[_currentWave] > 0)
            {
                if (_currentSpawnedAmount < _waveEnemyCount)
                {
                    if (_currentWave % 5 == 0)
                    {
                        _posToSpawn = new Vector3(0, 12f, 0);
                        Instantiate(_bossPrefab, _posToSpawn, Quaternion.identity);
                        _enemyWaveAmount[_currentWave] = 1;
                        _waveEnemyCount = 1;
                        _currentSpawnedAmount++;
                    }
                    else
                    {
                        
                        Enemy spawningEnemy;
                        int spawnPercentage = Random.Range(0, 101);
                        if (spawnPercentage <= 60)
                            spawningEnemy = _commonEnemyPrefab[Random.Range(0, _commonEnemyPrefab.Length)];
                        else if (spawnPercentage > 60 && spawnPercentage <= 80)
                            spawningEnemy = _uncommonEnemyPrefab[Random.Range(0, _uncommonEnemyPrefab.Length)];
                        else if (spawnPercentage > 80 && spawnPercentage <= 90)
                            spawningEnemy = _rareEnemyPrefab[Random.Range(0, _rareEnemyPrefab.Length)];
                        else if (spawnPercentage > 90 && spawnPercentage <= 98)
                            spawningEnemy = _epicEnemyPrefab[Random.Range(0, _epicEnemyPrefab.Length)];
                        else
                            spawningEnemy = _legendaryEnemyPrefab[Random.Range(0, _legendaryEnemyPrefab.Length)];

                        switch (spawningEnemy.EnemyID)
                        {
                            case 0: _posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0); break;
                            case 1: _posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0); break;
                            case 2: _posToSpawn = new Vector3(-12, Random.Range(3f, 6f), 0); break;
                            case 3: _posToSpawn = new Vector3(12, Random.Range(3f, 6f), 0); break;
                            case 4: _posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0); break;
                            case 5: _posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0); break;
                            case 6: _posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0); break;
                            case 7: _posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0); break;
                        }
                        Enemy newEnemy = Instantiate(spawningEnemy, _posToSpawn, Quaternion.identity);
                        _currentSpawnedAmount++;
                        newEnemy.transform.parent = _enemyContainer.transform;
                        yield return new WaitForSeconds(1f);
                    }
                }
                else
                    yield return null;
            }
            else
            {
                _UIManager.StartNewWaveMessage();
                yield return new WaitForSeconds(5f);
                _currentSpawnedAmount = 0;
                _enemyWaveAmount.Add(_waveAmount += _amountToAddToWave);
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
            int spawnPercentage = Random.Range(0, 101);
            GameObject spawningPowerup;
            if (spawnPercentage <= 60)
                spawningPowerup = _commonPowerups[Random.Range(0, _commonPowerups.Length)];
            else if (spawnPercentage > 60 && spawnPercentage <= 80)
                spawningPowerup = _uncommonPowerups[Random.Range(0, _uncommonPowerups.Length)];
            else if (spawnPercentage > 80 && spawnPercentage <= 90)
                spawningPowerup = _rarePowerups[Random.Range(0, _rarePowerups.Length)];
            else if (spawnPercentage > 90 && spawnPercentage <= 98)
                spawningPowerup = _epicPowerups[Random.Range(0, _epicPowerups.Length)];
            else
                spawningPowerup = _legendaryPowerups[Random.Range(0, _legendaryPowerups.Length)];

            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0);
            GameObject newPowerup = Instantiate(spawningPowerup, posToSpawn, Quaternion.identity);
            newPowerup.transform.parent = _powerupContainer.transform;
            yield return new WaitForSeconds(Random.Range(5, 8));
        }
    }

    public void OnPlayerDeath() => _stopSpawning = true;

    public void OnEnemyDeath() => _enemyWaveAmount[_currentWave]--;
}
