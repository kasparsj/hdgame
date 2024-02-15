using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tweens;

public class EyeShooter : MonoBehaviour
{
    public GameObject _prefab = null;
    public float delayMin = 1f;
    public float delayMax = 1f;

    // Start is called before the first frame update
    void Start()
    {
        ScheduleNextEvent();
    }

    void ScheduleNextEvent()
    {
        float randomTime = Random.Range(delayMin, delayMax);
        Invoke("PerformScheduledEvent", randomTime);
    }

    void PerformScheduledEvent()
    {
        var laser = Instantiate(_prefab, transform);
        laser.AddTween(new PositionTween {
            to = Camera.main.transform.position,
            duration = 3f,
            onEnd = (instance) => {
                Destroy(laser);
            }
        });
        ScheduleNextEvent();
    }

}
