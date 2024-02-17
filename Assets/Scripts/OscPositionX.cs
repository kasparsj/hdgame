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

    void Awake()
    {
        if (startOnAwake) {
            startPosition = transform.localPosition;
        }
    }

    void Update()
    {
        if (startPosition != null) {
            transform.localPosition = (Vector3)startPosition + Vector3.right * Mathf.Sin(Time.time * speed) * amplitude;
        }
    }

    public void startOsc() {
        startPosition = transform.localPosition;
    }
}
