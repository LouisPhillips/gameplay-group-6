using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public GameObject slime;
    private EnemySlime script; 

    private void Awake()
    {
        script = slime.GetComponent<EnemySlime>();     
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            script.coneHit = true; 
        }
    }

    private void FixedUpdate()
    {
        script.coneHit = false;
        transform.position = slime.transform.position;
        transform.rotation = slime.transform.rotation;
        transform.Rotate(new Vector3(0, 0, 90));
        transform.Translate(Vector3.down * 18f); 
    }
}
