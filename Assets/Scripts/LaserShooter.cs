using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tweens;

public class LaserShooter : MonoBehaviour
{
    public GameObject _prefab = null;
    public bool scheduleOnStart = false;
    public float delayMin = 1f;
    public float delayMax = 1f;
    public float timeMin = 5f;
    public float timeMax = 10f;
    public GameObject target;

    void Start()
    {
        if (scheduleOnStart) {
            ScheduleNextEvent();
        }
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
        Vector3 targetPos = target.transform.position;
        if (target.CompareTag("Player") || target.CompareTag("MainCamera")) {
            targetPos.y -= 0.25f;
        }
        shoot(targetPos, Random.Range(timeMin, timeMax));
        ScheduleNextEvent();
    }

    public void shoot(Vector3 targetPos, float time)
    {
        var laser = Instantiate(_prefab, transform);
        var laserController = laser.GetComponent<LaserController>();
        if (laserController) {
            laserController.target = targetPos;
        }
        Destroy(laser, time);
    }

}
