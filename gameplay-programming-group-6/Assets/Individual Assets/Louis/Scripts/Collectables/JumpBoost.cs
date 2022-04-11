using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoost : MonoBehaviour
{
    bool collided;
    public PlayerControls2 player;
    public bool collected;
    public GameObject obj;
    public GameObject obj2;
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
            }
        }
        if (collected)
        {
            time += Time.deltaTime;
            if (time < 5)
            {
                player.canJumpBoost = true;
            }
            if (time > 5)
            {
                player.canJumpBoost = false;
                collected = false;
            }
            if (time < 10)
            {
                //GetComponent<MeshRenderer>().enabled = false;
                obj.GetComponent<MeshRenderer>().enabled = false;
                obj2.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                //GetComponent<MeshRenderer>().enabled = true;
                obj.GetComponent<MeshRenderer>().enabled = true;
                obj2.GetComponent<MeshRenderer>().enabled = true;
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
