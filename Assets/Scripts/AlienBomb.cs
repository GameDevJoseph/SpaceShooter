using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBomb : MonoBehaviour
{
    [SerializeField] float _bombSpeed = 2f;
    [SerializeField] GameObject _explosionPrefab;

    SpriteRenderer _spriteRenderer;
    private float _flashTimer;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer == null)
            Debug.LogError("SpriteRenderer is null");

        StartCoroutine(StopBomb());
        
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _bombSpeed * Time.deltaTime);
    }

    IEnumerator StopBomb()
    {
        yield return new WaitForSeconds(Random.Range(2f, 4f));
        _bombSpeed = 0;
        StartCoroutine(BombFlash());
    }

    IEnumerator BombFlash()
    {
        yield return new WaitForSeconds(3f);
        _flashTimer = 0;
        while (_flashTimer < 3f)
        {
            _flashTimer += Time.deltaTime * 60f;
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.15f);
        }
        StartCoroutine(ExplodeBomb());
    }
    IEnumerator ExplodeBomb()
    {
        yield return new WaitForSeconds(.5f);
        Instantiate(_explosionPrefab.transform, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    
}
