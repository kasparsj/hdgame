using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class OscPositionX : MonoBehaviour
{
    public float amplitude = 30f;
    public float speed = 3f;
    public bool startOnAwake = false;
    private Vector3? startPosition = null;
    private Vector3? initialPosition = null;

    void Awake()
    {
        initialPosition = transform.localPosition;
        if (startOnAwake) {
            startPosition = initialPosition;
        }
    }

    void Update()
    {
        if (startPosition != null) {
            transform.localPosition = (Vector3)startPosition + Vector3.right * Mathf.Sin(Time.time * speed) * amplitude;
        }
    }

    public void startOsc() {
        startPosition = initialPosition;
    }

    public void stopOsc() {
        startPosition = null;
    }
}
