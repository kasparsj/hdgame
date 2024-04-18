using UnityEngine;

public class ColliderVisibilityToggle : MonoBehaviour
{
    public Camera targetCamera; // Assign the camera in the inspector

    void Update()
    {
        // Check if the object's layer is in the camera's culling mask
        bool isVisible = (targetCamera.cullingMask & (1 << gameObject.layer)) != 0;

        // Toggle the collider based on visibility
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = isVisible;
        }
    }
}
