
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
    PlayerControls controls;
    Vector2 stickDirection;
    bool movementPressed;
    bool runpressed;
    bool attackpressed;

    private void Awake()
    {
        controls = new PlayerControls();
        //moving the joystick
        controls.Player.Move.performed += context =>
        {
            stickDirection = context.ReadValue<Vector2>();
            movementPressed = stickDirection.x >= 0.5 || stickDirection.y >= 0.5 || stickDirection.x <= -0.5 || stickDirection.y <= -0.5;
        };
        controls.Player.Sprint.performed += context => runpressed = context.ReadValueAsButton();
        controls.Player.Attack.performed += context => attackpressed = context.ReadValueAsButton();
        controls.Player.Attack.canceled += context => attackpressed = context.ReadValueAsButton();
        controls.Player.Sprint.canceled += context => runpressed = context.ReadValueAsButton();

        //stickDirection = context.ReadValue<Vector2>();


        //releasing the joystick
        controls.Player.Move.canceled += context => stickDirection = Vector2.zero;
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        WalkingHash = Animator.StringToHash("WalkingState");
        RunningHash = Animator.StringToHash("RunningState");
        AttackHash = Animator.StringToHash("Attacking");
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

        if (movementPressed && !isWalking)
        {
            animator.SetBool(WalkingHash, true);
        }
        if (!movementPressed && isWalking)
        {
            animator.SetBool(WalkingHash, false);
        }
        if ((movementPressed && runpressed) && !isRunning)
        {
            animator.SetBool(RunningHash, true);
        }
        if ((!movementPressed || !runpressed) && isRunning)
        {
            animator.SetBool(RunningHash, false);
        }
        if (attackpressed && !isAttacking)
        {
            animator.SetBool(AttackHash, true);
            transform.GetComponent<Attacking>().Attack();
        }
        if (!attackpressed && isAttacking)
        {
            animator.SetBool(AttackHash, false);
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
}