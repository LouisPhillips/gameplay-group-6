using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attacking : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    public float attackRange = 0.5f;
    public int attackDamage = 1;

    bool attacking;

    PlayerControls controls;
    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void Update()
    {
        
    }
    public void Attack()
    {
        //play animation
        animator.SetTrigger("Attack");
        //deteckt enemies
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        //damage
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log(enemy.GetComponent<SlimeScript>().health);
            enemy.GetComponent<SlimeScript>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
