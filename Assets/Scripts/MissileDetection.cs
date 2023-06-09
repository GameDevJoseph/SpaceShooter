using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileDetection : MonoBehaviour
{
    [SerializeField] List<Enemy> _enemies = new List<Enemy>();
    [SerializeField] PlayerMissile _missile;
    [SerializeField] Enemy _lockedOnEnemy;
    bool _hasLockedIn = false;

    private void Start()
    {
        _missile = GetComponentInParent<PlayerMissile>();
    }
    private void Update()
    {
        if (_enemies.Count <= 0)
            return;

        foreach (Enemy enemy in _enemies)
        {
            if (!_hasLockedIn)
            {
                _lockedOnEnemy = enemy;
                _missile.AssignEnemy(_lockedOnEnemy);
                _hasLockedIn = true;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy == null)
                return;

            if (_enemies.Contains(enemy))
                return;

            _enemies.Add(enemy);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy == null)
                return;

            _enemies.Remove(enemy);
            _hasLockedIn = false;
        }
    }
}


