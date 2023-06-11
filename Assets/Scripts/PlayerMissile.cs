using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissile : MonoBehaviour
{
    [SerializeField] Enemy _targetedEnemy;
    [SerializeField] MissileDetection _enemiesFromMissileRadius;
    [SerializeField] LockOnVisual _lockOnVisual;
    [SerializeField] float _missileNonTargetedSpeed;
    [SerializeField] float _missileTargetedSpeed;


    [SerializeField] LockOnVisual _lockOn;
    private void Start()
    {
        Destroy(this.gameObject, 10f);
    }

    private void Update()
    {
        if (_targetedEnemy == null && _lockOn != null)
            Destroy(_lockOn.gameObject);

        transform.Translate(Vector2.up * _missileNonTargetedSpeed * Time.deltaTime);
        if (!_lockOn)
        {
            StopAllCoroutines();
            return;
        }

        StartCoroutine(LockedOnEnemy());
    }

    private void OnDestroy()
    {
        if(_lockOn)
            Destroy(_lockOn.gameObject);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (_lockOn == null)
                return;

            Enemy enemy = collision.GetComponent<Enemy>();

            Destroy(_lockOn.gameObject);
            enemy.MissileCollision();
        }

        if(collision.CompareTag("Boss"))
        {
            Boss boss = collision.GetComponent<Boss>();
            boss.Damage(10);
            Destroy(this.gameObject);
        }
    }

    public void AssignEnemy(Enemy enemy)
    {
        if (_lockOn != null)
            return;
        
        _targetedEnemy = enemy;
        if (_targetedEnemy == null)
            return;
        Transform LockOnEnemyGameObject = _targetedEnemy.gameObject.transform;
        _lockOn = Instantiate(_lockOnVisual, LockOnEnemyGameObject.position, Quaternion.identity);
        _lockOn.transform.parent = LockOnEnemyGameObject.transform;
    }

    IEnumerator LockedOnEnemy()
    {
        yield return new WaitForSeconds(1f);
        transform.Translate(Vector2.up * _missileTargetedSpeed * Time.deltaTime);
        Vector2 distance = transform.position - _targetedEnemy.transform.position;
        distance.Normalize();
        float rotateAngle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg + 90;
        Quaternion angle = Quaternion.AngleAxis(rotateAngle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, angle, 3f * Time.deltaTime);
    }
}
