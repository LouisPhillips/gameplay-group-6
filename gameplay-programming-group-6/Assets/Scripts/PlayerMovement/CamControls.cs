using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControls : MonoBehaviour
{
    PlayerControls controls;
    Vector2 camStickDirection;

    public float camSpeed; 

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += context => camStickDirection = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        Vector2 c = camStickDirection * camSpeed * Time.deltaTime;
        transform.Rotate(0, c.x, c.y); 
    }
}
