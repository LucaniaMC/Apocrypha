using UnityEngine;

public class Sludge : MonoBehaviour
{

    public GameObject sprite;
    public GameObject spark;


    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == 3)    //Check collision with player layer
        {
            float knockbackDirection = 0; //Multiplies with x knockback force to determine which direction is the knockback

            //Compare the x position of the player and itself to see which direction to knock the player back
            if (other.transform.position.x < transform.position.x) //if the player is on the left side...
            {
                knockbackDirection = -1f;   //knock the player to the left.
            }
            else    //Otherwise if the player is on the right, or in the middle...
            {
                knockbackDirection = 1f;    //knock the player to the right.
            }

            other.gameObject.GetComponent<PlayerMovement>().Knockback(10 * knockbackDirection , 500, true); //Apply knockback
            other.gameObject.GetComponent<HealthSystem>().Damage(10);    
            //Damage after knockback, so invisibility is only switched on after the player received knockback, otherwise knockback would not work
        }
    }

    public void onDamage()
    {
        Instantiate(spark, new Vector3(sprite.transform.position.x , sprite.transform.position.y , 0), Quaternion.identity); 
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
