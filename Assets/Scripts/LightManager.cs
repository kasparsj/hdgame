using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public Color color;
    public float intensity = 1;
    public AudioSource triggerSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var light = GetComponent<Light>();
        var p = (float3) light.transform.localPosition;
        p.z = Time.time;

        var amp = math.saturate(0.5f + noise.snoise(p) * 0.7f);

        var col = color;
        col = col.linear;

        light.intensity = amp * intensity;
        light.color = col;

        GetComponentInChildren<Renderer>().material.SetColor("_EmissiveColor", col * amp * intensity / 2);
    }

    public void ParentOnTriggerEnter(Collider other)
    {
        if (triggerSound) {
            AudioManager.Instance.ToggleAudioChannel(triggerSound);
        }
    }
}
