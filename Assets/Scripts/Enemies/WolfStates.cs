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

        if (!wolf.EdgeCheck()) 
        {
            wolf.MoveToPosition(wolf.player.position, wolf.moveSpeed);
        }
        else 
        {
            wolf.SetVelocity(new Vector2(0f, wolf.rb.velocity.y));
        }
    }

    public override void OnExit()
    {
        
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
    float stateTime;    //How long does the state last

    public WolfPauseState(Enemy wolf) : base(wolf) 
    {
        this.wolf = (Wolf)wolf;
    }
    public override void OnEnter()
    {
        stateTime = wolf.attackPauseTime;
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
    readonly float stateTime = 1f;    //How long does the state last

    public WolfMeleeState(Enemy wolf) : base(wolf) 
    {
        this.wolf = (Wolf)wolf;
    }
    public override void OnEnter()
    {
        wolf.SetVelocity(new Vector2(0f, wolf.rb.velocity.y));
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
        if(Time.time >= startTime + stateTime) //If the state time is over
        {
            wolf.TransitionToState(new WolfPauseState(wolf));
        }
    }
}
#endregion