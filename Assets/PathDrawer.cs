using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class PathDrawer : MonoBehaviour
{
    public GameObject prefab;
    public float width = 0.5f;
    public float height = 0.001f;
    public int radius = 450;
    public float shortenBy = 1/25f;
    public float rotationFix = 0.75f;
    private List<GameObject> cubes = new List<GameObject>();

    private Vector3 pointOnEllipse(float rad, float w, float h) {
        return new Vector3(math.cos(rad) * w / 2f, 0, math.sin(rad) * h / 2f);
    }

    void Start()
    {
        drawPath(radius);
    }

    public void clearPath() {
        foreach (GameObject cube in cubes) {
            Destroy(cube);
        }
        cubes.Clear();
    }

    public void drawPath(float radius) {
        for (var i=0; i<7; i++) {
            float angle = Mathf.PI * 2f / 7f;
            var from = pointOnEllipse(angle*(i + rotationFix), radius, radius);
            var to = pointOnEllipse(angle*((i+3)%7 + rotationFix), radius, radius);
            cubes.Add(DrawCubeBetweenPoints(from, to, radius));
        }
    }

    GameObject DrawCubeBetweenPoints(Vector3 from, Vector3 to, float radius)
    {
        Vector3 center = (from + to) / 2f;
        GameObject cube = Instantiate(prefab);
        Vector3 scale = new Vector3(width, height, radius - radius * shortenBy);
        cube.transform.localPosition = center;
        cube.transform.localScale = scale;
        cube.transform.rotation = Quaternion.LookRotation(to - from);
        //cube.transform.Rotate(90f, 0f, 0f);
        return cube;
    }
}
