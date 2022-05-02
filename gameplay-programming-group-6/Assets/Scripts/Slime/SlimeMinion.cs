using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeMinion : MonoBehaviour
{
    public int health;

    public GameObject slimeMinion;

    NavMeshAgent agent;
    NavMeshHit navHit;

    private float playerDistance;
    public float detectionRange = 10f;
    public float wanderRange = 10f;
    private Vector3 wanderTarget;
    private float wanderDelay;
    public float wanderEvery = 7f;

    public Transform slimeBoss;
    public bool bossCanBeDamaged = false;

    public Transform target;

    RaycastHit playerCast;
    private bool canAttack = true;
    private float attackDelay;
    public float attackTime = 2f;

    public bool hit;
    public Material healthy;
    public Material damaged;
    private float damagedTime;

    void Awake()
    {
        detectionRange = 100;
        health = 1;

        agent = gameObject.transform.GetComponent<NavMeshAgent>();
        target = GetPlayer.call.player.transform;

        foreach (Transform child in GameObject.FindGameObjectWithTag("Boss").transform)
        {
            slimeBoss = child;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health == 0)
        {
            health = -1;
            Destroy(gameObject);
            slimeBoss.transform.gameObject.GetComponent<SlimeBoss>().enemiesDestroyed += 1;
        }

        playerDistance = Vector3.Distance(target.position, gameObject.transform.position);

        if (target.GetComponent<PlayerMovement>().health > 0)
        {
            agent.destination = target.position;
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
}
