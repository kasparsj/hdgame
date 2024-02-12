using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class IntroController : MonoBehaviour
{
    public Light prefab = null;
    public int radius = 50;
    public AudioSource[] audioSources;
    public float intensity = 1;
    public Color color = new Color(1, 1, 1);

    private List<Light> _lights = new List<Light>();

    Vector3 pointOnEllipse(float rad, float w, float h) {
        return new Vector3(math.cos(rad) * w / 2, math.sin(rad) * h / 2);
    }

    void Start()
    {
        for (var i=0; i<audioSources.Length; i++) {
            var go = Instantiate(prefab, transform);
            go.transform.localPosition = pointOnEllipse(Mathf.PI * 2 / 7 * i, radius, radius);
            go.transform.localRotation = Quaternion.identity;

            Light light = go.GetComponent<Light>();
            LightManager lightManager = light.GetComponent<LightManager>();
            if (lightManager != null) {
                lightManager.color = color;
                lightManager.intensity = intensity;
                lightManager.triggerSound = audioSources[i];
            }
            else {
                Debug.Log("LightManager not found");
            }
            _lights.Add(light);
        }
    }

    void Update()
    {
    }

    public void ParentOnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
    }
}
