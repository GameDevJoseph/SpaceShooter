using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMissiles : MonoBehaviour
{
    [SerializeField] Player _player;
    [SerializeField] GameObject _telegraphBoostArea;
    [SerializeField] float _enemySpeed;

    bool _hasObtainedPlayerPos;
    bool _hasLockedOntoPlayer;
    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
            Debug.LogError("Player is null");

        Destroy(this.gameObject, 10f);
    }

    private void Update()
    {
        ChargeAtPlayer();

    }
    void ChargeAtPlayer()
    {
        if (_player == null)
            return;

        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (!_hasLockedOntoPlayer)
        {

            _enemySpeed = 0;
            _hasLockedOntoPlayer = true;
            StartCoroutine(TargetPlayer());
        }
        else if (_hasLockedOntoPlayer && !_hasObtainedPlayerPos)
        {
            if (_player != null)
            {
                Vector2 distance = _player.transform.position - transform.position;
                distance.Normalize();
                float rotateAngle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg + 90;
                Quaternion angle = Quaternion.AngleAxis(rotateAngle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, angle, 3f * Time.deltaTime);
            }
        }
    }
    IEnumerator TargetPlayer()
    {
        yield return new WaitForSeconds(3f);
        _telegraphBoostArea.SetActive(true);
        yield return new WaitForSeconds(3f);
        _hasObtainedPlayerPos = true;
        yield return new WaitForSeconds(0.5f);
        _enemySpeed = 30f;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();

            player.Damage();
            Destroy(this.gameObject);
        }
    }
}
