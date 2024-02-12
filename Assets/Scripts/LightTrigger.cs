using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script : MonoBehaviour
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
        if (other.CompareTag("Player"))
        {
            Transform siblingTransform = transform.parent.Find("Cord");
            if (siblingTransform != null) {
                siblingTransform.gameObject.SetActive(true);
            }
            GetComponentInParent<LightManager>().ParentOnTriggerEnter(other);
        }
    }
}
