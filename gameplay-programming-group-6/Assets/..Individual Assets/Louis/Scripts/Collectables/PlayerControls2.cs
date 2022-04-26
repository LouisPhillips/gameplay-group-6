using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using PathCreation;

public class PlayerControls2 : MonoBehaviour
{
    PlayerControls controls;
    private Vector2 movement;

    public Camera camera;
    public GameObject target;

    public SpeedBoost speedBoost;
    public JumpBoost jumpBoost;
    public shrinkBoost shrinkBoost;
    public ShieldBoost shieldBoost;
    public SplineSwitch spline;

    public float health = 10;
    public float speed = 0;
    public float splineSpeed = 3;
    private bool isRunning = false;

    private Vector3 inputDirection;
    private Quaternion rotation;
    private Vector3 movingVector;
    private Vector3 playerDirection;

    public float jumpHeight = 100f;
    public float gravity = 12f;
    private bool grouded;

    private Vector3 groundCheck = Vector3.down;
    private Vector3 aboveCheck = Vector3.up;

    public float range = 5f;
    Ray groundRaycast;
    Ray aboveRaycast;

    public SlimeScript slimeHealth;
    public float attackRange = 4f;
    private bool canDamage;
    RaycastHit hit5;

    public GameObject buttonReminder;
    public GameObject pauseMenu;

    public CinemachineVirtualCamera lookAtCam;
    public CinemachineFreeLook thirdPersonCamera;
    public CinemachineVirtualCamera splineCam;
    private Transform cameraTransform;
    public Transform lookAtObject;
    private bool lockedOn = false;
    private bool lockingOn = false;

    bool snapping_left = false;

    private bool rolling = false;
    public bool canResize = false;


    public bool canSpeedBoost = false;
    public bool canJumpBoost = false;
    public bool canShrinkBoost = false;
    public bool canShieldBoost = false;
    private bool canAttack = true;
    public bool canPressButton = false;
    public bool pressedButton = false;
    public bool canPlatformButton = false;
    public bool platButtonPressed = false;

    public Transform button;
    public Button button_trigger;
    public GameObject actualButton;
    public GameObject door;
    public Camera cutsceneCam;
    public GameObject trigger_box_button;
    private bool runAnim;

    private float time = 0f;
    private float timeRoll = 0f;
    private float resize = 0f;
    private float attackDelay = 0f;
    private float buttonDelay = 0f;

    private bool falling;
    private bool jumpPressed;

    private bool doubleJump;
    public bool takeNoDamage;

    public Transform particleSpeed;
    public Transform particleJump;
    public Transform particleShrink;

    public bool takingDamage;
    public bool dead;

    public GameObject playerTexture;

    public PathCreator path;
    public EndOfPathInstruction end;
    float distance;
    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Movement.performed += context => movement = context.ReadValue<Vector2>();

        controls.Player.Movement.canceled += context => ZeroMovement();

        controls.Player.Sprint.performed += context => isRunning = true;

        controls.Player.Jump.performed += context => OnJumpPressed();

        controls.Player.Roll.performed += context => OnRollPressed();

        controls.Player.Attack.performed += context => Attacking();

        controls.Player.Menu.performed += context => PausePressed();

        controls.Player.Menu.canceled += context => Unpaused();

        controls.Player.LockOn.performed += context => lockingOn = true;

        controls.Player.LockOn.canceled += context => lockingOn = false;
        controls.Player.LockOn.canceled += context => lockedOn = false;
       
        controls.Player.PivotRight.performed += context => thirdPersonCamera.m_XAxis.Value -= 90;
        controls.Player.PivotLeft.performed += context => thirdPersonCamera.m_XAxis.Value += 90;

        lookAtCam.enabled = false;
        cutsceneCam.enabled = false;
        splineCam.enabled = false;

        cameraTransform = Camera.main.transform;

        particleSpeed.GetComponent<ParticleSystem>().enableEmission = false;
        particleJump.GetComponent<ParticleSystem>().enableEmission = false;
        particleShrink.GetComponent<ParticleSystem>().enableEmission = false;

        /*p_Rigidbody.GetComponent<Rigidbody>();*/
    

        pauseMenu.SetActive(false);
        buttonReminder.SetActive(false);
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    void FixedUpdate()
    {
        float x = movement.x;
        float y = movement.y;

        Vector3 input = new Vector3(x, 0, y);

        inputDirection = Vector3.Lerp(inputDirection, input, Time.deltaTime * 10f);

        /// remove \/  \/  \/
        /// 
        Vector3 forwardCamera = Camera.main.transform.forward;
        Vector3 rightCamera = Camera.main.transform.right;

        rightCamera.y = 0f;
        forwardCamera.y = 0f;

        playerDirection = forwardCamera * inputDirection.z + rightCamera * inputDirection.x;

        Move(playerDirection);
        Turn(playerDirection);
    }

    void Update()
    {

        // Walk -> Sprint 
        if (!lockedOn /*&& !spline.InSpline*/)
        {
            if ((playerDirection.x > 0.1 || playerDirection.x < -0.1) || (playerDirection.z > 0.1 || playerDirection.z < -0.1))
            {
                GetComponent<Animator>().SetBool("Forward", true);
                if (isRunning)
                {
                    time += Time.deltaTime;
                    if (time < 5)
                    {
                        GetComponent<Animator>().SetBool("Sprinting", true);
                    }
                    else
                    {
                        GetComponent<Animator>().SetBool("Sprinting", false);
                        isRunning = false;
                        time = 0f;
                    }
                }
            }
            else
            {
                GetComponent<Animator>().SetBool("Forward", false);
                GetComponent<Animator>().SetBool("Sprinting", false);
                time = 0f;
                isRunning = false;
            }
        }

        groundRaycast = new Ray(transform.position, transform.TransformDirection(groundCheck * range));
        Debug.DrawRay(transform.position, transform.TransformDirection(groundCheck * range));

        if (Physics.Raycast(groundRaycast, out RaycastHit hit2, range))
        {
            if (hit2.collider.gameObject.tag == "Ground")
            {
                falling = false;
            }
        }
        else
        {
            falling = true;
            
        }

        if (falling)
        {
            GetComponent<Animator>().SetBool("Falling", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("Falling", false);
        }

        if(jumpPressed)
        {
            speed = 5; 
        }
        else
        {
            speed = 2;
        }

        aboveRaycast = new Ray(transform.position, transform.TransformDirection(aboveCheck * 3));
        Debug.DrawRay(transform.position, transform.TransformDirection(aboveCheck * 3));

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

        

        RaycastHit hit4;
        //Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 8, Color.red);

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit4, Mathf.Infinity))
        {
            if (hit4.transform.gameObject.tag == "Lockable")
            {
                if (lockingOn)
                {
                    lockedOn = true;
                    if (lockedOn)
                    {
                        thirdPersonCamera.enabled = false;
                        lookAtCam.enabled = true;
                        splineCam.enabled = false;
                        lookAtCam.m_LookAt = hit4.transform;
                        GetComponent<Animator>().SetBool("Forward", false);
                        GetComponent<Animator>().SetBool("Sprinting", false);
                        lookAtObject = hit4.transform;
                    }
                    if (snapping_left)
                    {
                        lookAtCam.transform.RotateAround(hit4.transform.position, Vector3.up, 20 * Time.deltaTime);
                    }
                }
            }
        }
        //Debug.Log("X  " + movement.x + "   Y  " + movement.y);

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), transform.forward * attackRange, Color.green);

        if(Physics.Raycast(new Vector3 (transform.position.x, transform.position.y + 0.1f, transform.position.z), transform.forward, out hit5, attackRange))
        {
            if (hit5.transform.gameObject.tag == "Lockable")
            {
                canDamage = true;
                
            }
        }
        else
        {
            canDamage = false;
        }

        if (!lockedOn)
        {
            thirdPersonCamera.enabled = true;
            lookAtCam.enabled = false;
            splineCam.enabled = false;
            lookAtCam.m_LookAt = null;
        }

        if (lockedOn)
        {
            transform.LookAt(lookAtObject);
            
            
            if (movement.x > 0.2)
            {
                 //Debug.Log("RIGHT");
                 GetComponent<Animator>().SetBool("StrafeRight", true);
            }
            else if (movement.x < -0.2)
            {
                //Debug.Log("LEFT");
                GetComponent<Animator>().SetBool("StrafeLeft", true);
            }
            else if (movement.y > 0.2)
            {
                //Debug.Log("FORWARD");
                GetComponent<Animator>().SetBool("StrafeForward", true);
            }
            else if (movement.y < -0.2)
            {
                //Debug.Log("BACKWARDS");
                GetComponent<Animator>().SetBool("StrafeBack", true);
            }
            else if (movement.x > 0.1 && movement.y > 0.1)
            {
                //Debug.Log("FORWARD RIGHT");
                GetComponent<Animator>().SetBool("StrafeForwardRight", true);
            }
            else if (movement.x < 0.1 || movement.y < 0.1)
            {
                GetComponent<Animator>().SetBool("StrafeLeft", false);
                GetComponent<Animator>().SetBool("StrafeRight", false);
                GetComponent<Animator>().SetBool("StrafeForward", false);
                GetComponent<Animator>().SetBool("StrafeBack", false);
                GetComponent<Animator>().SetBool("StrafeForwardLeft", false);
                GetComponent<Animator>().SetBool("StrafeBackLeft", false);
                GetComponent<Animator>().SetBool("StrafeForwardRight", false);
                GetComponent<Animator>().SetBool("StrafeBackRight", false);
            }
        }

        if (button_trigger.inside)
        {
            if (pressedButton)
            {
                runAnim = true;
            }
        }

        if(runAnim)
        {
            buttonDelay += Time.deltaTime;
            if (buttonDelay < 2)
            {
                transform.LookAt(button);
                transform.position = new Vector3(button.position.x, button.position.y, button.position.z + 1.3f);
                actualButton.GetComponent<Animator>().SetTrigger("Pressing");
                movement = Vector2.zero;
                Destroy(trigger_box_button);
                buttonReminder.SetActive(false);
                canPressButton = false;
            }
            else if (buttonDelay > 2 && buttonDelay < 6)
            {
                actualButton.GetComponent<Animator>().enabled = false;
                door.GetComponent<Animator>().SetTrigger("DoorOpen");
                movement = Vector2.zero;
                cutsceneCam.enabled = true;
                thirdPersonCamera.enabled = false;
                button_trigger.CutSceneAnimation.SetTrigger("Start");
            }
            else if (buttonDelay > 6 && buttonDelay < 12)
            {
                cutsceneCam.enabled = false;
                thirdPersonCamera.enabled = true;
                button_trigger.CutSceneAnimation.GetComponent<Animator>().enabled = false;
                //button_trigger.CutSceneAnimation.SetBool("Played", false);
            }
            else if (buttonDelay > 12)
            {
                pressedButton = false;
                runAnim = false;
                door.GetComponent<Animator>().enabled = false;
            }
        }


        if (spline.InSpline)
        {
            //Debug.Log(distance);
            splineCam.enabled = true;
            jumpHeight =7;
            thirdPersonCamera.enabled = false;
            GetComponent<Animator>().applyRootMotion = false;
            if(spline.facingX)
            {

                if (movement.x > 0.1 && !jumpPressed)
                {
                    transform.position = new Vector3(path.path.GetPointAtDistance(distance += splineSpeed * Time.deltaTime, end).x, transform.position.y, path.path.GetPointAtDistance(distance += splineSpeed * Time.deltaTime, end).z);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    GetComponent<Animator>().SetBool("Forward", true);
                }
                if (movement.x < -0.1 && !jumpPressed)
                {
                    transform.position = new Vector3(path.path.GetPointAtDistance(distance -= splineSpeed * Time.deltaTime, end).x, transform.position.y, path.path.GetPointAtDistance(distance -= splineSpeed * Time.deltaTime, end).z);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    GetComponent<Animator>().SetBool("Forward", true);
                }
                if (movement.y > 0.1 || movement.y < -0.1 && !jumpPressed)
                {
                    movement = Vector2.zero;
                    GetComponent<Animator>().SetBool("Forward", false);
                }
            }
            else if(!spline.facingX)
            {
                if (movement.y > 0.1 && !jumpPressed)
                {
                    //transform.position = path.path.GetPointAtDistance(distance += splineSpeed * Time.deltaTime, end);
                    transform.position = new Vector3(path.path.GetPointAtDistance(distance += splineSpeed * Time.deltaTime, end).x, transform.position.y, path.path.GetPointAtDistance(distance += splineSpeed * Time.deltaTime, end).z);
                    //transform.rotation = Quaternion.Euler(0, 0, 0);
                    GetComponent<Animator>().SetBool("Forward", true);
                }
                else if (movement.y < -0.1 && !jumpPressed)
                {
                    transform.position = new Vector3 (path.path.GetPointAtDistance(distance -= splineSpeed * Time.deltaTime, end).x, transform.position.y, path.path.GetPointAtDistance(distance -= splineSpeed * Time.deltaTime, end).z);
                    //transform.rotation = Quaternion.Euler(0, 180, 0);
                    GetComponent<Animator>().SetBool("Forward", true);
                }
                else if (movement.x > 0.1 || movement.x < -0.1)
                {
                    GetComponent<Animator>().SetBool("Forward", false);
                }
            }
            
            if(isRunning)
            {
                splineSpeed = 5;
            }
            else
            {
                splineSpeed = 2;
            }

            if (distance > path.path.length)
            {
                Debug.Log("over");
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f);
            }

            if(distance < path.path.length - path.path.length)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
            }

            if(jumpPressed && movement.x > 0.1)
            {
                    transform.position = new Vector3(path.path.GetPointAtDistance(distance += splineSpeed * Time.deltaTime, end).x
                    , transform.position.y , path.path.GetPointAtDistance(distance += splineSpeed * Time.deltaTime, end).z);
            }

            else if (jumpPressed && movement.x < -0.1)
            {
                transform.position = new Vector3(path.path.GetPointAtDistance(distance -= splineSpeed * Time.deltaTime, end).x
                    , transform.position.y, path.path.GetPointAtDistance(distance -= splineSpeed * Time.deltaTime, end).z);
            }
            
            Debug.Log("pos : " + distance + "       length : " + path.path.length);
        }
  
        if(!spline.InSpline)
        {
            jumpHeight = 7;
            GetComponent<Animator>().applyRootMotion = true;
        }
        // Speed Boost collected particles
        if (canSpeedBoost)
        {
            Color color = new Color(0, 50, 255, 255);
            particleSpeed.GetComponent<ParticleSystem>().startColor = color;
            particleSpeed.GetComponent<ParticleSystem>().enableEmission = true;
            speed = 7;
        }
        else
        {
            particleSpeed.GetComponent<ParticleSystem>().enableEmission = false;
        }

        //Jump Boost Collected Particles
        if (canJumpBoost)
        {
            Color color = new Color(0, 255, 0, 255);
            particleJump.GetComponent<ParticleSystem>().startColor = color;
            particleJump.GetComponent<ParticleSystem>().enableEmission = true;
        }
        else
        {
            particleJump.GetComponent<ParticleSystem>().enableEmission = false;
        }

        //shrink boost
        if (canShrinkBoost)
        {
            Color color = new Color(25, 255, 0, 255);
            particleShrink.GetComponent<ParticleSystem>().startColor = color;
            particleShrink.GetComponent<ParticleSystem>().enableEmission = true;
            transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        }
        else
        {
            if (canResize)
            {
                particleShrink.GetComponent<ParticleSystem>().enableEmission = false;
                transform.localScale = new Vector3(1, 1, 1);
                shrinkBoost.resized = true;
            }
        }

        // rolling delay manager
        if (rolling)
        {
            timeRoll += Time.deltaTime;
            if (timeRoll < 2)
            {
                rolling = true;
            }
            else
            {
                rolling = false;
                timeRoll = 0f;
            }
        }

        if (!canAttack)
        {
            attackDelay += Time.deltaTime;
            if (attackDelay < 0.5)
            {
                speed = 0;
                canAttack = false;
            }
            else
            {
                speed = 2;
                canAttack = true;
                attackDelay = 0f;
            }

        }

        if(health == 0)
        {
            GetComponent<Animator>().SetTrigger("Dead");
            controls.Disable();
            health = -1;
        }
        Debug.Log(takingDamage);
    }

    void Move(Vector3 playerDirection)
    {
        if (!lockedOn/* && !spline.InSpline*/)
        {
            movingVector.Set(playerDirection.x, playerDirection.y, playerDirection.z);
            movingVector = movingVector * speed * Time.deltaTime;
            transform.position += movingVector;
        }
    }

    void ZeroMovement()
    {
        movement = Vector2.zero;
        GetComponent<Animator>().SetBool("Forward", false);
        GetComponent<Animator>().SetBool("StrafeLeft", false);
        GetComponent<Animator>().SetBool("StrafeRight", false);
        GetComponent<Animator>().SetBool("StrafeForward", false);
        GetComponent<Animator>().SetBool("StrafeBack", false);
        GetComponent<Animator>().SetBool("StrafeForwardLeft", false);
        GetComponent<Animator>().SetBool("StrafeBackLeft", false);
        GetComponent<Animator>().SetBool("StrafeForwardRight", false);
        GetComponent<Animator>().SetBool("StrafeBackRight", false);
    }
    void Turn(Vector3 playerDirection)
    {
        if (!spline.InSpline)
        {
            if ((playerDirection.x > 0.1 || playerDirection.x < -0.1) || (playerDirection.z > 0.1 || playerDirection.z < -0.1))
            {
                rotation = Quaternion.LookRotation(playerDirection);
                transform.rotation = rotation;
            }
            else
            {
                transform.rotation = rotation;
            }
        }
    }

    public void OnJumpPressed()
    {
        jumpPressed = true;
        if (grouded)
        {
            GetComponent<Animator>().SetTrigger("Jump");
            GetComponent<Rigidbody>().velocity = new Vector3(playerDirection.x, jumpHeight, playerDirection.z);
        }
        else if (doubleJump)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(playerDirection.x, jumpHeight, playerDirection.z);
            doubleJump = false;
            jumpBoost.collected = false;
        }
    }

    public void OnRollPressed()
    {
        if (!rolling)
        {
            GetComponent<Animator>().SetTrigger("Roll");
            rolling = true;
        }
    }

    public void Attacking()
    {
        //transform.LookAt(transform.position, cameraTransform.rotation * Vector3.left);
        if (canPressButton)
        {
            GetComponent<Animator>().SetTrigger("ButtonPress");
            transform.LookAt(button);
            pressedButton = true;
        }
        
        if(canPlatformButton)
        {
            platButtonPressed = true;
        }

        if (canAttack)
        {
            if (!canPressButton)
            {
                if(canDamage)
                {
                    hit5.transform.gameObject.GetComponent<SlimeScript>().health -= 1;
                    hit5.transform.gameObject.GetComponent<SlimeScript>().hit = true;
                }
                GetComponent<Animator>().SetInteger("AttackIndex", Random.Range(0, 4));
                GetComponent<Animator>().SetTrigger("Attack");
                canAttack = false;
            }
        }
    }

    public void PausePressed()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void Unpaused()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    private void OnCollisionEnter(Collision ground)
    {
        jumpPressed = false;
        if (ground.gameObject.layer == 3)
        {
            GetComponent<Animator>().SetBool("Falling", false);
            falling = false;
   
            grouded = true;

            speed = 2;
            
        }
    }

    private void OnCollisionExit(Collision ground)
    {
        
        if (ground.gameObject.layer == 3)
        {
            
            if (jumpPressed)
            {
                speed = 5;
                falling = true;
                if (falling)
                {
                    GetComponent<Animator>().SetBool("Falling", true);
                    grouded = false;
                   
                }
                if(speedBoost.collected)
                {
                    speed = 7;
                }
            }

            if (jumpBoost.collected)
            {
                doubleJump = true;
            }
        }

        
    }
}
