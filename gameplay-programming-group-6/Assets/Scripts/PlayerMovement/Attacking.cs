using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attacking : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public LayerMask bossLayer;
    public LayerMask newLayer;
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
        Collider[] bossHit = Physics.OverlapSphere(attackPoint.position, attackDamage, bossLayer);
        Collider[] newSlimes = Physics.OverlapSphere(attackPoint.position, attackDamage, newLayer);
        //damage
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<SlimeScript>().TakeDamage(attackDamage);
            
        }
        foreach (Collider newslime in newSlimes)
        {
            newslime.GetComponent<EnemySlime>().health -= 1;
            newslime.GetComponent<EnemySlime>().hit_slime = true;
        }
        foreach (Collider boss in bossHit)
        {
            if(boss.GetComponent<SlimeBoss>().bossCanBeDamaged)
            {
                boss.GetComponent<SlimeBoss>().health -= 1;
            }
            
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
