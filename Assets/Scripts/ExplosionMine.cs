using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionMine : MonoBehaviour
{
    [SerializeField] float _mineSpeed = 3f;
    [SerializeField] List<Enemy> _enemyList = new List<Enemy>();
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] SpriteRenderer _spriteRenderer;
    // Start is called before the first frame update
    private void Start()
    {
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            Debug.LogError("Sprite Renderer is null");

        StartCoroutine(FlashMine());
        StartCoroutine(StopMine());
        StartCoroutine(ExplodeMine());
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _mineSpeed * Time.deltaTime);
    }
    IEnumerator FlashMine()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator StopMine()
    {
        yield return new WaitForSeconds(2f);
        _mineSpeed = 0;
    }
    IEnumerator ExplodeMine()
    {
        yield return new WaitForSeconds(7f);
        if (_enemyList.Count > 0)
        {
            foreach (Enemy enemy in _enemyList)
            {
                enemy.MineExplosion();
            }
        }
        Instantiate(_explosionPrefab.transform, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            _enemyList.Add(collision.gameObject.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _enemyList.Remove(collision.gameObject.GetComponent<Enemy>());
    }

}
