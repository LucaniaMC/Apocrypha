using UnityEngine;

//Attach this script to the hitbox object
[RequireComponent(typeof(Collider2D))]

public class EnemyHitbox : MonoBehaviour
{
    [SerializeField] private float damage;                //How much damage does this attack deal
    [SerializeField] private Vector2 knockbackVelocity;   // Velocity to knock the player back with
    [SerializeField] private float knockbackTime;         // How long will the player be knocked back from this attack
    [SerializeField] private float attackMultiplier = 1f; //Damage modifier for this attack
    [SerializeField] private Player player;
    [SerializeField] private GameObject self;           // Reference the main gameobject, not the object this script is attached to


    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == 3)    //Check collision with player layer
        {
            Vector2 adjustedKnockbackVelocity = knockbackVelocity; 

            //Knockback to the player is determined based on which side of the enemy the player is at
            if (other.transform.position.x < self.transform.position.x) //if the player is on the left side
            {
                adjustedKnockbackVelocity.x *= -1;   // Change knockback direction to the left
            }
            
            player.Knockback(adjustedKnockbackVelocity, knockbackTime, Mathf.RoundToInt(damage * attackMultiplier)); //Apply knockback and damage
        }
    }
}
