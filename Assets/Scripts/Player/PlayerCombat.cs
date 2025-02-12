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
            
        attackHitbox.enabled = false;   
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
    //Use this method for enemy attacks on player
    public void DamageAndKnockback(int damage, Vector2 knockbackVelocity, float time) 
    {
        if (!health.isInvincible) 
        {   
            health.Damage(damage);  //Damage needs to be called before invincibility
            Knockback(knockbackVelocity, time);
        }     
    }

    //knock the player towards a direction and transition to knockback state.
	//Takes input float x as horizontal velocity, y as vertical velocity, and float time >= 0 as knockback state length
	private void Knockback(Vector2 knockbackVelocity, float time)
    {
        SetVelocity(knockbackVelocity);    //Set player's velocity to knockback velocity   
        TransitionToState(new PlayerKnockbackState(this, time));    //Enter knockback state with given time
    }


    #endregion
}