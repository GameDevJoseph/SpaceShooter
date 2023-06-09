using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetectionZone : MonoBehaviour
{
    [SerializeField] List<GameObject> _lasers = new List<GameObject>();
    [SerializeField] Enemy _enemy;

    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();

        if (_enemy == false)
            Debug.LogError("Enemy is null");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.CompareTag("Laser"))
            _lasers.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Laser"))
            _lasers.Remove(collision.gameObject);
    }


    private void Update()
    {
        if (_lasers.Count <= 0)
            return;

        foreach (GameObject laser in _lasers)
        {
            if (laser == null)
                return;

            Vector2 distance = _enemy.transform.position - laser.transform.position;
            if (distance.y > 1.5f && distance.x < 1f && distance.x > 0)
                _enemy.DodgePlayerShot(Vector3.right);
            else if (distance.y > 1.5f && distance.x > -1f && distance.x < 0)
                _enemy.DodgePlayerShot(Vector3.left);
        }
    }
}
