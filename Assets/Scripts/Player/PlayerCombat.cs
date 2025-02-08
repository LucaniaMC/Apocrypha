using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a partial class for Player.cs containing functions that handle player attacks
//Only Player.cs needs to be attatched to an object

public partial class Player
{
    #region Attack
    // Turns on a given hitbox for a given amount of time, then turns off
    public IEnumerator AttackCoroutine(Collider2D attackHitbox, float attackTime) 
    {
        attackHitbox.enabled = true;
        yield return new WaitForSeconds(attackTime);
        attackHitbox.enabled = false;
    }
    #endregion


    #region Knockback
    //knock the player towards a direction and transition to knockback state.
	//Takes input float x as horizontal velocity, y as vertical velocity, and float time >= 0 as knockback state length
	public void Knockback(Vector2 direction, float time, int damage)
    {
        if (!health.isInvincible)
        {
            rb.velocity = direction;    //Set x and y to knockback velocity

            if (damage != 0) 
                health.Damage(damage);
            
            input.CancelChargeAttack();
            TransitionToState(new PlayerKnockbackState(this, time));    //Enter knockback state with given time
        }
    }
    #endregion
}