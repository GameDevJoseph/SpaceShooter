using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissile : MonoBehaviour
{
    [SerializeField] Enemy _targetedEnemy;
    [SerializeField] MissileDetection _enemiesFromMissileRadius;
    [SerializeField] LockOnVisual _lockOnVisual;

    LockOnVisual _lockOn;

    private void Update()
    {
        transform.Translate(Vector2.up * 1f * Time.deltaTime);
        if (!_targetedEnemy)
        {
            StopAllCoroutines();
            return;
        }

        StartCoroutine(LockedOnEnemy());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
            Destroy(_lockOn.gameObject);
        }
    }

    public void AssignEnemy(Enemy enemy)
    {
        _targetedEnemy = enemy;
        Transform LockOnEnemyGameObject = _targetedEnemy.gameObject.transform;
        _lockOn = Instantiate(_lockOnVisual, LockOnEnemyGameObject.position, Quaternion.identity);
        _lockOn.transform.parent = LockOnEnemyGameObject.transform;
    }

    IEnumerator LockedOnEnemy()
    {
        yield return new WaitForSeconds(1f);
        transform.Translate(Vector2.up * 5f * Time.deltaTime);
        Vector2 distance = transform.position - _targetedEnemy.transform.position;
        distance.Normalize();
        float rotateAngle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg + 90;
        Quaternion angle = Quaternion.AngleAxis(rotateAngle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, angle, 3f * Time.deltaTime);
    }

}
