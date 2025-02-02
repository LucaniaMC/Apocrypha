using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a partial class for Player.cs containing functions that handle player attacks
//Only Player.cs needs to be attatched to an object

public partial class Player
{
    public IEnumerator AttackCoroutine() 
    {
        attackCollider.enabled = true;
        yield return new WaitForSeconds(0.2f);
        attackCollider.enabled = false;
    }
}