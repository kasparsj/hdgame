using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class FieldController : MonoBehaviour
{
    public Light _prefab = null;
    public int _radius1 = 450;
    public int _radius2 = 250;
    public float _intensity = 4000;
    public Color _color1 = new Color(1, 0, 0);
    public Color _color2 = new Color(0, 1, 0);
    public AudioSource[] audioSources;

    List<Light> _lights1 = new List<Light>();
    List<Light> _lights2 = new List<Light>();

    Vector3 pointOnEllipse(float rad, float w, float h) {
        return new Vector3(math.cos(rad) * w / 2, math.sin(rad) * h / 2);
    }

    void Start()
    {
        for (var i = 0; i < 7; i++)
        {
            AudioSource sound = i < audioSources.Length ? audioSources[i] : null;
            Light light1 = createLight(Mathf.PI * 2 / 7 * (i + 0.5f), _radius1, _color1, sound);
            Light light2 = createLight(Mathf.PI * 2 / 7 * (i + 0), _radius2, _color2, sound);
            _lights1.Add(light1);
            _lights2.Add(light2);
        }
    }

    Light createLight(float rad, float radius, Color color, AudioSource sound)
    {
        var go = Instantiate(_prefab, transform);
        go.transform.localPosition = pointOnEllipse(rad, radius, radius);
        go.transform.localRotation = Quaternion.identity;

        Light light = go.GetComponent<Light>();
        LightManager lightManager = light.GetComponent<LightManager>();
        if (lightManager != null) {
            lightManager.color = color;
            lightManager.intensity = _intensity;
            lightManager.triggerSound = sound;
        }
        return light;
    }

    void Update()
    {
    }

    public void ParentOnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
    }
}
