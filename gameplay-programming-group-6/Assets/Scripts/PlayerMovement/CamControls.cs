using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControls : MonoBehaviour
{
    PlayerControls controls;
    Vector2 camStickDirection;

    private bool lockedOn = false;
    private GameObject lockOnTarget;
    private Transform player;
    private float switchTimer;
    public float switchTime; 


    public Transform target; 
    public float turnSpeed;
    public float moveSpeed;
    public Transform targeter; 
    public float topAngle;
    public float bottomAngle;
    public float maxLockOnDistance; 

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Look.performed += context => camStickDirection = context.ReadValue<Vector2>();
        controls.Player.Look.canceled += context => camStickDirection = Vector2.zero;

        controls.Player.LockOn.performed += context => lockOnPressed();

        player = GameObject.FindGameObjectWithTag("Player").transform; 
    }

    private void OnEnable()
    {
        controls.Player.Look.Enable();
        controls.Player.LockOn.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Look.Disable();
        controls.Player.LockOn.Disable();
    }

    private void Update()
    {
        if (lockedOn)
        {
            Vector3 midpoint = ((lockOnTarget.transform.position + target.position) / 2);
            player.LookAt(lockOnTarget.transform);
            player.rotation = Quaternion.Euler(0, player.eulerAngles.y, 0); 
            transform.position = midpoint + (-transform.forward * Vector3.Distance(lockOnTarget.transform.position, target.position)) + transform.up - transform.forward ;
            transform.rotation = player.rotation; 
            targeter.position = new Vector3(lockOnTarget.transform.position.x, lockOnTarget.transform.position.y + lockOnTarget.transform.localScale.y, lockOnTarget.transform.position.z); 

            if (Vector3.Distance(player.transform.position, lockOnTarget.transform.position) > Mathf.Sqrt(maxLockOnDistance))
            {
                lockedOn = false;
                player.gameObject.GetComponent<PlayerMovement>().lockOn = false;
            }

            switchTimer += Time.deltaTime;

            if (camStickDirection.x != 0 && FindNextClosestEnemy() != null && switchTimer > 0.5f)
            {
                lockOnTarget = FindNextClosestEnemy();
                switchTimer = 0;
            }
        }
        else
        {
            lockOnTarget = null; 
            targeter.position = new Vector3(10000,10000,10000); 
            transform.position = target.transform.position + (-transform.forward * 5);
            Vector2 c = camStickDirection * turnSpeed * Time.deltaTime;
            transform.Rotate(-c.y, c.x, 0);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);

            if (transform.eulerAngles.x > topAngle && transform.eulerAngles.x < 90)
            {
                transform.rotation = Quaternion.Euler(topAngle, transform.eulerAngles.y, 0);
            }

            if (transform.eulerAngles.x < 360 - bottomAngle && transform.eulerAngles.x > 270)
            {
                transform.rotation = Quaternion.Euler(360 - bottomAngle, transform.eulerAngles.y, 0);
            }
        }
    }

    private void lockOnPressed()
    {
        if (FindNextClosestEnemy() != null && lockedOn == false)
        {
            lockOnTarget = FindNextClosestEnemy();
            lockedOn = true;
            player.gameObject.GetComponent<PlayerMovement>().lockOn = true; 
            
        }
        else 
        {
            lockedOn = false;
            player.gameObject.GetComponent<PlayerMovement>().lockOn = false;
        }
    }

    private GameObject FindNextClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
        int i = 0;
        foreach (GameObject go in gos)
        {
            if (go == lockOnTarget)
            {  
                gos[i] = null;
                break;
            }
            i++;
        }

        GameObject closest = null;
        float angle = 360;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            if (go != null)
            {
                Vector3 diff = go.transform.position - position;
                float angleDiff = Vector3.SignedAngle(transform.forward, diff, transform.up);
                float curDistance = diff.sqrMagnitude;
                if (curDistance < maxLockOnDistance + 100)
                { 
                    if ((lockOnTarget == null ||angleDiff * Mathf.Sign(camStickDirection.x) > 0) && Mathf.Abs(angleDiff) < angle)
                    {
                        closest = go;
                        angle = Mathf.Abs(angleDiff);
                    }
                }
            }
        }
        return closest;
    }
}
