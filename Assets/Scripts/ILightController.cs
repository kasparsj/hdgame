using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILightController
{
    void LightOnTriggerEnter(int index, Collider other);
}
