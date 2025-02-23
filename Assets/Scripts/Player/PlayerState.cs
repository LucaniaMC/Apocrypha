using UnityEngine;

#region Default State
public abstract class PlayerState
{
    //References
    protected Player player;
    protected PlayerData data;
    protected PlayerInput input;
    protected HealthSystem health;

    public PlayerState(Player player) 
    {
        this.player = player;
        this.data = player.data;
        this.input = player.input;
        this.health = player.health;
    }

    public abstract void OnEnter();              //Called once when the state is entered
    public abstract void StateUpdate();          //Called every frame in Update
    public abstract void StateFixedUpdate();     //Called in FixexUpdate
    public abstract void OnExit();               //Called once when the state is exited

    public virtual void Transitions() {}          //Called in State Update, organize all transitions
}
#endregion


#region Walk State
public class PlayerWalkState : PlayerState
{
    public PlayerWalkState(Player player) : base(player) {}

    public override void OnEnter() 
    {
        player.SetWalkBoolAnimator(true);
        player.DashRefill();    //Refills dash on ground
    }

    public override void StateUpdate() 
    {
        player.SetUpDownAnimator();
        Transitions();
    }

    public override void StateFixedUpdate() 
    {
        //enables horizontal movement
        player.Move(input.MoveInput(), data.runSpeed, data.movementSmoothing);
    }

    public override void OnExit() 
    {
        player.SetWalkBoolAnimator(false);
        player.ResetTurnAnimator(); //Prevents turn animator from staying active when the player leaves the state
    }

    public override void Transitions() 
    {
        if (input.JumpBuffer()) // To jump state
        {
            player.TransitionToState(new PlayerJumpState(player));
        }
        if (!player.GroundCheck())  //To fall state
        {
            player.TransitionToState(new PlayerFallState(player));
            player.SetCoyoteTime();   //Starts coyote time when the player falls from ground
        }
        if (input.DashInput() && player.CanDash())    //To dash state
        {
            player.TransitionToState(new PlayerDashState(player));
        }
        if(input.AttackInput() && input.VerticalInput() == 0) //attack state
            player.TransitionToState(new PlayerRegularAttackState(player));

        if(input.AttackInput() && input.UpInput()) //up attack state
            player.TransitionToState(new PlayerUpAttackState(player));

        if(input.AttackInput() && input.DownInput()) //down attack state
            player.TransitionToState(new PlayerDownAttackState(player));

        if(input.AttackReleaseInput() && input.CanChargeAttack()) //Charge attack state
        {
            player.TransitionToState(new PlayerChargeAttackState(player));
        }
        if(input.SitInput()) //Sit state
        {
            player.TransitionToState(new PlayerSitState(player));
        }
    }
}
#endregion


#region Jump State
public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player) : base(player) {}

    public override void OnEnter() 
    {
        player.Jump(player.jumpForce);
        player.SetJumpAnimator(true);
        player.SpawnJumpParticle();
        input.ResetJumpBuffer();
    }

    public override void StateUpdate() 
    {   
        Transitions();
    }

    public override void StateFixedUpdate() 
    {
        //Enable air movement
        player.Move(input.MoveInput(), data.runSpeed, data.airMovementSmoothing);

        //Jump cut if jump button isn't held down
        if(!input.JumpHoldInput()) 
        {
            player.JumpCut(data.jumpCutRate);
        }
    }

    public override void OnExit() 
    {
        player.SetJumpAnimator(false);
    }

    public override void Transitions() 
    {
        if (player.rb.velocity.y < 0) //To fall state
        {
            player.TransitionToState(new PlayerFallState(player));
        }
        if (player.rb.velocity.y < 0 && player.WallCheck()) //To wall state
        {
            player.TransitionToState(new PlayerWallState(player));
        }
        if (input.DashInput() && player.CanDash())    //To dash state
        {
            player.TransitionToState(new PlayerDashState(player));
        }
    }
}
#endregion


#region Fall State
public class PlayerFallState : PlayerState
{
    public PlayerFallState(Player player) : base(player) {}

    public override void OnEnter() 
    {
        player.SetFallAnimator(true);
    }

    public override void StateUpdate() 
    {
        Transitions();
    }

    public override void StateFixedUpdate() 
    {
        //Enable air movement
        player.Move(input.MoveInput(), data.runSpeed, data.airMovementSmoothing);
        player.LimitFallVelocity(data.limitVelocity);
        player.CalculateFallTime();
    }

    public override void OnExit() 
    {
        player.SetFallAnimator(false);
        if (player.GroundCheck()) 
        {
            player.SpawnLandingParticle();
        }
        player.Invoke("ResetFallTime", 0.02f); //Invoke with a delay so the animator can process the value before it resets to 0
    }

    public override void Transitions() 
    {
        if (player.GroundCheck()) //To walk state
        {
            player.TransitionToState(new PlayerWalkState(player));
        }
        if (player.WallCheck()) //To wall state
        {
            player.TransitionToState(new PlayerWallState(player));
        }
        if (player.CoyoteTime() && input.JumpBuffer()) //To jump state, if jumped during coyote time
        {
            player.TransitionToState(new PlayerJumpState(player));
        }
        if (player.WallCoyoteTime() && input.JumpBuffer()) //To wall jump state, if jumped during wall coyote time
        {
            player.TransitionToState(new PlayerWallJumpState(player));
        }
        if (input.DashInput() && player.CanDash())    //To dash state
        {
            player.TransitionToState(new PlayerDashState(player));
        }
    }
}
#endregion


#region Wall State
public class PlayerWallState : PlayerState
{
    public PlayerWallState(Player player) : base(player) {}

    public override void OnEnter() 
    {
        player.SetWallAnimator(true);
        player.DashRefill();    //refills dash on wall
        player.SetWallParticle(true);
    }

    public override void StateUpdate() 
    {
        Transitions();
    }

    public override void StateFixedUpdate() 
    {
        //Enable air movement
        player.Move(input.MoveInput(), data.runSpeed, data.movementSmoothing);
        player.WallSlide(data.slideVelocity);
    }

    public override void OnExit() 
    {
        player.SetWallAnimator(false);
        player.SetWallParticle(false);
    }

    public override void Transitions() 
    {
        if (player.GroundCheck()) //To walk state
        {
            player.TransitionToState(new PlayerWalkState(player));
        }
        if (!player.WallCheck()) //To wall state
        {
            player.TransitionToState(new PlayerFallState(player));
            player.SetWallCoyoteTime(); //starts wall coyote time when the player falls from the wall
        }
        if (input.JumpBuffer()) // To wall jump state
        {
            player.TransitionToState(new PlayerWallJumpState(player));
        }
    }
}
#endregion


#region Wall Jump State
public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player player) : base(player) {}

    public override void OnEnter() 
    {
        player.Jump(player.wallJumpForce);
        player.SetJumpAnimator(true);
        input.ResetJumpBuffer();
    }

    public override void StateUpdate() 
    {   
        Transitions();
    }

    public override void StateFixedUpdate() 
    {
        //Enable air movement
        player.Move(input.MoveInput(), data.runSpeed, data.airMovementSmoothing);

        //Jump cut
        if(!input.JumpHoldInput()) 
        {
            player.JumpCut(data.jumpCutRate);
        }
    }

    public override void OnExit() 
    {
        player.SetJumpAnimator(false);
    }

    public override void Transitions() 
    {
        if (player.rb.velocity.y < 0) //To fall state
        {
            player.TransitionToState(new PlayerFallState(player));
        }
        if (input.DashInput() && player.CanDash())    //To dash state
        {
            player.TransitionToState(new PlayerDashState(player));
        }
    }
}
#endregion


#region Dash State
public class PlayerDashState : PlayerState
{
    readonly float dashStartTime = Time.time;
    readonly float invisTimeAfterDash = 0.1f;

    public PlayerDashState(Player player) : base(player) {}

    public override void OnEnter() 
    {
        player.FlipToInput(input.MoveInput());    // Allows the player to turn when dashing out of immobile states
        player.SetDashAnimator(true);
        player.SetDashParticle(true);
        player.DashStart();
        health.SetInvincible(data.dashTime + invisTimeAfterDash);   //Player is invincible during and a bit after dash
    }                                
    public override void StateUpdate() 
    {
        if (Time.time >= dashStartTime + data.dashTime) //if dash ended, transition to other states
        {
            Transitions();
        }
    }         
    public override void StateFixedUpdate() 
    {
        player.Dash(data.dashSpeed);
    }    
    
    public override void OnExit() 
    {
        player.SetDashAnimator(false);
        player.SetDashParticle(false);
        player.DashEnd(data.gravity);
    }          

    public override void Transitions() 
    {
        if (player.GroundCheck())
        {
            player.TransitionToState(new PlayerWalkState(player));
        }
        else
        {
            player.TransitionToState(new PlayerFallState(player));
        }
    }                   
}
#endregion


#region Attack State
public abstract class PlayerAttackState : PlayerState
{   
    protected float startTime = Time.time;    // When did the state start
    protected float forwardTime = 0.2f;       // How long the player moves forward
    protected float totalTime = 0.5f;         // Total duration of the attack state
    protected float comboTime = 0.1f;         // Time window for combos/dashing before state end
    protected float moveVelocity = 5f;        // How fast the player moves forward

    public PlayerAttackState(Player player) : base(player) {}

    // Each derived state must provide its own attack collider
    protected abstract Collider2D GetAttackCollider();

    // Each derived state must set its own animator for entering the state
    protected abstract void SetAttackAnimator();

    // And each state must reset its animator on exit
    protected abstract void ResetAttackAnimator();

    public override void OnEnter() 
    {
        player.FlipToInput(input.MoveInput());      // Allows turning during combo
        player.SetVelocity(new Vector2(0f, 0f));    // Reset velocity for consistent movement
        player.Attack(GetAttackCollider(), 0.2f);   // Trigger the attack using the provided collider
        SetAttackAnimator();

        if (!player.facingRight)    // Adjust movement direction based on facing
        {
            moveVelocity = -moveVelocity;
        }
    }

    public override void StateUpdate() 
    {
        Transitions();  
    }

    public override void StateFixedUpdate() 
    {
        // Move forward for forwardTime duration if not at an edge
        if ((Time.time <= startTime + forwardTime) & !player.EdgeCheck())
        {
            player.SetVelocity(new Vector2(moveVelocity, player.rb.velocity.y));
        }
        else //Stops moving
        {
            player.SetVelocity(new Vector2(0f, player.rb.velocity.y));
        }
    }

    public override void OnExit() 
    {
        ResetAttackAnimator();
    }

    public override void Transitions() 
    {
        if (Time.time >= startTime + (totalTime - comboTime)) // When within the combo window, allow chaining of attacks or dashing
        {
            if(input.AttackInput() && input.VerticalInput() == 0) //attack state
                player.TransitionToState(new PlayerRegularAttackState(player));
            if(input.AttackInput() && input.UpInput()) //up attack state
                player.TransitionToState(new PlayerUpAttackState(player));
            if(input.AttackInput() && input.DownInput()) //down attack state
                player.TransitionToState(new PlayerDownAttackState(player));
            if (input.DashInput() && player.CanDash())    // Allows the player dash out of attack state early
                player.TransitionToState(new PlayerDashState(player));
        } 
        if (Time.time >= startTime + totalTime) // After totalTime, transition to the walk state
        {
            player.TransitionToState(new PlayerWalkState(player));
        }
    }
}
#endregion


#region Regular Attack State
public class PlayerRegularAttackState : PlayerAttackState
{
    public PlayerRegularAttackState(Player player) : base(player) {}

    protected override Collider2D GetAttackCollider() => player.attackCollider;

    protected override void SetAttackAnimator() => player.SetAttackAnimator();

    protected override void ResetAttackAnimator() => player.ResetAttackAnimator();

    public override void OnExit()
    {
        // Call base reset logic then perform the alternate attack setup.
        base.OnExit();
        player.SetAltAttack();
    }
}
#endregion


#region Up Attack State
public class PlayerUpAttackState : PlayerAttackState
{
    public PlayerUpAttackState(Player player) : base(player) { }

    protected override Collider2D GetAttackCollider() => player.upAttackCollider;

    protected override void SetAttackAnimator() => player.SetUpAttackAnimator();

    protected override void ResetAttackAnimator() => player.ResetUpAttackAnimator();
}
#endregion


#region Down Attack State
public class PlayerDownAttackState : PlayerAttackState
{
    public PlayerDownAttackState(Player player) : base(player) { }

    protected override Collider2D GetAttackCollider() => player.downAttackCollider;

    protected override void SetAttackAnimator() => player.SetDownAttackAnimator();

    protected override void ResetAttackAnimator() => player.ResetDownAttackAnimator();
}
#endregion


#region Charge Attack State
public class PlayerChargeAttackState : PlayerAttackState
{
    public PlayerChargeAttackState(Player player) : base(player)
    {
        totalTime = 0.7f;  // Longer duration for charge attacks
        moveVelocity = 15f; // Increased forward speed for charge
    }

    protected override Collider2D GetAttackCollider() => player.chargeAttackCollider;

    protected override void SetAttackAnimator() => player.SetChargeAttackAnimator(true);

    protected override void ResetAttackAnimator() => player.SetChargeAttackAnimator(false);
}
#endregion


#region Sit State
public class PlayerSitState : PlayerWalkState
{
    public PlayerSitState(Player player) : base(player) {}

    public override void OnEnter() 
    {
        base.OnEnter();
        player.SetSitAnimator(true);
    }

    public override void StateUpdate() 
    {
        base.StateUpdate();
    }

    public override void StateFixedUpdate() 
    {
        base.StateFixedUpdate();
    }

    public override void OnExit() 
    {
        base.OnExit();
        player.SetSitAnimator(false);
    }

    public override void Transitions() 
    {
        base.Transitions();
        if(input.SitInput() || input.MoveInput() != 0f) // To walk state
        {
            player.TransitionToState(new PlayerWalkState(player));
        }
    }
}
#endregion


#region Knockback State
public class PlayerKnockbackState : PlayerState
{
    public PlayerKnockbackState(Player player, float time) : base(player) 
    {
        stateTime = time;
    }

    readonly float startTime = Time.time;
    readonly float stateTime; //How long does the state last, set in constructor

    public override void OnEnter() 
    {
        player.SetKnockbackAnimator(true);
        health.SetInvincible(stateTime + data.invincibleTime);    // Give player invincible time after state is over, so the player can reposition

        if(input.CanChargeAttack())     // If the player has charged up attack, cancel it so the player has to recharge
        {
            input.CancelChargeAttack();
        }
    }

    public override void StateUpdate() 
    {
        if (Time.time >= startTime + stateTime || (player.GroundCheck() && player.rb.velocity.y < 0)) // Transition to other states when timer is over
        {
            Transitions();
        }
    }

    public override void StateFixedUpdate() 
    {

    }

    public override void OnExit() 
    {
        player.SetKnockbackAnimator(false);
    }

    public override void Transitions() 
    {
        player.TransitionToState(new PlayerFallState(player));
    }
}
#endregion


#region Death State
public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(Player player) : base(player) {}

    public override void OnEnter()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override void StateFixedUpdate()
    {
        
    }

    public override void StateUpdate()
    {
        
    }

    public override void Transitions()
    {
        
    }
}
#endregion
