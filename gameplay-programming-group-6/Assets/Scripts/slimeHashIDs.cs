using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimeHashIDs : MonoBehaviour
{

    public int slimeIdleState;
    public int slimeLockOnState;
    public int slimeMovingState;


    private void Awake()
    {
        slimeIdleState = Animator.StringToHash("SlimeIdle");
        slimeLockOnState = Animator.StringToHash("SlimeLockOn");
        slimeMovingState = Animator.StringToHash("SlimeMove");
    }
}
