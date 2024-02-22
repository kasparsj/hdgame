using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Tweens;

public class FieldSpatialController : AudioManager, ILightController
{
    public Light _prefab = null;
    public GameObject[] audioObjects;
    public int minRadius = 50;
    public int maxRadius = 450;
    public float _intensity = 4000;
    public Color _color = new Color(0, 1, 0);
    private GameObject eye;
    private List<Light> _lights = new List<Light>();
    private List<List<int>> combinations = new List<List<int>>{
         new List<int> { 0, 1, 2 },
         new List<int> { 3, 4, 5 },
         new List<int> { 6, 7, 8 }
    };

    private Vector3 pointOnEllipse(float rad, float w, float h) {
        return new Vector3(math.cos(rad) * w / 2f, 0, math.sin(rad) * h / 2f);
    }

    private Vector3 getPosition(int light, float radius) {
        float angle = Mathf.PI * 2f / 7f;
        // if (light < 7) {
            return pointOnEllipse((light+0.75f) * angle, radius, radius);
        // }
        // else {
            // return pointOnEllipse(light * angle, radius, radius);
        // }
    }

    public void init() {
        for (var i=0; i<combinations.Count; i++) {
            for (var j=0; j<combinations[i].Count; j++) {
                var pos = getPosition(i, 300f) + getPosition(j, 10f);
                var index = i * 3 + j;

                var go = Instantiate(_prefab, transform);
                go.transform.localPosition = pos;
                go.transform.localRotation = Quaternion.identity;

                var light = go.GetComponent<Light>();
                _lights.Add(light);

                if (audioObjects[combinations[i][j]]) {
                    var audioGO = Instantiate(audioObjects[combinations[i][j]], go.transform);
                    audioSources[index] = audioGO.GetComponent<AudioSource>();
                    Sync[index] = true;
                }

                var lightManager = go.GetComponent<LightManager>();
                if (lightManager) {
                    lightManager.index = index;
                    lightManager.color = _color;
                    lightManager.intensity = _intensity;
                    lightManager.randomSeed = (uint) UnityEngine.Random.Range(0, 100000);
                    if (audioSources[index]) {
                        lightManager.audioSource = audioSources[index];
                    }
                }
            }
        }

        startEye();
    }

    void startEye() {
        var hdGO = GameObject.Find("hd");
        eye = hdGO.transform.Find("eye").gameObject;
        if (eye) {
            eye.SetActive(true);
            StartCoroutine(ScheduleAction(1f, () => {
                // shootLightAt(findFreeIndex(), 3f, 2);
                // StartCoroutine(ScheduleAction(3f, () => {
                //     shootLightAt(findFreeIndex(), 3f, 2);
                //     StartCoroutine(ScheduleAction(7f, () => {
                //         var posOsc = eye.GetComponent<OscPositionX>();
                //         if (posOsc) {
                //             posOsc.startOsc();
                //         }
                //     }));
                // }));
            }));
        }
    }

    void Start()
    {
        init();

        base.Start();
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

        var light = _lights[index];
        LightManager lightManager = light.GetComponent<LightManager>();
        if (audioSources[index].isPlaying) {
            // lightManager.toggleBars(false);
            FadeOutPause(audioSources[index]);
        }
        else {
            // lightManager.toggleBars(true);
            FadeIn(audioSources[index]);
        }
    }

    void shootLightAt(int index, float dur, int? level = null)
    {
        var pos = _lights[index].gameObject.transform.position;
        var laserShooter = eye.GetComponentInChildren<LaserShooter>();
        if (laserShooter) {
            laserShooter.shoot(pos, dur);
        }
    }

    IEnumerator ScheduleAction(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}
