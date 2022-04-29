using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControls : MonoBehaviour
{
    PlayerControls controls;
    Vector2 camStickDirection;

    public Transform target; 
    public float turnSpeed;
    public float moveSpeed;

    public float topAngle;
    public float bottomAngle; 

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Look.performed += context => camStickDirection = context.ReadValue<Vector2>();
        controls.Player.Look.canceled += context => camStickDirection = Vector2.zero; 
    }

    private void OnEnable()
    {
        controls.Player.Look.Enable(); 
    }

    private void OnDisable()
    {
        controls.Player.Look.Disable();
    }

    private void Update()
    {
        transform.position =  target.transform.position + (-transform.forward * 5);
        Vector2 c = camStickDirection * turnSpeed * Time.deltaTime;
        transform.Rotate(-c.y, c.x, 0); 
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0); 

        if (transform.eulerAngles.x > topAngle && transform.eulerAngles.x < 90)
        {
            transform.rotation = Quaternion.Euler(topAngle, transform.eulerAngles.y, 0); 
        }

        Debug.Log(transform.eulerAngles.x); 
        if (transform.eulerAngles.x < 360 - bottomAngle && transform.eulerAngles.x > 270)
        {
            transform.rotation = Quaternion.Euler(360 - bottomAngle, transform.eulerAngles.y, 0);
        }
    }
}
