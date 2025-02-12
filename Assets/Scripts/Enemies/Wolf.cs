using UnityEngine;

public class Wolf : GroundEnemy
{
    [Header("Visual Effect References")]
    public GameObject sprite;
    public GameObject spark;


    protected override void FixedUpdate()
    {
        //Simple test behavior, replace with state behavior later
        if(IsPlayerInPursueRange()) 
        {
            FlipToTarget(player.position);

            if(!IsPlayerInMeleeRange() && !EdgeCheck()) 
            {
                MoveToPosition(player.position, moveSpeed);
            }  
            else 
            {
                SetVelocity(new Vector2(0f, rb.velocity.y));
            }  
        }
    }


    public override void OnDamage()
    {
        Instantiate(spark, new Vector3(sprite.transform.position.x , sprite.transform.position.y , 0), Quaternion.identity); 
        KnockBack();
    }

    public override void OnDeath()
    {
        Destroy(gameObject);
    }
}
