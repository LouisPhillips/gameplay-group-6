using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedPlatTimer : MonoBehaviour
{
    private float time;
    public float speed;
    public float timeBeforeDespawn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if(time > timeBeforeDespawn)
        {
            Destroy(gameObject);
        }

        transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
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
