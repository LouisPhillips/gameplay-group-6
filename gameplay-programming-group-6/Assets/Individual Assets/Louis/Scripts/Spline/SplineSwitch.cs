using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineSwitch : MonoBehaviour
{
    public bool InSpline;
    [Header("True for Spline going X, false for Y")]
    public bool facingX;
    [Header("Place ground of spline section below")]
    public GameObject surface;

    public Transform pointA;
    public Transform pointB;

    private void Start()
    {
        transform.localScale = new Vector3(surface.transform.localScale.x, surface.transform.localScale.y + 100, surface.transform.localScale.z);
        transform.position = new Vector3(surface.transform.position.x, transform.position.y, surface.transform.position.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        InSpline = true;
    }

    private void OnTriggerExit(Collider other)
    {
        InSpline = false;
    }
}
