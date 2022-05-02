using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class SlimeBoss : MonoBehaviour
{
    public int health;

    public GameObject slimeMinion;

    public enum HealthState { Damagable, Protected };
    public HealthState health_state;

    NavMeshAgent agent;
    NavMeshHit navHit;

    private float playerDistance;
    public float detectionRange = 20f;
    public float wanderRange = 10f;
    private Vector3 wanderTarget;
    private float wanderDelay;
    public float wanderEvery = 7f;

    public bool bossCanBeDamaged = false;

    public Transform target;
    public Transform goBackToPoint;
    public bool bossAttack;
    public int enemiesDestroyed = 0;
    public int enemiesSpawned = 0;

    RaycastHit playerCast;
    private bool canAttack = true;
    private float attackDelay;
    public float attackTime = 2f;

    public bool hit;
    public Material healthy;
    public Material damaged;
    private float damagedTime;

    public GameObject ui;
    public Transform slimeMinionSpawn;

    // Start is called before the first frame update
    void Start()
    {
        health = 30;
        health_state = HealthState.Protected;
        agent = gameObject.transform.GetComponent<NavMeshAgent>();
        target = GetPlayer.call.player.transform;
        ui.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (health_state)
        {
            case HealthState.Damagable:
                {
                    bossCanBeDamaged = true;

                    break;
                }
            case HealthState.Protected:
                {
                    bossCanBeDamaged = false;
                    break;
                }
        }

        if (enemiesDestroyed == 10)
        {
            health_state = HealthState.Damagable;
            if (health == 20)
            {
                health_state = HealthState.Protected;
            }
        }
        if (enemiesDestroyed == 25)
        {
            health_state = HealthState.Damagable;
            if (health == 10)
            {
                health_state = HealthState.Protected;
            }
        }
        if (enemiesDestroyed == 45)
        {
            health_state = HealthState.Damagable;
            if (health == 0)
            {
                Destroy(gameObject);
            }
        }
    
        
        if (health == 0)
        {
            Destroy(gameObject);
        }

        playerDistance = Vector3.Distance(target.position, gameObject.transform.position);

        if (target.GetComponent<PlayerMovement>().health > 0)
        {
            if (!bossCanBeDamaged)
            {
                //Debug.Log("Going to back");
                agent.destination = goBackToPoint.position;
            }
            else
            {
                Debug.Log("Going to player");
                agent.destination = target.position;
            }
        }
        if(playerDistance <= detectionRange)
        {
            ui.SetActive(true);
            if (enemiesSpawned < 10 && health == 30)
            {
                Instantiate(slimeMinion, slimeMinionSpawn.position, gameObject.transform.rotation);
                enemiesSpawned += 1;
            }
            if (enemiesSpawned < 25 && health == 20)
            {
                Instantiate(slimeMinion, slimeMinionSpawn.position, gameObject.transform.rotation);
                enemiesSpawned += 1;
            }
            if (enemiesSpawned < 45 && health == 10)
            {
                Instantiate(slimeMinion, slimeMinionSpawn.position , gameObject.transform.rotation);
                enemiesSpawned += 1;
            }
        }
        else
        {
            ui.SetActive(false);
        }
        //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), transform.forward * 3, Color.green);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - 4, transform.position.z), transform.forward * 7, Color.green);
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 4f, transform.position.z), transform.forward, out playerCast, 7))
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
