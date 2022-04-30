using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public SpawnedPlatTimer platform;
    public GameObject spawningObject;
    public float minSpawnTime;
    public float maxSpawnTime;

    public float minZSpawn;
    public float maxZSpawn;
   
    private float RespawnTime;

    // Start is called before the first frame update
    void Start()
    {
        spawningObject.transform.localPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        spawningObject.transform.parent = transform;
        RespawnTime += Time.deltaTime;
        {
            float random = Random.Range(minSpawnTime, maxSpawnTime);
            if (RespawnTime > random)
            {
                Instantiate(spawningObject, new Vector3(transform.position.x, transform.position.y, transform.position.z + Random.Range(minZSpawn, maxZSpawn)) , transform.rotation);
                RespawnTime = 0;
               
            }
            

        }
        
    }
}
