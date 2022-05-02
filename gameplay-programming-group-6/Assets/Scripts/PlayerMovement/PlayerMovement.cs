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
    private int lastHealth = 20;
    private bool hit = false; 
    private float hitTimer;
    private float regenTimer;
    private bool regenerating; 
    private float speed = 5;
    public float normalSpeed;
    public float highSpeed;
    public float sprintSpeed;
    public float jumpPower;
    public float turnSpeed;
    public float groundCheckLength;
    private bool sliding = false;
    public PhysicMaterial physMat;
    private Renderer renderer;
    public bool lockOn; 
    public Vector3 respawnPoint;


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
    private ParticleSystem regenSys;
    private ParticleSystem jumpSys;
    private ParticleSystem speedSys; 

    /// Jump
    private bool jumpPressed = false;
    private bool grounded;
    private bool slidingGrounded;
    private bool falling;
    private bool doubleJump = false;

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
        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case "RPG-Character-Mesh": 
                renderer = child.GetComponent<Renderer>();
                    break;
                case "SpeedEffect":
                    speedSys = child.GetComponent<ParticleSystem>();
                    speedSys.Stop(); 
                    break;
                case "JumpEffect":
                    jumpSys = child.GetComponent<ParticleSystem>();
                    jumpSys.Stop();
                    break;
                case "RegenEffect":
                    regenSys = child.GetComponent<ParticleSystem>();
                    regenSys.Stop();
                    break;

            }
        }
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
        if (stickDirection.y > 0.1)
        {

        }
        RaycastHit hit;

        slidingGrounded = Physics.Raycast(transform.position + transform.up, -transform.up, out hit, 1.5f);
        if (Physics.Raycast(transform.position + transform.up / 10, -transform.up, out hit, .2f))
        {
            grounded = true;
            falling = false;

            if (canJumpBoost)
            {
                doubleJump = true;

                if (jumpSys.isStopped)
                {
                    jumpSys.Play(); 
                }
            }
            else if (jumpSys.isPlaying)
            {
                jumpSys.Stop(); 
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
            physMat.staticFriction = 0;
            physMat.dynamicFriction = 0;
            physMat.bounciness = 0;
            physMat.frictionCombine = PhysicMaterialCombine.Minimum;
            physMat.bounceCombine = PhysicMaterialCombine.Minimum;

            if (sliding == false)
            {
                GetComponent<Rigidbody>().AddForce((transform.forward - transform.up) * 5, ForceMode.Impulse);
            }

            sliding = true;
           
        }
        else if (hit.transform.tag == "EndSlide")
        {
            sliding = false;
            physMat.staticFriction = 0.6f;
            physMat.dynamicFriction = 0.6f;
            physMat.bounciness = 0;
            physMat.frictionCombine = PhysicMaterialCombine.Average;
            physMat.bounceCombine = PhysicMaterialCombine.Average;

        }
        if (!slidingGrounded && GetComponent<Rigidbody>().velocity.y < -10 && !takeNoDamage)
        {
            health += -1;
        }

    }

    private void Update()
    {
        Debug.Log(health); 
        healthLoss();
        if (lockOn)
        {
            transform.Translate(stickDirection.x * speed * Time.deltaTime, 0, stickDirection.y * speed * Time.deltaTime);
        }
        else {
            if ((Mathf.Abs(stickDirection.x) > 0.1f || Mathf.Abs(stickDirection.y) > 0.1f) && !sliding)
            {
                Vector3 movement = new Vector3(stickDirection.x, 0, stickDirection.y);
                movement = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * movement;
                Quaternion yTemp = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), turnSpeed);
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, yTemp.eulerAngles.y, transform.rotation.eulerAngles.z);
                transform.position += (transform.forward * movement.magnitude * speed * Time.deltaTime);
            }
            else if (sliding && Mathf.Abs(stickDirection.x) > 0.1f)
            {
                Vector3 movement = new Vector3(stickDirection.x, 0, stickDirection.y);
                movement = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * movement;
                transform.position += new Vector3(movement.x * Time.deltaTime * speed, 0, 0);
            }
        }

        if (!canSpeedBoost && speedSys.isPlaying)
        {
            speedSys.Stop(); 
        }

        if (canSpeedBoost)
        {
            if (speedSys.isStopped)
            {
                speedSys.Play(); 
            }
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
        // ray cast must stay in update as raycast must check every frame for shrunk collider 
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

        if(health <= 0)
        {
            transform.position = respawnPoint;
            health = 20;
        }
    }

    private void Jump()
    {
        jumpPressed = true;
        if (grounded || (slidingGrounded && sliding))
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
        else if (doubleJump && !grounded)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            doubleJump = false;
            canJumpBoost = false;
        }
    }

    private void healthLoss()
    {
        if (lastHealth != health)
        {
            renderer.material.SetColor("_Color", Color.red);
            hit = true;
            regenTimer = 0; 
        }
        if (hit)
        {
            hitTimer += Time.deltaTime;
            takeNoDamage = true;
        }
        else if (health < 20)
        {
            regenTimer += Time.deltaTime; 
        }
        if (hitTimer > 0.5f)
        {
            hitTimer = 0;
            takeNoDamage = false;
            hit = false; 
            renderer.material.SetColor("_Color", Color.white);
        }
        if (regenTimer > 10)
        {
            regenerating = true; 
        }

        if (regenerating)
        {
            regenTimer += Time.deltaTime; 
            if (regenTimer > 1f)
            {
                health += 1;
                regenTimer = 0; 
            }
            if (health == 20)
            {
                regenTimer = 0;
                regenerating = false; 
            }

            if (regenSys.isStopped)
            {
                regenSys.Play(); 
            }
        }
        else if (regenSys.isPlaying)
        {
            regenSys.Stop(); 
        }

        lastHealth = health; 
    }
}