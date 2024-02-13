using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Tweens;

public class FieldController : AudioManager, ILightController
{
    public Light _prefab = null;
    public int _radius1 = 450;
    public int _radius2 = 250;
    public float _intensity = 4000;
    public Color _color1 = new Color(1, 0, 0);
    public Color _color2 = new Color(0, 1, 0);

    private List<Light> _lights = new List<Light>();
    private float[] _pos = {
        0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f,
        0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f,
    };

    private Vector3 pointOnEllipse(float rad, float w, float h) {
        return new Vector3(math.cos(rad) * w / 2, 0, math.sin(rad) * h / 2);
    }

    private Vector3 getPosition(int light) {
        if (light < 7) {
            return pointOnEllipse(Mathf.PI * 2 / 7 * (light+0.5f), _radius1 * _pos[light], _radius1 * _pos[light]);
        }
        else {
            return pointOnEllipse(Mathf.PI * 2 / 7 * light, _radius2 * _pos[light], _radius2 * _pos[light]);
        }
    }

    void Start()
    {
        base.Start();

        for (var i = 0; i < 14; i++)
        {
            Light light = createLight(i, i < 7 ? _color1 : _color2);
            _lights.Add(light);
        }
    }

    Light createLight(int index, Color color)
    {
        var pos = getPosition(index);
        var go = Instantiate(_prefab, transform);
        go.transform.localPosition = pos;
        go.transform.localRotation = Quaternion.identity;

        Light light = go.GetComponent<Light>();
        LightManager lightManager = light.GetComponent<LightManager>();
        if (lightManager != null) {
            lightManager.index = index;
            lightManager.color = color;
            lightManager.intensity = _intensity;
        }
        if (audioSources.Length > index) {
            audioSources[index].gameObject.transform.localPosition = pos;
        }
        return light;
    }

    void Update()
    {
        base.Update();
    }

    public void LightOnTriggerEnter(int index, Collider other)
    {
        if (_pos[index] >= 1) return;
        playAudioSource(index);
        tweenToNextPosition(index);
    }

    private void playAudioSource(int light) {
        if (audioSources.Length > light) {
            UnmuteAudioChannel(audioSources[light]);
            var tween = new AudioSourceVolumeTween {
                from = 0,
                to = 1,
                duration = 0.5f,
            };
            audioSources[light].gameObject.AddTween(tween);
        }
    }

    private void lightMute(int light) {
        var tween = new AudioSourceVolumeTween {
            to = 0,
            duration = 0.5f,
            onEnd = (instance) => {
                audioSources[light].mute = true;
                LightManager lightManager = _lights[light].GetComponent<LightManager>();
                lightManager.toggleCord(false);
                lightManager.toggleSphere(true);
            }
        };
        audioSources[light].gameObject.AddTween(tween);
    }

    private void tweenToNextPosition(int light) {
        float delta = light < 7 ? 1/2.0f : 1/1.0f;
        _pos[light] += delta;
        var tween = new LocalPositionTween {
            to = getPosition(light),
            duration = 3,
            onEnd = (instance) => {
                if (_pos[light] >= 1) {
                    lightComplete(light);
                }
                else {
                    lightMute(light);
                }
            }
        };
        _lights[light].gameObject.AddTween(tween);
    }

    private void lightComplete(int light) {
        LightManager lightManager = _lights[light].GetComponent<LightManager>();
        lightManager.toggleCord(false);
        lightManager.toggleSphere(false);
        lightManager.toggleBars(true);
    }
}
