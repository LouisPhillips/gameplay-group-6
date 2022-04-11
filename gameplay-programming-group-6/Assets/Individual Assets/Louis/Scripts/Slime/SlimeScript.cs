using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeScript : MonoBehaviour
{
    public int health;
    public enum slimeType {Large, Medium, Small };
    public slimeType slime;

    public GameObject largeSlime;
    public GameObject mediumSlime;
    public GameObject smallSlime;

    NavMeshAgent agent;
    NavMeshHit navHit;

    private float playerDistance;
    public float detectionRange = 10f;
    public float wanderRange = 10f;
    private Vector3 wanderTarget;
    private float wanderDelay;
    public float wanderEvery = 7f;

    public Transform target;

    RaycastHit playerCast;
    private bool canAttack = true;
    private float attackDelay;
    public float attackTime = 2f;

    public bool hit;
    public Material healthy;
    public Material damaged;
    private float damagedTime;

    // Start is called before the first frame update
    void Start()
    {
        switch(slime)
        {
            case slimeType.Small:
                {
                    health = 1;
                    break;
                }
            case slimeType.Medium:
                {
                    health = 3;
                    break;
                }
            case slimeType.Large:
                {
                    health = 5;
                    break;
                }
        }

        agent = gameObject.transform.GetComponent<NavMeshAgent>();
        target = GetPlayer.call.player.transform;

    }

    // Update is called once per frame
    void Update()

    {
        switch (slime)
        {
            case slimeType.Small:
                {
                    if(health == 0)
                    {
                        health = -1;
                        Destroy(gameObject);
                        break;
                    }
                    break;
                }
            case slimeType.Medium:
                {
                    if (health == 0)
                    {
                        Instantiate(smallSlime, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), gameObject.transform.rotation);
                        Instantiate(smallSlime, new Vector3(gameObject.transform.position.x + 2, gameObject.transform.position.y, gameObject.transform.position.z), gameObject.transform.rotation);
                        health = -1;
                        Destroy(gameObject);
                        break;
                    }

                    break;
                }
            case slimeType.Large:
                {
                    if (health == 0)
                    {
                        Instantiate(mediumSlime, new Vector3(gameObject.transform.position.x + 2, gameObject.transform.position.y, gameObject.transform.position.z), gameObject.transform.rotation);
                        Instantiate(mediumSlime, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), gameObject.transform.rotation);
                        health = -1;
                        Destroy(gameObject);
                        break;
                    }
                    break;
                }
        }

        playerDistance = Vector3.Distance(target.position, gameObject.transform.position);

        if (target.GetComponent<PlayerControls2>().health > 0)
        {
            if (playerDistance <= detectionRange)
            {
                agent.destination = target.position;
            }
        }

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), transform.forward * 3, Color.green);

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), transform.forward, out playerCast, 3))
        {
            if (playerCast.transform.gameObject.tag == "Player")
            {
                Attack();
            }
        }

        if (!canAttack)
        {
            attackDelay += Time.deltaTime;
            if (attackDelay < attackTime)
            {
                canAttack = false;
            }
            else
            {
                canAttack = true;
                attackDelay = 0f;
            }

        }

        if (hit)
        {
            damagedTime += Time.deltaTime;
            if(damagedTime < 0.4)
            {
                gameObject.GetComponent<MeshRenderer>().material = healthy;
            }
            else if (damagedTime < 0.6 && damagedTime > 0.4)
            {
                gameObject.GetComponent<MeshRenderer>().material = damaged;
            }
            else if (damagedTime > 0.6)
            {
                gameObject.GetComponent<MeshRenderer>().material = healthy;
                damagedTime = 0f;
                hit = false;
            }
        }

        if (playerDistance > detectionRange)
        {
            wanderDelay += Time.deltaTime;
            if (wanderDelay > wanderEvery)
            {
                agent.SetDestination(RandomSphere(gameObject.transform.position, wanderRange));
                if(agent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    wanderDelay = 0;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, detectionRange);
    }

    void Attack()
    {
        if (canAttack)
        {
            if (target.GetComponent<PlayerControls2>().takeNoDamage == false)
            {
                target.GetComponent<PlayerControls2>().health -= 1;
            }
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 1);
            canAttack = false;
        }
    }

    Vector3 RandomSphere(Vector3 start, float range)
    {
        Vector3 randomPoint = Random.insideUnitSphere * range;

        randomPoint += start;

        NavMeshHit navMeshHit;

        NavMesh.SamplePosition(randomPoint, out navMeshHit, range, NavMesh.AllAreas);

        return navMeshHit.position;
    }

    
}

