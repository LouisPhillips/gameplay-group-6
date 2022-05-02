using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform target;

    private void OnTriggerEnter(Collider other)
    {  
        if (other.tag == "Player")
        {
            other.transform.position = target.transform.position;
            other.transform.rotation = target.transform.rotation;
            other.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
    }
}
