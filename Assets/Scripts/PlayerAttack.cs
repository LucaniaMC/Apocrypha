using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [Header("Parameters")]

    public float attackCooldown = 0.1f; //Cooldown time between attacks
    public int attackDamage = 10;
    [Space]
    public bool attacking = false;


    [Header("References")]

    public CapsuleCollider2D attackCollider;


    void Awake() 
    {
        attackCollider.enabled = false;
    }


    //attack, turn on attack hitbox for a bit
    public IEnumerator Attack() 
    {
        if (attacking == false) 
        {
            attacking = true;
            attackCollider.enabled = true;
            yield return new WaitForSeconds(.2f);
            attackCollider.enabled = false;
            yield return new WaitForSeconds(attackCooldown);
            attacking = false;
        }  
    }


    //Damage all enemies in attack hitbox
    void OnTriggerEnter2D(Collider2D other) 
    {
        List<GameObject> attackedEnemy = new List<GameObject>();

        if (other.gameObject.layer == 7) 
        {
            attackedEnemy.Add(other.gameObject);

            foreach (GameObject enemy in attackedEnemy) 
            {
                enemy.GetComponent<HealthSystem>().Damage(attackDamage);
            }
        }
    }
}
