using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    PlayerControls controls;
   
    Animator anim;
    HashIDs hash;
    Camera cam;

    /// Controls
    Vector2 stickDirection;
    bool sprint; 

    /// Data
    public int health = 20;
    private float speed = 5;
    public float normalSpeed;
    public float highSpeed;
    public float sprintSpeed; 
    public float jumpPower;
    public float turnSpeed;
    public float groundCheckLength;
    private bool sliding;
    public PhysicMaterial mat; 


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
    private bool slidingGrounded; 
    private bool falling;
    private bool doubleJump;

    public bool takeNoDamage = false;
    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += context => stickDirection = context.ReadValue<Vector2>();
        controls.Player.Move.canceled += context => stickDirection = Vector2.zero;

        controls.Player.Jump.performed += context => Jump();

        controls.Player.Sprint.performed += context => sprint = true;
        controls.Player.Sprint.canceled += context => sprint = false; 


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
        RaycastHit hit;

        slidingGrounded = Physics.Raycast(transform.position + transform.up, -transform.up, out hit, 1.5f); 
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

        Physics.Raycast(transform.position + transform.up, -transform.up, out hit, 1.5f);

        if (hit.transform.tag == "Slide")
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, hit.transform.eulerAngles.y, transform.eulerAngles.z);

            if (sliding == false)
            {
                GetComponent<Rigidbody>().AddForce((transform.forward - transform.up) * 5, ForceMode.Impulse); 
            }

          
            sliding = true; 
            mat.staticFriction = 0;
            mat.dynamicFriction = 0;
            mat.bounciness = 0;
            mat.frictionCombine = PhysicMaterialCombine.Minimum;
            mat.bounceCombine = PhysicMaterialCombine.Minimum; 
        }
        else if (hit.transform.tag == "EndSlide")
        {
            sliding = false;
            mat.staticFriction = 0.6f;
            mat.dynamicFriction = 0.6f;
            mat.bounciness = 0;
            mat.frictionCombine = PhysicMaterialCombine.Average;
            mat.bounceCombine = PhysicMaterialCombine.Average;

        }


        Ray aboveRaycast = new Ray(transform.position, transform.TransformDirection(aboveCheck * 3));
        if (Physics.Raycast(aboveRaycast, out  hit, 3))
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

    private void Update()
    {
        if ((Mathf.Abs(stickDirection.x) > 0.1f || Mathf.Abs(stickDirection.y) > 0.1f) && !sliding)
        {
            Vector3 movement = new Vector3(stickDirection.x, 0, stickDirection.y);
            movement = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * movement;
            Quaternion yTemp = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), turnSpeed);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, yTemp.eulerAngles.y, transform.rotation.eulerAngles.z);
            transform.position += (transform.forward * movement.magnitude * speed * Time.deltaTime);
        }
        else if ((sliding && !slidingGrounded) || sliding && Mathf.Abs(stickDirection.x) > 0.1f)
        {
            Vector3 movement = new Vector3(stickDirection.x, 0, stickDirection.y);
            movement = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * movement;
            transform.position += new Vector3(movement.x * Time.deltaTime * speed, 0, 0); 
        }

        if (canSpeedBoost)
        {
            Color color = new Color(0, 50, 255, 255);
            speed = highSpeed;
        }
        else if (sprint)
        {
            speed = sprintSpeed; 
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
    }

    private void Jump()
    {
        jumpPressed = true;

        if (grounded || slidingGrounded)
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