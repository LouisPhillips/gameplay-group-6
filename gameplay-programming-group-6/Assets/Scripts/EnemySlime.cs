using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySlime : MonoBehaviour
{
    public bool coneHit;
    public float jumpForce;
    public float turnSpeed;
    public float slowSpeed;
    public float fastSpeed;
    public float acceleration;
    public float preWait;
    public float postWait;
    public float lookAngle; 
    public float movementDistance;
    public float nearAngle; 
    public GameObject[] PatrolArea;

    private Transform player;
    private Quaternion originalRot;
    private Vector3 movementTarget;
    private Animator anim;
    private slimeHashIDs hash;
    private bool faster;
    private float speed;
    private float timer;
    private bool returning; 
    private bool finalTurn;
    private float moveLength; 
    private RaycastHit hit;
    private bool found;
    private enum ENEMYSTATE
    {
        walking,
        looking,
        attacking,
    }
    private ENEMYSTATE enemyState = ENEMYSTATE.walking;

    private enum LOOKINGSTATE
    {
        preLook, 
        lookingLeft, 
        lookingRight,
        postLook, 
    }
    private LOOKINGSTATE lookingState; 

    private void Awake()
    {
        anim = GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<slimeHashIDs>();
        speed = slowSpeed;
        enemyState = ENEMYSTATE.looking;
        lookingState = LOOKINGSTATE.preLook; 
        foreach (Transform child in GameObject.FindGameObjectWithTag("Player").transform)
        {
            if (child.name == "CamTarget")
            {
                player = child; 
            }
        }
        if (PatrolArea[0] == null)
        {
            Physics.Raycast(transform.position + transform.up, -transform.up, out hit);
            PatrolArea[0] = hit.transform.gameObject;
        }
    
    }

    private void Update()
    {
        if (enemyState != ENEMYSTATE.attacking)
        {
            if (coneHit)
            {
                Vector3 dir = (player.transform.position - transform.position).normalized;
                dir = dir * ((Vector3.Distance(transform.position, player.transform.position)) - 3);
                if (!Physics.BoxCast(transform.position, transform.localScale / 2, player.transform.position - transform.position, transform.rotation, Vector3.Distance(transform.position, player.transform.position) - 3))
                {
                    found = true;
                    movementTarget = player.transform.position + ((transform.position - player.transform.position).normalized * 5);
                    enemyState = ENEMYSTATE.walking;
                    Quaternion lookat = Quaternion.LookRotation(movementTarget - transform.position);
                    lookat = Quaternion.Euler(lookat.eulerAngles.x, lookat.eulerAngles.y - 90, lookat.eulerAngles.z);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookat, Time.deltaTime * turnSpeed);
                }
            }
        }

        switch (enemyState)
        {
            case ENEMYSTATE.walking:
                anim.SetBool(hash.slimeMovingState, true);
                anim.SetBool(hash.slimeIdleState, false);

                if (faster)
                {
                    speed = Mathf.Lerp(speed, fastSpeed, Time.deltaTime * acceleration);
                }
                else
                {
                    speed = Mathf.Lerp(speed, slowSpeed, Time.deltaTime * acceleration);
                }

                transform.Translate(Vector3.right * speed * Time.deltaTime);

                if (Vector3.Distance(movementTarget, transform.position) < 0.5F)
                {
                    if (found)
                    {
                        enemyState = ENEMYSTATE.attacking;
                        anim.SetBool(hash.slimeMovingState, false);
                        anim.SetBool(hash.slimeLockOnState, true);
                    }
                    else
                    {
                        anim.SetBool(hash.slimeMovingState, false);
                        anim.SetBool(hash.slimeIdleState, true);
                        enemyState = ENEMYSTATE.looking;
                    }
                }
                


                break;
            case ENEMYSTATE.looking:
                Quaternion test1;
                Quaternion test2;

                switch (lookingState)
                {
                    case LOOKINGSTATE.preLook:
                        originalRot = transform.rotation;
                        timer += Time.deltaTime;
                        if (timer > preWait)
                        {
                            movementTarget = new Vector3 (Mathf.Infinity, Mathf.Infinity, Mathf.Infinity); 
                            timer = 0; 
                            if (Random.Range(0, 2) >= 1)
                            {
                                lookingState = LOOKINGSTATE.lookingLeft; 
                            }
                            else 
                            {
                                lookingState = LOOKINGSTATE.lookingRight;
                            }
                        }
                        break;
                    case LOOKINGSTATE.lookingLeft:
                        if (returning)
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, originalRot, turnSpeed * Time.deltaTime);
                        }
                        else
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                        Quaternion.Euler(originalRot.eulerAngles.x, originalRot.eulerAngles.y - lookAngle, originalRot.eulerAngles.z), turnSpeed * Time.deltaTime);
                        }

                        test1 = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                        test2 = Quaternion.Euler(originalRot.eulerAngles.x, originalRot.eulerAngles.y - lookAngle, originalRot.eulerAngles.z);
                     
                        if (Mathf.Abs(Mathf.DeltaAngle(test1.eulerAngles.y, test2.eulerAngles.y)) < nearAngle)
                        {
                            returning = true;
                        }

                        if (returning && Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, originalRot.eulerAngles.y)) < nearAngle)
                        {
                            if (finalTurn)
                            {
                                lookingState = LOOKINGSTATE.postLook;
                            }
                            else
                            {
                                lookingState = LOOKINGSTATE.lookingRight;
                                finalTurn = true;
                                returning = false;
                            }
                        }
                        break;
                    case LOOKINGSTATE.lookingRight:
                        if (returning)
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, originalRot, turnSpeed * Time.deltaTime);
                        }
                        else
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                        Quaternion.Euler(originalRot.eulerAngles.x, originalRot.eulerAngles.y + lookAngle, originalRot.eulerAngles.z), turnSpeed * Time.deltaTime);
                        }

                        test1 = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                        test2 = Quaternion.Euler(originalRot.eulerAngles.x, originalRot.eulerAngles.y + lookAngle, originalRot.eulerAngles.z);

                        if (Mathf.Abs(Mathf.DeltaAngle(test1.eulerAngles.y, test2.eulerAngles.y)) < nearAngle)
                        {
                            returning = true;
                        }
                    
                        if (returning && Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, originalRot.eulerAngles.y)) < nearAngle) 
                        {
                            if (finalTurn)
                            {
                                lookingState = LOOKINGSTATE.postLook;
                            }
                            else
                            {
                                lookingState = LOOKINGSTATE.lookingLeft;
                                finalTurn = true;
                                returning = false; 
                            }
                        }
                        break; 
                    case LOOKINGSTATE.postLook:
                        if (movementTarget.magnitude == Mathf.Infinity)
                        {
                            bool foundDirection = false;
                            moveLength = movementDistance; 

                            while (!foundDirection)
                            {
                                Vector3 vectorToTest = Quaternion.Euler(0, Random.Range(0, 360), 0) * (Vector3.forward);
                                
                                if (!Physics.BoxCast(transform.position, new Vector3(2, 2, 2), vectorToTest, out hit, transform.rotation, moveLength))
                                {
                                    Vector3 testPosition = (transform.position + (vectorToTest * moveLength));
                                    Physics.Raycast(testPosition, Vector3.down * 2, out hit);
                                    foreach (GameObject go in PatrolArea)
                                    {
                                        if (go == hit.transform.gameObject)
                                        {
                                            foundDirection = true;
                                            movementTarget = testPosition; 
                                        }
                                    }
                                }
                                else
                                {
                                    moveLength += -1; 
                                }
                            }
                        }

                        timer += Time.deltaTime;
                        if (timer > postWait)
                        {
                            Quaternion lookat = Quaternion.LookRotation(movementTarget - transform.position);
                            lookat = Quaternion.Euler(lookat.eulerAngles.x, lookat.eulerAngles.y - 90, lookat.eulerAngles.z); 
                            transform.rotation = Quaternion.Slerp(transform.rotation, lookat, Time.deltaTime * turnSpeed);
                            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, lookat.eulerAngles.y)) < 1)
                            {
                                lookingState = LOOKINGSTATE.preLook;
                                enemyState = ENEMYSTATE.walking;
                                finalTurn = false;
                                returning = false;
                                timer = 0; 
                            }
                        }
                        break; 
                }
                break;
            case ENEMYSTATE.attacking:
                transform.LookAt(player);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);

                break;
        }

    }

    public void SpeedUp()
    {
        faster = true;
    }

    public void SlowDown()
    {
        faster = false;
    }
}
