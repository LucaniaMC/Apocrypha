using UnityEngine;

#region Idle State
// Stands in place
public class WolfIdleState : EnemyState
{
    protected Wolf wolf;

    public WolfIdleState(Enemy enemy, Wolf wolf) : base(enemy) 
    {
        this.wolf = wolf;
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
        if(enemy.IsPlayerInPursueRange()) 
        {
            enemy.TransitionToState(new WolfPursueState(enemy, wolf));
        }
    }
}
#endregion


#region Pursue State
// Move to the player, flipping if needed
public class WolfPursueState : EnemyState
{
    protected Wolf wolf;

    public WolfPursueState(Enemy enemy, Wolf wolf) : base(enemy) 
    {
        this.wolf = wolf;
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
        wolf.MoveToPosition(wolf.player.position, wolf.moveSpeed);
    }

    public override void OnExit()
    {
        
    }

    public override void Transitions() 
    {
        if(!enemy.IsPlayerInPursueRange()) 
        {
            enemy.TransitionToState(new WolfIdleState(enemy, wolf));
        }

        if(enemy.IsPlayerInAttackRange()) 
        {
            enemy.TransitionToState(new WolfLeapState(enemy, wolf));
        }
    }
}
#endregion

#region Pause State
//Pause for a given time during attacks, face the player
public class WolfPauseState : EnemyState
{
    protected Wolf wolf;
    readonly float startTime = Time.time;    // When did the state start
    readonly float stateTime = 2f;

    public WolfPauseState(Enemy enemy, Wolf wolf) : base(enemy) 
    {
        this.wolf = wolf;
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
            if(!enemy.IsPlayerInPursueRange()) 
            {
                enemy.TransitionToState(new WolfIdleState(enemy, wolf));
            }

            if(enemy.IsPlayerInAttackRange()) 
            {
                enemy.TransitionToState(new WolfLeapState(enemy, wolf));
            }

            if(enemy.IsPlayerInPursueRange()) 
            {
                enemy.TransitionToState(new WolfPursueState(enemy, wolf));
            }
        }
    }
}
#endregion


#region Leap State
//Jump towards the player, exit state when grounded
public class WolfLeapState : EnemyState
{
    protected Wolf wolf;

    public WolfLeapState(Enemy enemy, Wolf wolf) : base(enemy) 
    {
        this.wolf = wolf;
    }
    public override void OnEnter()
    {
        wolf.JumpToPosition(wolf.player.position, 1f, 1f);
    }
    
    public override void StateUpdate()
    {
        if(wolf.GroundCheck()) 
        {
            Transitions();
        }
    }

    public override void StateFixedUpdate()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override void Transitions() 
    {
        enemy.TransitionToState(new WolfPauseState(enemy, wolf));
    }
}
#endregion