using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attacking : MonoBehaviour
{
    private Rigidbody sphererigidbody;
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public LayerMask bossLayer;
    public LayerMask newLayer;
    public float attackRange = 0.5f;
    public int attackDamage = 1;

    public int SwitchesUsed = 0;

    bool attacking;

    PlayerControls controls;
    private void Awake()
    {
        attackPoint = transform; 
        controls = new PlayerControls();
        sphererigidbody = GetComponent<Rigidbody>();
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void Hit()
    {
        //detect enemies
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
            if (boss.GetComponent<SlimeBoss>().bossCanBeDamaged)
            {
                boss.GetComponent<SlimeBoss>().health -= 1;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name); 
        if (other.tag == "Switch" && other.GetComponent<CutsceneSwitch>().used == false)
        {
            other.GetComponent<CutsceneSwitch>().PlayTimeline();
            SwitchesUsed += 1;
            if (SwitchesUsed == 3)
            {
                other.GetComponent<CutsceneSwitch>().allSwitch = true;
            }
            other.GetComponent<CutsceneSwitch>().used = true;
        }
        
        if (other.tag == "Enemy")
        {
            if (other.GetComponent<EnemySlime>())
            {
                other.GetComponent<EnemySlime>().health -= 1;
                other.GetComponent<EnemySlime>().hit_slime = true;
            }
            else if (other.GetComponent<SlimeBoss>())
            {
                other.GetComponent<SlimeBoss>().health -= 1; 
            }
            else if (other.GetComponent<SlimeScript>())
            {
                other.GetComponent<SlimeScript>().TakeDamage(attackDamage);
            }
        }    
    }
}
