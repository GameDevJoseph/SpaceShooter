using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupDetector : MonoBehaviour
{
    [SerializeField] List<GameObject> _powerups = new List<GameObject>();
    
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

    public void PowerupMagnet(Player player)
    {
        if (_powerups.Count <= 0)
            return;

        foreach (GameObject powerup in _powerups)
        {
            powerup.transform.position = Vector3.MoveTowards(powerup.transform.position, player.transform.position,1f * Time.deltaTime);
        }
    }
}
