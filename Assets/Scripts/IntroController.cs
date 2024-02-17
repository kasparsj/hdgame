using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Tweens;

public class IntroController : AudioManager, ILightController
{
    public Light prefab = null;
    public int radius = 50;
    public int radius2 = 55;
    public float intensity = 1;
    public Color color = new Color(1, 1, 1);
    private List<Light> _lights = new List<Light>();
    private float pos = 0;

    private Vector3 pointOnEllipse(float rad, float w, float h) {
        return new Vector3(math.cos(rad) * w / 2, math.sin(rad) * h / 2);
    }

    private Vector3 getPosition(float p) {
        if ((p - Mathf.Floor(p)) < 0.2f) {
            return pointOnEllipse(Mathf.PI * 2 / 7 * p, radius, radius);
        }
        return pointOnEllipse(Mathf.PI * 2 / 7 * p, radius2, radius2);
    }

    void Start()
    {
        base.Start();

        var go = Instantiate(prefab, transform);
        go.transform.localPosition = getPosition(pos);
        go.transform.localRotation = Quaternion.identity;

        Light light = go.GetComponent<Light>();
        LightManager lightManager = light.GetComponent<LightManager>();
        if (lightManager != null) {
            lightManager.color = color;
            lightManager.intensity = intensity;
        }
        else {
            Debug.Log("LightManager not found");
        }
        _lights.Add(light);
    }

    void Update()
    {
        base.Update();
    }

    public void LightOnTriggerEnter(int light, Collider other)
    {
        var index = (int) pos;
        if (audioSources[index]) {
            UnmuteAudioChannel(audioSources[index]);
            var tween = new AudioSourceVolumeTween {
                from = 0,
                to = 1,
                duration = 0.5f,
            };
            audioSources[index].gameObject.AddTween(tween);
        }
        if (index == audioSources.Length-1) {
            introComplete(light);
        }
        else {
            tweenToNextPosition(light);
        }
    }

    private void tweenToNextPosition(int light) {
        pos += 0.5f;
        var tween = new LocalPositionTween {
            to = getPosition(pos + 0.1f),
            duration = 4,
            onEnd = (instance) => {
                pos += 0.5f;
                var tween2 = new LocalPositionTween {
                    to = getPosition(pos),
                    duration = 4,
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
                lightToHD(light);
            }
        };
        _lights[light].gameObject.AddTween(tween);
    }

    private void lightToHD(int light) {
        var hd = GameObject.Find("hd");
        if (hd != null) {
            var tween2 = new PositionTween {
                to = hd.transform.position,
                duration = 14,
                onEnd = (instance) => {
                    onEnd(light);
                    var field = FindObjectsOfType<FieldController>()[0];
                    field.initEye();
                }
            };
            _lights[light].intensity *= 10;
            _lights[light].range *= 10;
            _lights[light].gameObject.AddTween(tween2);
        }
    }

    private void onEnd(int light) {
        Destroy(_lights[light].gameObject);
        // LightManager lightManager = _lights[light].GetComponent<LightManager>();
        // lightManager.toggleCord(false);
        // lightManager.toggleSphere(false);


        for (var i=0; i<3; i++) {
            var audioSource = audioSources[i];
            var tween3 = new AudioSourceVolumeTween {
                to = 0,
                duration = 3,
                onEnd = (instance) => {
                    audioSource.Stop();
                }
            };
            audioSource.gameObject.AddTween(tween3);
        }
    }
}
