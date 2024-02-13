using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    void Start()
    {        
    }

    void Update()
    {        
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform sphereTransform = transform.parent.Find("Sphere");
        var isSphere = sphereTransform != null && sphereTransform.gameObject.activeSelf;
        if (isSphere && other.CompareTag("Player"))
        {
            GetComponentInParent<LightManager>().ParentOnTriggerEnter(other);
        }
    }
}
