using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPlatform : MonoBehaviour
{
    public PlayerMovement player;
    public float maxDistance;
    public float speed;
    private bool inside;
    private float time;
    private float startTime;
    private bool end;
    private Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startTime = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        //if(player.platButtonPressed)
        {
            if (transform.position.z < maxDistance)
            {
                if(!end)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed * Time.deltaTime);
                }
                
            }
            else if (transform.position.z > maxDistance)
            {
                if(!inside)
                {
                    time += Time.deltaTime;
                    if(time > 5)
                    {
                        Debug.Log("here");
                        end = true;
                    }
                }
            }
            if(end)
            {
                transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime * 0.3f);
                if (transform.position.z <= startPos.z + 0.1)
                {
                    //player.platButtonPressed = false;
                    end = false;
                    time = 0;
                }
            }
        }

        /*if less than tdistance, move 

        if > orr = travelDistance, wait until player leaves then 5 seconds

        after 5 seconds, reverse*/
    }

    private void OnTriggerEnter(Collider other)
    {
        inside = true;
        //player.canPlatformButton = true;
        //player.buttonReminder.SetActive(true);
        //player.transform.parent = transform;
    }

    private void OnTriggerExit(Collider other)
    {
        inside = false;
        //player.canPlatformButton = false;
        //player.buttonReminder.SetActive(false);
        //player.transform.parent = null;
    }
}
