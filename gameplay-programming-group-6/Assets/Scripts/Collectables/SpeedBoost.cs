using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    bool collided;
    public PlayerMovement player;
    public bool collected;
    public float time = 0f;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (!collected)
        {
            if (collided)
            {
                collected = true;
                //Destroy(gameObject);
                
                

            }
        }
 


        if (collected)
        {
            time += Time.deltaTime;
            if (time < 5)
            {
                player.canSpeedBoost = true;
            }
            if (time > 5)
            {

                player.canSpeedBoost = false;
                collected = false;
            }
            if (time < 10)
            {
                GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                GetComponent<MeshRenderer>().enabled = true;
                
                collided = false;
                time = 0f;
            }
        }

    }

    private void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player"))
        {
            collided = true;
        }
    }
}
