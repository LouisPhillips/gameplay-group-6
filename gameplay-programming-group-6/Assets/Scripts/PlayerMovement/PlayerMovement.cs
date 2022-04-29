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

    private void Update()
    {
        if (canSpeedBoost)
        {
            Color color = new Color(0, 50, 255, 255);
            speed = 8;
        }
        else
        {
            speed = 5;
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
        if(grounded)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
        else if (doubleJump)
        {
            Debug.Log("double jump");
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            doubleJump = false;
            jumpBoost.collected = false;
        }
        
    }

    private void OnCollisionEnter(Collision ground)
    {
        jumpPressed = false;
        if (ground.gameObject.tag == "Ground")
        {
            //GetComponent<Animator>().SetBool("Falling", false);
            falling = false;

            grounded = true;

            speed = 2;

        }
    }

    private void OnCollisionExit(Collision ground)
    {

        if (ground.gameObject.tag == "Ground")
        {
            if (jumpPressed)
            {
                falling = true;
                if (falling)
                {
                    //GetComponent<Animator>().SetBool("Falling", true);
                    grounded = false;

                }
            }

            if (jumpBoost.collected)
            {
                doubleJump = true;
            }
        }


    }
}