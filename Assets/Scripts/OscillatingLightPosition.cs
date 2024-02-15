using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class OscillatingLightPosition : MonoBehaviour
{
    public float amplitude = 30f; // How far the light moves.
    public float speed = 3f;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = startPosition + Vector3.right * Mathf.Sin(Time.time * speed) * amplitude;
    }
}
