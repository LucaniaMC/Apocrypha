using UnityEngine;
using System.Collections.Generic;

//Attach this script to the player hitbox object with a Collider2D component
[RequireComponent(typeof(Collider2D))]

public class PlayerAttackHitbox : MonoBehaviour
{
    public PlayerData data;
    [SerializeField] private float attackModifier = 1f; //Damage modifier for individual attack hitboxes

    //Damage all enemies in attack hitbox
    void OnTriggerEnter2D(Collider2D other) 
    {
        List<GameObject> attackedEnemy = new List<GameObject>();    //Used list for easy resize

        if (other.gameObject.layer == 7)    //If the collided object is in enemy layer, layer 7 is enemy
        {
            attackedEnemy.Add(other.gameObject);    //Add the object to the list

            //Get the HealthSystem component of each enemy, and damages them
            foreach (GameObject enemy in attackedEnemy) 
            {
                enemy.GetComponent<HealthSystem>().Damage(Mathf.RoundToInt(data.attackDamage * attackModifier));
            }
        }
    }
}
