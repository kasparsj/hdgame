using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform sphereTransform = transform.parent.Find("Sphere");
        if (sphereTransform != null && sphereTransform.gameObject.activeSelf && other.CompareTag("Player"))
        {
            GetComponentInParent<LightManager>().ParentOnTriggerEnter(other);
        }
    }
}
