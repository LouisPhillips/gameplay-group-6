using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftToRight : MonoBehaviour
{
    private int direction = 1;
    public float rightMax;
    public float leftMax;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + (direction * speed * Time.deltaTime), transform.position.y, transform.position.z);
        if (transform.position.x > rightMax)
        {
            direction = -1;
        }
        else if(transform.position.x < leftMax)
        {
            direction = 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.parent = transform;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.parent = null;
        }

    }
}
