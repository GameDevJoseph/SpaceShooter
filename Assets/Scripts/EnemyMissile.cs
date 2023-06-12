using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    Transform _targetPowerup;

    private void Update()
    {
        if (_targetPowerup == null)
        {
            transform.Translate(Vector2.down * 5f * Time.deltaTime);
            return;
        }

        transform.Translate(Vector2.down * 5f * Time.deltaTime);
        Vector2 distance = _targetPowerup.transform.position - transform.position;
        distance.Normalize();
        float rotateAngle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg + 90;
        Quaternion angle = Quaternion.AngleAxis(rotateAngle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, angle, 3f * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Powerups"))
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }

        if(collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player == null)
                return;
            player.Damage();
            Destroy(this.gameObject);
        }
    }

    public void DetectPowerup(Transform powerup)
    {
        _targetPowerup = powerup;
    }
}
