using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void OnValidate()
    {
        Debug.Log(GetComponent<MeshRenderer>().bounds.size);
    }
}
