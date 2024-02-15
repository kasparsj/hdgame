using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tweens;

public class LaserShooter : MonoBehaviour
{
    public GameObject _prefab = null;
    public float delayMin = 1f;
    public float delayMax = 1f;
    public float timeMin = 5f;
    public float timeMax = 10f;

    void Start()
    {
        ScheduleNextEvent();
    }

    void Update()
    {

    }

    void ScheduleNextEvent()
    {
        float randomTime = Random.Range(delayMin, delayMax);
        Invoke("PerformScheduledEvent", randomTime);
    }

    void PerformScheduledEvent()
    {
        var laser = Instantiate(_prefab, transform);
        var laserController = laser.GetComponent<LaserController>();
        if (laserController) {
            laserController.target = Camera.main.transform.position;
            laserController.target.y = 0.5f;
        }
        Destroy(laser, Random.Range(timeMin, timeMax));
        ScheduleNextEvent();
    }

}
