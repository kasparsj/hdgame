using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class FieldController : AudioManager, ILightController
{
    public Light _prefab = null;
    public int _radius1 = 450;
    public int _radius2 = 250;
    public float _intensity = 4000;
    public Color _color1 = new Color(1, 0, 0);
    public Color _color2 = new Color(0, 1, 0);

    List<Light> _lights1 = new List<Light>();
    List<Light> _lights2 = new List<Light>();

    Vector3 pointOnEllipse(float rad, float w, float h) {
        return new Vector3(math.cos(rad) * w / 2, math.sin(rad) * h / 2);
    }

    void Start()
    {
        base.Start();

        for (var i = 0; i < 7; i++)
        {
            Light light1 = createLight(i, Mathf.PI * 2 / 7 * (i + 0.5f), _radius1, _color1);
            Light light2 = createLight(7+i, Mathf.PI * 2 / 7 * (i + 0), _radius2, _color2);
            _lights1.Add(light1);
            _lights2.Add(light2);
        }
    }

    Light createLight(int index, float rad, float radius, Color color)
    {
        var go = Instantiate(_prefab, transform);
        go.transform.localPosition = pointOnEllipse(rad, radius, radius);
        go.transform.localRotation = Quaternion.identity;

        Light light = go.GetComponent<Light>();
        LightManager lightManager = light.GetComponent<LightManager>();
        if (lightManager != null) {
            lightManager.index = index;
            lightManager.color = color;
            lightManager.intensity = _intensity;
        }
        return light;
    }

    void Update()
    {
        base.Update();
    }

    public void LightOnTriggerEnter(int index, Collider other)
    {
        if (audioSources.Length > index) {
            UnmuteAudioChannel(audioSources[index]);
            audioSources[index].volume = 0;
            StartCoroutine(AudioFade.In(audioSources[index], 0.5f));
        }
    }
}
