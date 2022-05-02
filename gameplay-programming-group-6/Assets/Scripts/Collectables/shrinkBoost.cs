using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shrinkBoost : MonoBehaviour
{
    bool collided;
    public PlayerMovement player;
    public bool collected;
    public GameObject obj;
    public GameObject obj2;
    public float time = 0f;
    public bool resized;
    // Start is called before the first frame update
    void Start()
    {
        resized = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (!collected)
        {
            if (collided)
            {
                collected = true;
            }
        }

        if (collected)
        {
            
            time += Time.deltaTime;
            if (time < 5)
            {
                player.canShrinkBoost = true;
            }
            if (time > 5)
            {
                player.canResize = true;
                player.canShrinkBoost = false;
            }
            if (time < 10)
            {
                obj.GetComponent<MeshRenderer>().enabled = false;
                obj2.GetComponent<MeshRenderer>().enabled = false;
            }
            if (time > 10)
            {
                obj.GetComponent<MeshRenderer>().enabled = true;
                obj2.GetComponent<MeshRenderer>().enabled = true;
                collected = false;
                collided = false;
                if(resized)
                {
                    time = 0f;
                    resized = false;
                }
            }
            else
            {
                
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
