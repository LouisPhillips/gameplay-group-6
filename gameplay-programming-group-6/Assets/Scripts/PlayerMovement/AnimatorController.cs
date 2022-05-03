
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class AnimatorController : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    int WalkingHash;
    int RunningHash;
    int AttackHash;
    int JumpHash;
    int DanceHash;
    int InAirHash; 
    public PlayerControls controls;
    Vector2 stickDirection;
    bool movementPressed;
    bool runPressed;
    bool attackPressed;
    bool jumpPressed; 
    bool canAttack;
    
    PlayerMovement script; 


    private void Awake()
    {
        controls = new PlayerControls();
        //moving the joystick

        controls.Player.Move.performed += context =>
        {
            stickDirection = context.ReadValue<Vector2>();
            movementPressed = stickDirection.x >= 0.2 || stickDirection.y >= 0.2 || stickDirection.x <= -0.2 || stickDirection.y <= -0.2;
        }; 
        //releasing the joystick
        controls.Player.Move.canceled += context =>
        {
            stickDirection = new Vector2(0, 0);
            movementPressed = false; 
        }; 

        controls.Player.Sprint.performed += context => runPressed = context.ReadValueAsButton();
        controls.Player.Attack.performed += context => attackPressed = context.ReadValueAsButton();
        controls.Player.Attack.canceled += context => attackPressed = context.ReadValueAsButton();
        controls.Player.Sprint.canceled += context => runPressed = context.ReadValueAsButton();
        controls.Player.Attack.canceled += context => canAttack = true;
        controls.Player.Jump.performed += context => jumpPressed = context.ReadValueAsButton();
        controls.Player.Jump.canceled += context => jumpPressed = context.ReadValueAsButton();


        animator = GetComponent<Animator>();
        script = GetComponent<PlayerMovement>();
        WalkingHash = Animator.StringToHash("WalkingState");
        RunningHash = Animator.StringToHash("RunningState");
        AttackHash = Animator.StringToHash("Attacking");
        JumpHash = Animator.StringToHash("Jump");
        InAirHash = Animator.StringToHash("InAir");
        DanceHash = Animator.StringToHash("Dancing");
 
    }

    private void Update()
    {
        handleMovement();
    }
    void handleMovement()
    {
        bool isWalking = animator.GetBool(WalkingHash);
        bool isRunning = animator.GetBool(RunningHash);
        bool isAttacking = animator.GetBool(AttackHash);

        canAttack = !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        if (!script.sliding)
        {
            animator.SetBool(WalkingHash, movementPressed);
            animator.SetBool(RunningHash, runPressed && movementPressed);
        }
        else 
        { 
            animator.SetBool(WalkingHash, false);  
        }
        animator.SetBool(AttackHash, attackPressed && canAttack);
        animator.SetBool(JumpHash, (script.slidingGrounded && GetComponent<Rigidbody>().velocity.y > 0.1f));
        animator.SetBool(InAirHash, (!script.grounded));

    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
}