using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Tweens;

public class IntroController : AudioManager, ILightController
{
    public Light prefab = null;
    public int radius1 = 468;
    public int radius2 = 515;
    public float intensity = 1;
    public Color color = new Color(0, 0, 1);
    public Camera camera;
    public LayerMask introMask;

    private List<Light> _lights = new List<Light>();
    private float pos = 0;
    private GameObject eye;

    public AudioSource[] impactSounds;

    private Vector3 pointOnEllipse(float rad, float w, float h) {
        return new Vector3(math.cos(rad) * w / 2, math.sin(rad) * h / 2);
    }

    private Vector3 getPosition(float p) {
        if ((p - Mathf.Floor(p)) < 0.2f) {
            return pointOnEllipse(Mathf.PI * 2 / 7 * p, radius1, radius1);
        }
        return pointOnEllipse(Mathf.PI * 2 / 7 * p, radius2, radius2);
    }

    void Start()
    {
        camera.cullingMask = introMask;
        base.Start();
        _lights.Add(createLight(_lights.Count, color));
    }

    void Update()
    {
        base.Update();
    }

    private Light createLight(int index, Color color)
    {
        var go = Instantiate(prefab, transform);
        go.transform.localPosition = getPosition(pos);
        go.transform.localRotation = Quaternion.identity;

        Light light = go.GetComponent<Light>();
        LightManager lightManager = light.GetComponent<LightManager>();
        if (lightManager != null) {
            lightManager.index = index;
            lightManager.color = color;
            lightManager.intensity = intensity;
        }
        return light;
    }

    public void LightOnTriggerEnter(int light, Collider other)
    {
        var lightManager = _lights[light].GetComponent<LightManager>();
        var index = (int) pos;
        switch (index) {
            case 0:
            case 1:
            case 2:
            case 3:
                if (lightManager) {
                    lightManager.toggleCord(true);
                    lightManager.toggleSphere(false);
                }
                if (impactSounds[index]) {
                    impactSounds[index].Play();
                    impactSounds[index].gameObject.transform.parent = _lights[light].gameObject.transform;
                }
                if (audioSources[index]) {
                    FadeIn(audioSources[index]);
                    //audioSources[index].gameObject.transform.parent = _lights[light].gameObject.transform;
                }
                if (index == 3) {
                    camera.cullingMask = -1;
                    introComplete(light);
                }
                else {
                    tweenToNextPosition(light);
                }
                break;
        }
    }

    private void tweenToNextPosition(int light) {
        pos += 0.5f;
        var tween = new LocalPositionTween {
            to = getPosition(pos + 0.1f),
            duration = 5,
            onEnd = (instance) => {
                pos += 0.5f;
                var tween2 = new LocalPositionTween {
                    to = getPosition(pos),
                    duration = 5,
                    onEnd = (instance) => {
                        LightManager lightManager = _lights[light].GetComponent<LightManager>();
                        lightManager.toggleCord(false);
                        lightManager.toggleSphere(true);
                    }
                };
                _lights[light].gameObject.AddTween(tween2);
            },
        };
        _lights[light].gameObject.AddTween(tween);
    }

    private void introComplete(int light) {
        var tween = new LocalPositionTween {
            to = getPosition(pos + 0.12f),
            duration = 1,
            onEnd = (instance) => {
                pos += 1f;
                lightToCenter(light);
            }
        };
        _lights[light].gameObject.AddTween(tween);
    }

    private void lightToCenter(int light) {
        var dest = new Vector3(0, 3, 0);
        var mid = Vector3.Lerp(dest, _lights[light].gameObject.transform.localPosition, 0.5f);
        mid.z = -20;
        var tween = new LocalPositionTween {
            to = mid,
            duration = 4,
            onEnd = (instance) => {
                var tween2 = new LocalPositionTween
                {
                    to = dest,
                    duration = 4,
                    onEnd = (instance) =>
                    {
                        onEnd(light);
                    }
                };
                _lights[light].gameObject.AddTween(tween2);
            }
        };
        _lights[light].gameObject.AddTween(tween);
    }

    private void onEnd(int light) {
        Destroy(_lights[light].gameObject);

        for (var i=0; i<3; i++) {
            FadeOutStop(audioSources[i]);
        }

        var field = FindObjectsOfType<FieldController>()[0];
        field.init();

        StartCoroutine(RunInterval(1f));
    }

    IEnumerator RunInterval(float interval)
    {
        var field = FindObjectsOfType<FieldController>()[0];
        while (true)
        {
            int secs = (int)audioSources[3].time;
            if (secs == 75 || secs == 150)
            {
                field.startLaser();
            }

            yield return new WaitForSeconds(interval);
        }
    }
}
