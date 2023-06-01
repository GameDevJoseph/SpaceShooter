using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Spawn Data")]
    [SerializeField] GameObject[] _enemyPrefab;
    [SerializeField] GameObject _enemyContainer;

    [Header("Powerup Spawn Data")]
    [SerializeField] GameObject[] powerups;
    [SerializeField] GameObject minePowerup;
    [SerializeField] GameObject _powerupContainer;

    bool _stopSpawning = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(3f);
        while(!_stopSpawning)
        {
                Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7f, 0);
                GameObject newEnemy = Instantiate(_enemyPrefab[Random.Range(0,_enemyPrefab.Length)], posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                yield return new WaitForSeconds(5f);
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
            if(DiceRoll >= 3 && DiceRoll <= 4)
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
}
