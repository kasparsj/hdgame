using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class IntroController : AudioManager, ILightController
{
    public Light prefab = null;
    public int radius = 50;
    public float intensity = 1;
    public Color color = new Color(1, 1, 1);

    private List<Light> _lights = new List<Light>();

    Vector3 pointOnEllipse(float rad, float w, float h) {
        return new Vector3(math.cos(rad) * w / 2, math.sin(rad) * h / 2);
    }

    void Start()
    {
        base.Start();

        for (var i=0; i<audioSources.Length; i++) {
            var go = Instantiate(prefab, transform);
            go.transform.localPosition = pointOnEllipse(Mathf.PI * 2 / 7 * i, radius, radius);
            go.transform.localRotation = Quaternion.identity;

            Light light = go.GetComponent<Light>();
            LightManager lightManager = light.GetComponent<LightManager>();
            if (lightManager != null) {
                lightManager.index = i;
                lightManager.color = color;
                lightManager.intensity = intensity;
            }
            else {
                Debug.Log("LightManager not found");
            }
            _lights.Add(light);
        }
    }

    void Update()
    {
        base.Update();
    }

    public void LightOnTriggerEnter(int index, Collider other)
    {
        if (audioSources[index]) {
            UnmuteAudioChannel(audioSources[index]);
            audioSources[index].volume = 0;
            StartCoroutine(AudioFade.In(audioSources[index], 0.5f));
        }
        if (index == audioSources.Length-1) {
            for (var i=0; i<2; i++) {
                StartCoroutine(AudioFade.Out(audioSources[i], 3));
            }
        }
    }
}
