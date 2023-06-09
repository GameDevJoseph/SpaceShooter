using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnVisual : MonoBehaviour
{

    void Update()
    {
        float rotationAmount = Mathf.PingPong(0.5f * Time.time * 1f, 1f);
        Quaternion startRotation = Quaternion.Euler(0, 0, 0);
        Quaternion endRotation = Quaternion.Euler(0, 0, 180);
        transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotationAmount);
    }
}
