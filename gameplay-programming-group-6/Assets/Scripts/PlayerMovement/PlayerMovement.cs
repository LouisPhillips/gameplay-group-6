using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public float speed;
    public float jumpPower;
    public float groundCheckLength;

    PlayerControls controls;
    Vector2 stickDirection;
    Animator anim;
    HashIDs hash;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += context => stickDirection = context.ReadValue<Vector2>();
        controls.Player.Move.canceled += context => stickDirection = Vector2.zero;

        controls.Player.Jump.performed += context => Jump();

        anim = GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<HashIDs>();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(stickDirection.x) > 0.1f || Mathf.Abs(stickDirection.y) > 0.1f)
        {
            Vector2 m = new Vector2(stickDirection.x, stickDirection.y) * Time.deltaTime * speed;
            transform.position += new Vector3(m.x, 0, m.y);
        }

    }

    private void Jump()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
}