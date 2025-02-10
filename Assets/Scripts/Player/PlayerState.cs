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

    public abstract void Transitions();          //Called in State Update, organize all transitions
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
        Transitions();
    }

    public override void StateFixedUpdate() 
    {
        //enables horizontal movement
        player.Move(input.moveInput, data.runSpeed, data.movementSmoothing);
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
        if (input.dashInput && player.CanDash())    //To dash state
        {
            player.TransitionToState(new PlayerDashState(player));
        }
        if(input.attackInput) //attack state
        {
            player.TransitionToState(new PlayerAttackState(player));
        }
        if(input.attackReleaseInput && input.CanChargeAttack()) //Charge attack state
        {
            player.TransitionToState(new PlayerChargeAttackState(player));
        }
        if(input.sitInput) //Sit state
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
        player.Move(input.moveInput, data.runSpeed, data.airMovementSmoothing);

        //Jump cut if jump button isn't held down
        if(!input.jumpHoldInput) 
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
        if (input.dashInput && player.CanDash())    //To dash state
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
        player.Move(input.moveInput, data.runSpeed, data.airMovementSmoothing);
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
        if (input.dashInput && player.CanDash())    //To dash state
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
        player.Move(input.moveInput, data.runSpeed, data.movementSmoothing);
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
        player.Move(input.moveInput, data.runSpeed, data.airMovementSmoothing);

        //Jump cut
        if(!input.jumpHoldInput) 
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
        if (input.dashInput && player.CanDash())    //To dash state
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
        player.FlipCheck(input.moveInput);    // Allows the player to turn when dashing out of immobile states
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
public class PlayerAttackState : PlayerState
{
    public PlayerAttackState(Player player) : base(player) {}

    readonly float startTime = Time.time;    // When did the state start
    readonly float forwardTime = 0.2f;       // How long does the player move forward
    readonly float totalTime = 0.5f;         // How long does the state last
    readonly float comboTime = 0.1f;         // Time frame for the player to combo attack or dash out before the end of state
    float moveVelocity = 5f;                 // How fast the player moves forward

    public override void OnEnter() 
    {
        player.FlipCheck(input.moveInput);          // Allows the player to turn during combo
        player.SetVelocity(new Vector2(0f, 0f));    // Reset player velocity for consistent movement
        player.SetAttackAnimator(true);
        player.Attack(player.attackCollider, 0.2f);

        if (!player.facingRight) //moves player left if facing left
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
        if ((Time.time <= startTime + forwardTime) & !player.EdgeCheck()) //Move player forward, stops on edge
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
        player.SetAttackAnimator(false);
        player.SetAltAttack();
    }

    public override void Transitions() 
    {
        if (Time.time >= startTime + (totalTime - comboTime)) //combo time transitions
        {
            if(input.attackInput)       // Start another attack if attacked during combo time
                player.TransitionToState(new PlayerAttackState(player));

            if (input.dashInput && player.CanDash())    // Allows the player dash out of attack state early
                player.TransitionToState(new PlayerDashState(player));
        } 
        if (Time.time >= startTime + totalTime) // To walk state, when state timer is over
        {
            player.TransitionToState(new PlayerWalkState(player));
        }
    }
}
#endregion


#region Charge Attack State
public class PlayerChargeAttackState : PlayerState
{
    public PlayerChargeAttackState(Player player) : base(player) {}

    readonly float startTime = Time.time;    // When did the state start
    readonly float forwardTime = 0.2f;       // How long does the player move forward
    readonly float totalTime = 0.7f;         // How long does the state last
    readonly float comboTime = 0.1f;         // Time frame for the player to combo attack or dash out before the end of state
    float moveVelocity = 15f;                // How fast the player moves forward

    public override void OnEnter() 
    {
        player.FlipCheck(input.moveInput);          // Allows the player to turn during combo
        player.SetVelocity(new Vector2(0f, 0f));    // Reset player velocity for consistent movement
        player.SetChargeAttackAnimator(true);
        player.Attack(player.chargeAttackCollider, 0.2f);

        if (!player.facingRight) //moves player left if facing left
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
        if ((Time.time <= startTime + forwardTime) & !player.EdgeCheck()) //Move player forward, stops on edge
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
        player.SetChargeAttackAnimator(false);
    }

    public override void Transitions() 
    {
        if (Time.time >= startTime + (totalTime - comboTime)) //combo time transitions
        {
            if(input.attackInput)       // Start another attack if attacked during combo time
                player.TransitionToState(new PlayerAttackState(player));

            if (input.dashInput && player.CanDash())    // Allows the player dash out of attack state early
                player.TransitionToState(new PlayerDashState(player));
        } 
        if (Time.time >= startTime + totalTime) // To walk state, when state timer is over
        {
            player.TransitionToState(new PlayerWalkState(player));
        }
    }
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
        if(input.sitInput || input.moveInput != 0f) // To walk state
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
