using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Tweens;

public class FieldPuzzleController : AudioManager, ILightController
{
    public Light _prefab = null;
    public int minRadius = 50;
    public int maxRadius = 450;
    public float _intensity = 4000;
    public Color _color = new Color(0, 1, 0);
    private GameObject eye;
    private Light[] _lights = new Light[7];
    private int rotation = 0;

    private Vector3 pointOnEllipse(float rad, float w, float h) {
        return new Vector3(math.cos(rad) * w / 2f, 0, math.sin(rad) * h / 2f);
    }

    private int getLevel()
    {
        int level = 0;
        for (var i=0; i<_lights.Length; i++) {
            if (_lights[i] != null) {
                level++;
            }
        }
        return level;
    }

    private Vector3 getPosition(int light, float radius) {
        float angle = Mathf.PI * 2f / 7f;
        // if (light < 7) {
            return pointOnEllipse((rotation+light+0.75f) * angle, radius, radius);
        // }
        // else {
            // return pointOnEllipse((light + rotation) * angle, radius, radius);
        // }
    }

    private float getRadius(int? level = null)
    {
        if (!level.HasValue) {
            level = getLevel();
        }
        float perc = 1.0f - ((int) level / (float) _lights.Length);
        return minRadius + (maxRadius - minRadius) * perc;
    }

    private Vector3 getLevelPosition(int light, int? level = null) {
        return getPosition(light, getRadius(level));
    }

    public void init() {
        var hdGO = GameObject.Find("hd");
        eye = hdGO.transform.Find("eye").gameObject;
        if (eye) {
            eye.SetActive(true);
            StartCoroutine(ScheduleAction(1f, () => {
                shootLightAt(findFreeIndex(), 3f, 2);
                StartCoroutine(ScheduleAction(3f, () => {
                    shootLightAt(findFreeIndex(), 3f, 2);
                    StartCoroutine(ScheduleAction(7f, () => {
                        drawPath(2);
                        var posOsc = eye.GetComponent<OscPositionX>();
                        if (posOsc) {
                            posOsc.startOsc();
                        }
                    }));
                }));
            }));
        }
    }

    void Start()
    {
        base.Start();

        init();
    }

    Light createLight(int index, Color color, Vector3 pos)
    {
        var go = Instantiate(_prefab, transform);
        go.transform.localPosition = pos;
        go.transform.localRotation = Quaternion.identity;

        Light light = go.GetComponent<Light>();
        LightManager lightManager = light.GetComponent<LightManager>();
        if (lightManager != null) {
            lightManager.index = index;
            lightManager.color = color;
            lightManager.intensity = _intensity;
            lightManager.randomSeed = (uint) UnityEngine.Random.Range(0, 100000);
        }
        // if (audioSources.Length > index) {
        //     audioSources[index].gameObject.transform.localPosition = pos;
        // }
        return light;
    }

    void Update()
    {
        base.Update();
    }

    public void LightOnTriggerEnter(int index, Collider other)
    {
        if (_lights[index] == null) return;
        if (allLights()) {
            finish();
            return;
        }
        //playAudioSource(index);

        int target = getMoveIndex(index);
        Debug.Log(index + "/" + target);
        if (target > -1) {
            swapLight(index, target);
            //rotation = rotation + (UnityEngine.Random.Range(0, 2) * 2 - 1);
            int nextLevel = getLevel() + 1;
            tweenToPosition(5f, getRadius(nextLevel), (Light light) => {
                LightManager lightManager = light.GetComponent<LightManager>();
                lightManager.toggleCord(false);
                lightManager.toggleSphere(true);
            });
            StartCoroutine(ScheduleAction(5f, () => {
                shootLightAt(findFreeIndex(), 5f, nextLevel);
                drawPath(nextLevel);
            }));
        }
        else {
            Debug.Log("Destroy " + index);
            Destroy(_lights[index].gameObject);
            _lights[index] = null;
        }
    }

    void drawPath(int level)
    {
        var pathGO = GameObject.Find("path");
        if (pathGO) {
            var pathDrawer = pathGO.GetComponent<PathDrawer>();
            if (pathDrawer) {
                //pathDrawer.clearPath();
                //pathDrawer.drawPath(getRadius(level));
            }
        }
    }

    void swapLight(int index, int target)
    {
        var light = _lights[index];
        _lights[target] = light;
        LightManager lightManager = light.GetComponent<LightManager>();
        lightManager.index = target;
        _lights[index] = null;
    }

    bool allLights()
    {
        for (var i=0; i<_lights.Length; i++) {
            if (_lights[i] == null) {
                return false;
            }
        }
        return true;
    }

    void finish()
    {
        tweenToPosition(5f, 450, (Light light) => {
            LightManager lightManager = light.GetComponent<LightManager>();
            lightManager.toggleCord(false);
            lightManager.toggleSphere(false);
            lightManager.toggleBars(true);
        });
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

    private void tweenToPosition(float dur, float radius, System.Action<Light> callback) {
        for (var i=0; i<_lights.Length; i++) {
            var light = _lights[i];
            if (!light) continue;
            LightManager lightManager = light.GetComponent<LightManager>();
            lightManager.toggleCord(true);
            lightManager.toggleSphere(false);
            var to = getPosition(i, radius);
            var tween = new LocalPositionTween {
                to = to,
                duration = dur,
                onEnd = (instance) => {
                    callback(light);           
                }
            };
            light.gameObject.AddTween(tween);
        }
    }

    int getMoveIndex(int index)
    {
        var i = (index + 3) % 7;
        if (_lights[i] == null) {
            return i;
        }
        return -1;
    }

    int findFreeIndex()
    {
        int i = -1;
        if (_lights.Length > 0) {
            do {
                i = UnityEngine.Random.Range(0, _lights.Length);
            }
            while (_lights[i] != null);
        }
        return i;
    }

    void shootLightAt(int index, float dur, int? level = null)
    {
        var pos = getLevelPosition(index, level);
        var laserShooter = eye.GetComponentInChildren<LaserShooter>();
        if (laserShooter) {
            laserShooter.shoot(pos, dur);
        }
        StartCoroutine(ScheduleAction(dur - 0.5f, () => {
            _lights[index] = createLight(index, _color, pos);
        }));
    }

    IEnumerator ScheduleAction(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}
