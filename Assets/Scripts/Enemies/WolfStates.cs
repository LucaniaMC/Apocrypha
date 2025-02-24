using System.Collections;
using UnityEngine;

#region Idle State
// Stands in place
public class WolfIdleState : EnemyState
{
    protected Wolf wolf;

    public WolfIdleState(Enemy wolf) : base(wolf) 
    {
        this.wolf = (Wolf)wolf;
    }

    public override void OnEnter()
    {
        wolf.SetVelocity(new Vector2(0f, wolf.rb.velocity.y));  //Stays in place
    }
    
    public override void StateUpdate()
    {
        Transitions();
    }

    public override void StateFixedUpdate()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override void Transitions() 
    {
        if(wolf.IsPlayerInPursueRange()) 
        {
            wolf.TransitionToState(new WolfPursueState(wolf));
        }
    }
}
#endregion


#region Pursue State
// Move to the player, flipping if needed
public class WolfPursueState : EnemyState
{
    protected Wolf wolf;

    public WolfPursueState(Enemy wolf) : base(wolf) 
    {
        this.wolf = (Wolf)wolf;
    }

    public override void OnEnter()
    {

    }
    
    public override void StateUpdate()
    {
        Transitions();
    }

    public override void StateFixedUpdate()
    {
        wolf.FlipToTarget(wolf.player.position);

        if (!wolf.OnEdge()) 
        {
            wolf.MoveToPosition(wolf.player.position, wolf.moveSpeed);
            wolf.animator.SetBool("IsWalking", true);
        }
        else //stops moving if on edge or wall
        {
            wolf.SetVelocity(new Vector2(0f, wolf.rb.velocity.y));
            wolf.animator.SetBool("IsWalking", false);
        }
    }

    public override void OnExit()
    {
        wolf.animator.SetBool("IsWalking", false);
        wolf.animator.ResetTrigger("Flip");
    }

    public override void Transitions() 
    {
        if(!wolf.IsPlayerInPursueRange()) 
        {
            wolf.TransitionToState(new WolfIdleState(wolf));
        }

        if(wolf.IsPlayerInMeleeRange()) 
        {
            wolf.TransitionToState(new WolfMeleeState(wolf));
        }
    }
}
#endregion

#region Pause State
// Pause for a given time during attacks, face the player
public class WolfPauseState : EnemyState
{
    protected Wolf wolf;
    readonly float startTime = Time.time;    // When did the state start
    readonly float stateTime;    // How long does the state last

    public WolfPauseState(Enemy wolf) : base(wolf) 
    {
        this.wolf = (Wolf)wolf;
        stateTime = this.wolf.attackPauseTime + Random.Range(0f, 0.5f);
    }
    public override void OnEnter()
    {
        wolf.SetVelocity(new Vector2(0f, wolf.rb.velocity.y)); //Stays in place
    }
    
    public override void StateUpdate()
    {
        Transitions();
    }

    public override void StateFixedUpdate()
    {
        wolf.FlipToTarget(wolf.player.position);
    }

    public override void OnExit()
    {
        
    }

    public override void Transitions() 
    {
        if(Time.time >= startTime + stateTime) //If the state time is over
        {
            if(wolf.IsPlayerInPursueRange() && !wolf.IsPlayerInMeleeRange()) 
            {
                wolf.TransitionToState(new WolfPursueState(wolf));
            }

            if(!wolf.IsPlayerInPursueRange()) 
            {
                wolf.TransitionToState(new WolfIdleState(wolf));
            }

            if(wolf.IsPlayerInMeleeRange()) 
            {
                wolf.TransitionToState(new WolfMeleeState(wolf));
            }
        }
    }
}
#endregion


#region Melee State
// Attack the player
public class WolfMeleeState : EnemyState
{
    protected Wolf wolf;
    readonly float startTime = Time.time;    // When did the state start
    readonly float stateTime = 2f;    //How long does the state last

    private Coroutine activeAttackRoutine;
    private bool earlyExit = false;

    public WolfMeleeState(Enemy wolf) : base(wolf) 
    {
        this.wolf = (Wolf)wolf;
    }
    public override void OnEnter()
    {
        wolf.SetVelocity(new Vector2(0f, wolf.rb.velocity.y));
        //Start attack coroutine
        activeAttackRoutine = wolf.StartCoroutine(AttackRoutine());
    }
    
    public override void StateUpdate()
    {
        Transitions();
    }

    public override void StateFixedUpdate()
    {
        
    }

    public override void OnExit()
    {
        if (activeAttackRoutine != null)    //In case if the state ends before the end of coroutine
        {
            wolf.StopCoroutine(activeAttackRoutine);
            wolf.animator.SetBool("AttackStart", false);
            wolf.animator.SetBool("IsAttacking", false);
            wolf.attackCollider.enabled = false;
        }
    }

    public override void Transitions() 
    {
        if(Time.time >= startTime + stateTime) //If the state time is over
        {
            wolf.TransitionToState(new WolfPauseState(wolf));
        }
        if(earlyExit)
        {
            wolf.TransitionToState(new WolfPursueState(wolf));
        }
    }

    //coroutine for attack
    private IEnumerator AttackRoutine() 
    {
        //Start of anticipation, total 0.6 seconds
        wolf.animator.SetBool("AttackStart", true);

        yield return new WaitForSeconds(1f + Random.Range(-0.2f, 0.2f));

        //Do not attack if the player is too far away, exit state
        if(!wolf.IsPlayerInAttackRange()) 
        {
            earlyExit = true;
            yield break;
        }

        //Start of attack, total 1 seconds
        wolf.animator.SetBool("IsAttacking", true);
        wolf.animator.SetBool("AttackStart", false);

        //Turn on and off collider
        yield return new WaitForSeconds(0.1f);
        wolf.attackCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        wolf.attackCollider.enabled = false;
        yield return new WaitForSeconds(0.6f);

        //End of attack
        wolf.animator.SetBool("IsAttacking", false);
    }
}
#endregion