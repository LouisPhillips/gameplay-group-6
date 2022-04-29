using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    PlayerControls controls;
    Vector2 stickDirection;
    Animator anim;
    HashIDs hash;
    Camera cam; 

    /// Data
    public int health = 20;
    private float speed = 5;
    public float normalSpeed;
    public float highSpeed;
    public float jumpPower;
    public float turnSpeed; 
    public float groundCheckLength;


    /// Power ups
    public SpeedBoost speedBoost;
    public JumpBoost jumpBoost;
    public shrinkBoost shrinkBoost;
    public ShieldBoost shieldBoost;
    public bool canSpeedBoost = false;
    public bool canJumpBoost = false;
    public bool canShrinkBoost = false;
    public bool canShieldBoost = false;
    private Vector3 aboveCheck = Vector3.up;
    public bool canResize = false;
    private float resize = 0f;

    /// Jump
    private bool jumpPressed = false;
    private bool grounded;
    private bool falling;
    private bool doubleJump;

    public bool takeNoDamage = false;
    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += context => stickDirection = context.ReadValue<Vector2>();
        controls.Player.Move.canceled += context => stickDirection = Vector2.zero;

        controls.Player.Jump.performed += context => Jump();

        anim = GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<HashIDs>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(); 
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
            Vector3 movement = new Vector3(stickDirection.x, 0, stickDirection.y); 
            movement = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * movement;
            Quaternion yTemp = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), turnSpeed);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, yTemp.eulerAngles.y, transform.rotation.eulerAngles.z);
            transform.position += (transform.forward * movement.magnitude * speed * Time.deltaTime);
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up, -transform.up, out hit, 1.1f))
        {
            grounded = true;
            falling = false;

            if (canJumpBoost)
            {
                doubleJump = true;
            }
        }
        else
        {
            grounded = false;
            falling = true;
        }

    }

    private void Update()
    {
        if (canSpeedBoost)
        {
            Color color = new Color(0, 50, 255, 255);
            speed = highSpeed;
        }
        else
        {
            speed = normalSpeed;
        }


        //shrink boost
        if (canShrinkBoost)
        {

            transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        }
        else
        {
            if (canResize)
            {
                transform.localScale = new Vector3(1, 1, 1);
                shrinkBoost.resized = true;
            }
        }
        Ray aboveRaycast = new Ray(transform.position, transform.TransformDirection(aboveCheck * 3));
        if (Physics.Raycast(aboveRaycast, out RaycastHit hit, 3))
        {
            if (hit.collider.gameObject.tag == "Shrunk")
            {
                canResize = false;
            }
        }
        else
        {
            resize += Time.deltaTime;
            if (resize > 1)
            {
                canResize = true;
                resize = 0f;
            }

        }
    }

    private void Jump()
    {
        jumpPressed = true;

        if (grounded)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
        else if (doubleJump && !grounded)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            doubleJump = false;
            jumpBoost.collected = false;
        }

    }

}