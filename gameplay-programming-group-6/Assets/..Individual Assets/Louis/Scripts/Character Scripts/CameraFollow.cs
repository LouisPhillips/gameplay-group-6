using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float timeZ;
    public float timeX;
    public float timeY;
    public float offset = -20f;
    public GameObject player;
    public Vector3 velocity;
    public Transform lookAtPoint;

    private Vector3 originalPos;

    Ray lookingAt;

    void Start()
    {
        originalPos = transform.position;
    }

    void FixedUpdate()
    {
        float x = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocity.x, timeX);
        float z = Mathf.SmoothDamp(transform.position.z, player.transform.position.z + offset, ref velocity.z, timeZ);
        float y = Mathf.SmoothDamp(transform.position.y, player.transform.position.y + 3, ref velocity.y, timeY);
        transform.position = new Vector3(x, y, z);
       // transform.LookAt(lookAtPoint.position)
    }
}
