using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public int index = 0;
    public Color color = new Color(1, 1, 1);
    public float intensity = 1;
    public uint randomSeed = 0;

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

        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null) {
            renderer.material.SetColor("_EmissiveColor", col * amp * intensity / 2);
        }
    }

    public void toggleCord(bool visible)
    {
        Transform cordTransform = transform.Find("Cord");
        if (cordTransform != null) {
            cordTransform.gameObject.SetActive(visible);
            if (visible) {
                var cordController = cordTransform.gameObject.GetComponent<LightCordController>();
                if (cordController) {
                    cordController.randomSeed = (float) randomSeed;
                }
            }
        }
    }

    public void toggleSphere(bool visible)
    {
        var light = GetComponent<Light>();
        light.enabled = visible;

        Transform sphereTransform = transform.Find("Sphere");
        if (sphereTransform != null) {
            sphereTransform.gameObject.SetActive(visible);
        }
    }

    public void toggleBars(bool visible)
    {
        Transform barsTransform = transform.Find("Bars");
        if (barsTransform != null) {
            barsTransform.gameObject.SetActive(visible);
            if (visible) {
                var barController = barsTransform.gameObject.GetComponentInChildren<LightBarController>();
                if (barController) {
                    barController.randomSeed = randomSeed;
                }
            }
        }
    }

    public void ParentOnTriggerEnter(Collider other)
    {
        toggleCord(true);
        toggleSphere(false);

        ILightController controller = GetComponentInParent<ILightController>();
        if (controller != null) {
            GetComponentInParent<ILightController>().LightOnTriggerEnter(index, other);
        }
    }
}
