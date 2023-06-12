using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionMine : MonoBehaviour
{
    [SerializeField] float _mineSpeed = 3f;
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] SpriteRenderer _spriteRenderer;
    private float _timer;


    // Start is called before the first frame update
    private void Start()
    {
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            Debug.LogError("Sprite Renderer is null");

        StartCoroutine(StopMine());
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _mineSpeed * Time.deltaTime);
    }
    IEnumerator FlashMine()
    {
        yield return new WaitForSeconds(3f);
        _timer = 0;
        while (_timer < 3f)
        {
            _timer += Time.deltaTime * 60f;
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.15f);
        }
        StartCoroutine(ExplodeMine());
    }

    IEnumerator StopMine()
    {
        yield return new WaitForSeconds(2f);
        _mineSpeed = 0;
        StartCoroutine(FlashMine());
    }
    IEnumerator ExplodeMine()
    {
        Instantiate(_explosionPrefab.transform, transform.position, Quaternion.identity);
        Destroy(gameObject);
        yield return new WaitForSeconds(.5f);
    }
}
