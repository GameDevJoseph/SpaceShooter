using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPowerupDetector : MonoBehaviour
{
    [SerializeField] List<GameObject> _powerups = new List<GameObject>();
    [SerializeField] Enemy _enemy;
    [SerializeField] GameObject _lockedOnPowerup;

    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();

        if (_enemy == null)
            Debug.LogError("Enemy is null");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Powerups"))
            _powerups.Add(collision.gameObject);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Powerups"))
            _powerups.Remove(collision.gameObject);
    }

    private void Update()
    {
        if (!_enemy)
            return;

        if (_powerups.Count <= 0)
            return;

        foreach (GameObject powerup in _powerups)
        {
            if (_enemy == null)
                return;

            if (powerup == null)
                return;

            Vector2 distance = _enemy.transform.position - powerup.transform.position;
            if (distance.y > 5f)
            {
                _lockedOnPowerup = powerup;
                _enemy.EnemyShootingPowerup(_lockedOnPowerup);
            }
        }
    }
}
