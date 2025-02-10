using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a partial class for Player.cs containing functions that handle player attack and knockback
//Only Player.cs needs to be attatched to an object

public partial class Player
{
    private Coroutine activeAttackRoutine;     //Currently active attack coroutine


    #region Attack
    // Turns on a given hitbox for a given amount of time, then turns off
    public void Attack(Collider2D attackHitbox, float attackTime) 
    {
        if(activeAttackRoutine != null)     // Stops currently active coroutine if any
            StopCoroutine(activeAttackRoutine);
        
        activeAttackRoutine = StartCoroutine(AttackCoroutine(attackHitbox, attackTime));

    }

    private IEnumerator AttackCoroutine(Collider2D attackHitbox, float attackTime) 
    {
        attackHitbox.enabled = true;
        yield return new WaitForSeconds(attackTime);
        attackHitbox.enabled = false;
    }
    #endregion


    #region Knockback
    //knock the player towards a direction and transition to knockback state.
	//Takes input float x as horizontal velocity, y as vertical velocity, and float time >= 0 as knockback state length
	public void Knockback(Vector2 knockbackVelocity, float time, int damage)
    {
        if (!health.isInvincible)
        {
            SetVelocity(knockbackVelocity);    //Set player's velocity to knockback velocity

            if (damage != 0) 
                health.Damage(damage);  //Damage needs to be called before invincibility
            
            TransitionToState(new PlayerKnockbackState(this, time));    //Enter knockback state with given time
        }
    }
    #endregion
}