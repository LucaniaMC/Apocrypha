using UnityEngine;

//Attach this script to the hitbox object
[RequireComponent(typeof(Collider2D))]

public class EnemyHitbox : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float damage;                //How much damage does this attack deal
    [SerializeField] private Vector2 knockbackVelocity;   // Velocity to knock the player back with
    [SerializeField] private float knockbackTime;         // How long will the player be knocked back from this attack

    [Header("References")]
    [SerializeField] private Player player;             // The player class
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
            player.DamageAndKnockback(Mathf.RoundToInt(damage), adjustedKnockbackVelocity, knockbackTime); //Apply knockback and damage
        }
    }
}
