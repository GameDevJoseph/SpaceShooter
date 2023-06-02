using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleObject : MonoBehaviour
{
    [SerializeField] float _scaleSpeed = 3f;
    [SerializeField] Vector3 _lowScaleRange;
    [SerializeField] Vector3 _maxScaleRange;
    
    // Update is called once per frame
    void Update()
    {
        float scaling = Mathf.PingPong(0.5f * Time.time * _scaleSpeed, 1f);
        transform.localScale = Vector3.Lerp(_lowScaleRange, _maxScaleRange, scaling);
    }
}
