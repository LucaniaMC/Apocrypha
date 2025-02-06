using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a partial class for Player.cs containing functions that handle player attacks
//Only Player.cs needs to be attatched to an object

public partial class Player
{
    // Turns on a given hitbox for a given amount of time
    public IEnumerator AttackCoroutine(Collider2D attackHitbox, float attackTime) 
    {
        attackHitbox.enabled = true;
        yield return new WaitForSeconds(attackTime);
        attackHitbox.enabled = false;
    }
}