using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemybehaviour : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform Player;
    public LayerMask whatIsGround, whatIsPlayer;

    // patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //attacking
    public float TimeBetweenAttacks;
    bool alreadyattacked;
    //states
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public int maxHealth = 100;
    int currentHealth;
    public int enemyCount = 3;
    public float xAgent;
    public float zAgent;
    private void Start()
    {
        currentHealth = maxHealth;
        
        
    }

    private void Update()
    {
        //check for sight and attack
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();

    }

    private void Awake()
    {
        Player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Patroling() 
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet) agent.SetDestination(walkPoint);
        Vector3 distanceTowalkPoint = transform.position - walkPoint;

        //walkpoint reached
        if (distanceTowalkPoint.magnitude < 1f) walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) walkPointSet = true;
        

    }
    private void ChasePlayer()
    {
        agent.SetDestination(Player.position);
    }
    private void AttackPlayer()
    {
        //enemy not moving
        agent.SetDestination(transform.position);

        transform.LookAt(Player);

        if (!alreadyattacked) 
        {
            //attack code




            alreadyattacked = true;
            Invoke(nameof(ResetAttack), TimeBetweenAttacks);
        }

    }
    private void ResetAttack() 
    {
        alreadyattacked = false;
    }
     public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        //hurt animation
        if (currentHealth <= 0) StartCoroutine(EnemySpawm()); Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        
        Debug.Log("Enemy dies!");
        Destroy(gameObject);
        
    }
    IEnumerator EnemySpawm()
    {
        while(enemyCount < 3)
        {
            xAgent = agent.transform.position.x;
            zAgent = agent.transform.position.z;
            Instantiate(agent,new Vector3(xAgent,0,zAgent),Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            enemyCount += 1;

        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
