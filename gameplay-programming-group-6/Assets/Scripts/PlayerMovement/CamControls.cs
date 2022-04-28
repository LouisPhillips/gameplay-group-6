using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControls : MonoBehaviour
{
    PlayerControls controls;
    Vector2 camStickDirection;

    public Transform target; 
    public float camSpeed; 

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Look.performed += context => camStickDirection = context.ReadValue<Vector2>();
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
        Vector2 c = camStickDirection * camSpeed * Time.deltaTime;
        transform.LookAt(target); 

        transform.RotateAround(target.transform.position, transform.up, c.x);
        transform.RotateAround(target.transform.position, transform.right, c.y);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);  
    }
}
