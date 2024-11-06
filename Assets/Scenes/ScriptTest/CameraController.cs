using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    private Vector3 initialPosition;
    public float shakeDuration = 0.1f;
    public float shakeAmount = 0.1f;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeAmount;
            float y = Random.Range(-1f, 1f) * shakeAmount;
            transform.localPosition = new Vector3(x, y, initialPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = initialPosition;
    }

    void LateUpdate()
    {
        transform.position = playerTransform.position + initialPosition;
    }
}
