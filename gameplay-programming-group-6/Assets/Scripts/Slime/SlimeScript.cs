using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeScript : MonoBehaviour
{
    public int health;
    public enum slimeType { Large, Medium, Small, Boss, Minion };
    public slimeType slime;

    public GameObject slimeBoss;
    public GameObject largeSlime;
    public GameObject mediumSlime;
    public GameObject smallSlime;
    public GameObject slimeMinion;


    NavMeshAgent agent;
    NavMeshHit navHit;

    private float playerDistance;
    public float detectionRange = 10f;
    public float wanderRange = 10f;
    private Vector3 wanderTarget;
    private float wanderDelay;
    public float wanderEvery = 7f;

    public bool bossCanBeDamaged = false;

    public Transform target;
    public bool bossAttack;
    public int enemiesDestroyed = 0;
    int enemiesSpawned = 0;

    RaycastHit playerCast;
    private bool canAttack = true;
    private float attackDelay;
    public float attackTime = 2f;

    public bool hit;
    public Material healthy;
    public Material damaged;
    private float damagedTime;

    // Start is called before the first frame update
    void Awake()
    {
        switch (slime)
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
            case slimeType.Boss:
                {
                    health = 30;
                    break;
                }
            case slimeType.Minion:
                {
                    detectionRange = 100;
                    health = 1;
                    break;
                }
        }

        agent = gameObject.transform.GetComponent<NavMeshAgent>();
        target = GetPlayer.call.player.transform;
        slimeBoss = GameObject.FindGameObjectWithTag("Boss");
    }

    // Update is called once per frame
    void Update()
    {
        switch (slime)
        {
            case slimeType.Small:
                {
                    if (health == 0)
                    {
                        health = -1;
                        Destroy(gameObject);
                        break;
                    }
                    break;
                }
            case slimeType.Minion:
                {
                    if (health == 0)
                    {
                        health = -1;
                        Destroy(gameObject);
                        slimeBoss.transform.gameObject.GetComponent<SlimeBoss>().enemiesDestroyed += 1;
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
            case slimeType.Boss:
                {
                    if (health == 30)
                    {
                        if (enemiesSpawned < 10)
                        {
                            Instantiate(slimeMinion, new Vector3(gameObject.transform.position.x * 2 + Random.Range(1, 3), gameObject.transform.position.y, gameObject.transform.position.z * 2 + Random.Range(1, 3)), gameObject.transform.rotation);
                            enemiesSpawned += 1;
                        }
                        if (enemiesDestroyed == 10)
                        {
                            enemiesSpawned = 0;
                            bossCanBeDamaged = true;
                            if (health == 20)
                            {
                                bossCanBeDamaged = false;
                            }
                        }
                        break;
                    }
                    if (health == 20)
                    {
                        if (enemiesSpawned < 25)
                        {
                            Instantiate(slimeMinion, new Vector3(gameObject.transform.position.x * 2 + Random.Range(1, 3), gameObject.transform.position.y, gameObject.transform.position.z * 2 + Random.Range(1, 3)), gameObject.transform.rotation);
                            enemiesSpawned += 1;
                        }
                        if (enemiesDestroyed == 25)
                        {
                            enemiesSpawned = 0;
                            bossCanBeDamaged = true;
                            if (health == 10)
                            {
                                bossCanBeDamaged = false;
                            }
                        }
                        break;
                    }
                    if (health == 10)
                    {
                        if (enemiesSpawned < 45)
                        {
                            Instantiate(slimeMinion, new Vector3(gameObject.transform.position.x * 2 + Random.Range(1, 3), gameObject.transform.position.y, gameObject.transform.position.z * 2 + Random.Range(1, 3)), gameObject.transform.rotation);
                            enemiesSpawned += 1;
                        }
                        if (enemiesDestroyed == 45)
                        {
                            bossCanBeDamaged = true;
                            if (health == 0)
                            {
                                Destroy(gameObject);
                            }
                        }
                        break;
                    }
                    break;
                }

        }

        playerDistance = Vector3.Distance(target.position, gameObject.transform.position);

        if (target.GetComponent<PlayerMovement>().health > 0)
        {
            switch (slime)
            {
                case slimeType.Boss:
                    {
                        if (!bossCanBeDamaged)
                        {
                            Debug.Log("Going to back");
                
                            /*if (!bossCanBeDamaged)
                            {
                                agent.destination = goBackToPoint.position;
                            }   */
                        }
                        else
                        {
                            Debug.Log("Going to player");
                            agent.destination = target.position;
                        }
                        break;
                    }
                case slimeType.Small:
                    {
                        if (playerDistance <= detectionRange)
                        {
                            agent.destination = target.position;
                            break;
                        }
                        break;
                    }
                case slimeType.Medium:
                    {
                        if (playerDistance <= detectionRange)
                        {

                            agent.destination = target.position;
                        }

                        break;
                    }
                case slimeType.Large:
                    {
                        if (playerDistance <= detectionRange)
                        {
                            agent.destination = target.position;
                        }

                        break;
                    }
                case slimeType.Minion:
                    {
                        agent.destination = target.position;
                        break;
                    }


            }

        }

       
        switch (slime)
        {
            case slimeType.Boss:
                {
                    Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - 4, transform.position.z), transform.forward * 7, Color.green);
                    if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 4f, transform.position.z), transform.forward, out playerCast, 7))
                    {
                        if (playerCast.transform.gameObject.tag == "Player")
                        {
                            Attack();
                        }
                    }
                    break;
                }
            case slimeType.Large:
                {
                    if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), transform.forward, out playerCast, 3))
                    {
                        if (playerCast.transform.gameObject.tag == "Player")
                        {
                            Attack();
                        }
                    }
                    break;
                }
            case slimeType.Medium:
                {
                    if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), transform.forward, out playerCast, 3))
                    {
                        if (playerCast.transform.gameObject.tag == "Player")
                        {
                            Attack();
                        }
                    }
                    break;
                }
            case slimeType.Small:
                {
                    if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), transform.forward, out playerCast, 3))
                    {
                        if (playerCast.transform.gameObject.tag == "Player")
                        {
                            Attack();
                        }
                    }
                    break;
                }
            case slimeType.Minion:
                {
                    Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), transform.forward * 3, Color.green);
                    if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), transform.forward, out RaycastHit minion, 3))
                    {
                        if (minion.transform.gameObject.tag == "Player")
                        {
                            Attack();
                        }
                    }
                    break;
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
            if (damagedTime < 0.4)
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
            switch (slime)
            {
                case slimeType.Boss:
                    {
                        wanderDelay += Time.deltaTime;
                        if (wanderDelay > wanderEvery)
                        {
                            agent.SetDestination(RandomSphere(gameObject.transform.position, wanderRange));
                            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
                            {
                                wanderDelay = 0;
                            }
                        }
                        break;
                    }
                case slimeType.Minion:
                    {
                        wanderDelay += Time.deltaTime;
                        if (wanderDelay > wanderEvery)
                        {
                            agent.SetDestination(RandomSphere(gameObject.transform.position, wanderRange));
                            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
                            {
                                wanderDelay = 0;
                            }
                        }
                        break;

                    }
                case slimeType.Large:
                    {
                        wanderDelay += Time.deltaTime;
                        if (wanderDelay > wanderEvery)
                        {
                            agent.SetDestination(RandomSphere(gameObject.transform.position, wanderRange));
                            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
                            {
                                wanderDelay = 0;
                            }
                        }
                        break;
                    }
                case slimeType.Medium:
                    {
                        wanderDelay += Time.deltaTime;
                        if (wanderDelay > wanderEvery)
                        {
                            agent.SetDestination(RandomSphere(gameObject.transform.position, wanderRange));
                            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
                            {
                                wanderDelay = 0;
                            }
                        }
                        break;
                    }
                case slimeType.Small:
                    {
                        wanderDelay += Time.deltaTime;
                        if (wanderDelay > wanderEvery)
                        {
                            agent.SetDestination(RandomSphere(gameObject.transform.position, wanderRange));
                            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
                            {
                                wanderDelay = 0;
                            }
                        }
                        break;
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
            if (target.GetComponent<PlayerMovement>().takeNoDamage == false)
            {
                target.GetComponent<PlayerMovement>().health -= 1;
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
