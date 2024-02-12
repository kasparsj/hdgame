using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GetComponent<MeshRenderer>().bounds.size);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
