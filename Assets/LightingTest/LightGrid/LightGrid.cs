using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class LightGrid : MonoBehaviour
{
    [SerializeField] Light _prefab = null;
    [SerializeField] int _radius = 50;
    [SerializeField] int _edges = 7;
    [SerializeField] float _intensity = 1;
    [SerializeField] Color _color = new Color(1, 1, 1);

    List<Light> _lights = new List<Light>();

    Vector3 pointOnEllipse(float rad, float w, float h) {
        return new Vector3(math.cos(rad) * w / 2, math.sin(rad) * h / 2);
    }

    void Start()
    {
        for (var i = 0; i < _edges; i++)
        {
            var go = Instantiate(_prefab, transform);
            go.transform.localPosition = pointOnEllipse(Mathf.PI * 2 / 7 * i, _radius, _radius);
            go.transform.localRotation = Quaternion.identity;
            go.GetComponentInChildren<LightManager>().color = _color;
            go.GetComponentInChildren<LightManager>().intensity = _intensity;
            _lights.Add(go.GetComponent<Light>());
        }
    }

    void Update()
    {
    }

    public void ParentOnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
    }
}
