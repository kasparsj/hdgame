using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using Tweens;

public class FieldController : AudioManager, ILightController
{
    public Light _prefab = null;
    public int radius = 450;
    public float _intensity = 4000;
    public Color _color = new Color(0, 1, 0);
    private GameObject eye;
    private Light[] _lights = new Light[8];
    private int bars = 0;

    public AudioSource eyeSound;
    Vector3 playerPos;

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

    public void init()
    {
        var index = 0;
        _lights[index] = createLight(index, _color, new Vector3(0, 3, 0));
        startChords(index);
    }

    void Start()
    {
        //init();

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
        if (lightManager) {
            lightManager.toggleSphere(false);
        }
        switch (index) {
            case 0:
                audioSources[index].spatialBlend = 0f;
                // hh
                //FadeIn(audioSources[9]);

                startEye();
                //var delay = UnityEngine.Random.Range(15f, 30f);
                //FadeOut(audioSources[index], delay);
                //StartCoroutine(ScheduleAction(delay, () => {
                //    lightManager.toggleBars(false);
                //    startLaser();
                //}));
                break;
            default:
                toggleMelodies();
                audioSources[index].spatialBlend = 0f;
                togglePercs();
                if (lightManager) {
                    lightManager.toggleBars(true);
                    bars++;
                    if (bars >= 6) {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }

                }
                break;
        }
    }

    private void startChords(int light) {
        LightManager lightManager = _lights[light].GetComponent<LightManager>();
        lightManager.toggleCord(false);
        lightManager.toggleSphere(true);
        lightManager.toggleBars(true);

        FadeIn(audioSources[light]);
    }

    public void startLaser(float dur = 10f) {
        if (eye)
        {
            LightManager lightManager = _lights[0].GetComponent<LightManager>();
            lightManager.toggleBars(false);

            pauseEye();
            shootLightAt(0, dur);
            //shootLightAt(findFreeIndex(), dur);
            StartCoroutine(ScheduleAction(dur, () =>
            {
                // todo: next level
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                //startEye();
                //if (countFree() > 0) {
                //    var delay = UnityEngine.Random.Range(45f, 90f);
                //    StartCoroutine(ScheduleAction(delay, () => {
                //        startLaser(UnityEngine.Random.Range(3.5f, 7f));
                //    }));
                //}
            }));
        }
    }

    int findNonSpatialIndex()
    {
        int i;
        do {
            i = UnityEngine.Random.Range(1, 8);
        }
        while (audioSources[i].spatialBlend != 0f);
        return i;
    }

    int countNonSpatial()
    {
        var count = 0;
        for (var i=1; i<8; i++) {
            if (audioSources[i].spatialBlend == 0f) {
                count++;
            }
        }
        return count;
    }

    int findFreeIndex()
    {
        if (countFree() == 1) {
            return 7;
        }
        int i = -1;
        if (_lights.Length > 0 && countFree() > 0) {
            do {
                i = UnityEngine.Random.Range(1, _lights.Length-1);
            }
            while (_lights[i] != null);
        }
        return i;
    }

    int countFree()
    {
        int free = 0;
        for (var i=1; i<_lights.Length; i++) {
            if (_lights[i] == null) free++;
        }
        return free;
    }

    private void startEye() {
        eyeSound.Play();
        var hdGO = GameObject.Find("hd");
        eye = hdGO.transform.Find("eye").gameObject;
        if (eye) {
            StartCoroutine(FlashEye(30));
            var posOsc = eye.GetComponent<OscPositionX>();
            if (posOsc) {
                posOsc.startOsc();
            }
        }
        var fpCtrl = GameObject.Find("First Person Controller");
        fpCtrl.GetComponent<FirstPersonMovement>().enabled = false;
        fpCtrl.GetComponent<Jump>().enabled = false;
        fpCtrl.GetComponent<Crouch>().enabled = false;
        fpCtrl.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        var pos = fpCtrl.transform.position;
        var tween = new PositionTween
        {
            delay = 5,
            to = new Vector3(pos.x, pos.y + 200, pos.z),
            duration = 90f,
            onStart = (instance) => {
                LightManager lightManager = _lights[0].GetComponent<LightManager>();
                lightManager.toggleSphere(false);
                lightManager.toggleBars(false);

                for (int i = 0; i < 7; i++)
                {
                    var pos = getPosition(i, radius);
                    var light = createLight(i, _color, pos);
                    lightManager = light.GetComponent<LightManager>();
                    lightManager.toggleCord(false);
                    lightManager.toggleSphere(false);
                    lightManager.toggleBars(true);
                }
            },
            onUpdate = (instance, value) => {
                playerPos = value;
            },
        };
        fpCtrl.AddTween(tween);
    }

    private void pauseEye()
    {
        var posOsc = eye.GetComponent<OscPositionX>();
        if (posOsc) {
            posOsc.stopOsc();
        }
    }

    IEnumerator FlashEye(int numTimes)
    {
        var hdGO = GameObject.Find("hd");
        eye = hdGO.transform.Find("eye").gameObject;
        var i = 0;

        while (true)
        {
            eye.SetActive(!eye.activeSelf);
            if (i >= numTimes && eye.activeSelf) {
                yield break;
            }
            i++;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.25f));
        }
    }

    void shootLightAt(int index, float dur)
    {
        //var pos = getPosition(index, radius);
        //var pos = GameObject.Find("First Person Controller").transform.position;
        //var pos = Camera.main.transform.position;
        var pos = playerPos;
        pos = new Vector3(pos.x, pos.y, pos.z);
        var laserShooter = eye.GetComponentInChildren<LaserShooter>();
        if (laserShooter) {
            laserShooter.shoot(pos, dur);
        }
        //StartCoroutine(ScheduleAction(dur - 1f, () => {
        //    var light = createLight(index, _color, pos);
        //    _lights[index] = light;
        //    var nextSource = audioSources[index];
        //    nextSource.gameObject.transform.position = light.gameObject.transform.position;
        //    FadeIn(nextSource);
        //}));
    }

    void toggleMelodies()
    {
        if (countNonSpatial() == 0) {
            return;
        }
        var numPlaying = 0;
        for (var i=1; i<8; i++) {
            if (audioSources[i].spatialBlend == 0f && audioSources[i].volume > 0f) {
                numPlaying++;
            }
        }
        var index = findNonSpatialIndex();
        if (numPlaying == 0) {
            FadeIn(audioSources[index], 3f);
        }
        else if (numPlaying >= 2) {
            FadeOut(audioSources[index], 3f);
        }
        else {
            if (audioSources[index].volume > 0f) {
                FadeOut(audioSources[index], 3f);
            }
            else {
                FadeIn(audioSources[index], 3f);
            }
        }
    }

    void togglePercs()
    {
        if (countNonSpatial() <= 2) {
            var rnd = UnityEngine.Random.Range(0, 2) == 1 ? 10 : 12;
            if (audioSources[rnd].isPlaying) {
                rnd = rnd == 10 ? 12 : 10;
            }
            FadeIn(audioSources[rnd]);
        }
        else {
            var numPlaying = 0;
            for (var i=10; i<13; i++) {
                if (audioSources[i].volume > 0f) {
                    numPlaying++;
                }
            }
            var index = UnityEngine.Random.Range(10, 13);
            var percSource = audioSources[index];
            if (numPlaying == 0) {
                FadeIn(percSource);
            }
            else if (numPlaying == 3) {
                FadeOut(percSource);
            }
            else {
                if (percSource.volume == 0f) {
                    FadeIn(percSource);
                }
                else {
                    FadeOut(percSource);
                }
            }
        }
    }

    IEnumerator ScheduleAction(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}
