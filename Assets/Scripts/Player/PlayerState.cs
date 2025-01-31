using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region Default State
public abstract class PlayerState
{
    //References
    protected Player player;
    protected PlayerInput input;
    protected PlayerData data;

    //Constructor set references
    public PlayerState(Player player, PlayerInput input, PlayerData data) 
    {
        this.player = player;
        this.input = input;
        this.data = data;
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
    public PlayerWalkState(Player player, PlayerInput input, PlayerData data) : base(player, input, data) {}

    public override void OnEnter() 
    {
        player.SetWalkBoolAnimator(true);
        player.DashRefill();
    }

    public override void StateUpdate() 
    {
        Transitions();
    }

    public override void StateFixedUpdate() 
    {
        player.Move(input.moveInput, data.runSpeed, data.movementSmoothing);
    }

    public override void OnExit() 
    {
        player.SetWalkBoolAnimator(false);
        player.ResetCoyoteTime();
        player.ResetTurnAnimator(); //Prevents turn animator from staying active when the player leaves the state
    }

    public override void Transitions() 
    {
        if (input.jumpHoldInput) // To jump state
        {
            player.TransitionToState(new PlayerJumpState(player, input, data));
        }
        if (!player.GroundCheck())  //To fall state
        {
            player.TransitionToState(new PlayerFallState(player, input, data));
        }
        if (input.dashInput && player.CanDash())    //To dash state
        {
            player.TransitionToState(new PlayerDashState(player, input, data));
        }
        if(input.attackInput) //test attack state
        {
            player.TransitionToState(new PlayerAttackState(player, input, data));
        }
    }
}
#endregion


#region Jump State
public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player, PlayerInput input, PlayerData data) : base(player, input, data) {}

    public override void OnEnter() 
    {
        player.Jump(data.jumpForce);
        player.SetJumpAnimator(true);
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
            player.TransitionToState(new PlayerFallState(player, input, data));
        }
        if (player.rb.velocity.y < 0 && player.WallCheck()) //To wall state
        {
            player.TransitionToState(new PlayerWallState(player, input, data));
        }
        if (input.dashInput && player.CanDash())    //To dash state
        {
            player.TransitionToState(new PlayerDashState(player, input, data));
        }
    }
}
#endregion


#region Fall State
public class PlayerFallState : PlayerState
{
    public PlayerFallState(Player player, PlayerInput input, PlayerData data) : base(player, input, data) {}

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
        player.Invoke("ResetFallTime", 0.02f); //Invoke with a delay so the animator can process the value before it resets to 0
    }

    public override void Transitions() 
    {
        if (player.GroundCheck()) //To walk state
        {
            player.TransitionToState(new PlayerWalkState(player, input, data));
        }
        if (player.WallCheck()) //To wall state
        {
            player.TransitionToState(new PlayerWallState(player, input, data));
        }
        if (player.CoyoteTime() && input.jumpHoldInput) //To jump state, if jumped during coyote time
        {
            player.TransitionToState(new PlayerJumpState(player, input, data));
        }
        if (input.dashInput && player.CanDash())    //To dash state
        {
            player.TransitionToState(new PlayerDashState(player, input, data));
        }
    }
}
#endregion


#region Wall State
public class PlayerWallState : PlayerState
{
    public PlayerWallState(Player player, PlayerInput input, PlayerData data) : base(player, input, data) {}

    public override void OnEnter() 
    {
        player.SetWallAnimator(true);
        player.DashRefill();
    }

    public override void StateUpdate() 
    {
        Transitions();
    }

    public override void StateFixedUpdate() 
    {
        //Enable air movement
        player.WallSlide(data.slideVelocity);
        player.Move(input.moveInput, data.runSpeed, data.movementSmoothing);
    }

    public override void OnExit() 
    {
        player.SetWallAnimator(false);
    }

    public override void Transitions() 
    {
        if (player.GroundCheck()) //To walk state
        {
            player.TransitionToState(new PlayerWalkState(player, input, data));
        }
        if (!player.WallCheck()) //To wall state
        {
            player.TransitionToState(new PlayerFallState(player, input, data));
        }
        if (input.jumpHoldInput) // To wall jump state
        {
            player.TransitionToState(new PlayerWallJumpState(player, input, data));
        }
    }
}
#endregion


#region Wall Jump State
public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player player, PlayerInput input, PlayerData data) : base(player, input, data) {}

    public override void OnEnter() 
    {
        player.WallJump(data.jumpForce);
        player.SetJumpAnimator(true);
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
            player.TransitionToState(new PlayerFallState(player, input, data));
        }
        if (input.dashInput && player.CanDash())    //To dash state
        {
            player.TransitionToState(new PlayerDashState(player, input, data));
        }
    }
}
#endregion


#region Dash State
public class PlayerDashState : PlayerState
{
    private float dashStartTime;

    public PlayerDashState(Player player, PlayerInput input, PlayerData data) : base(player, input, data) {}

    public override void OnEnter() 
    {
        player.SetDashAnimator(true);
        dashStartTime = Time.time;
        player.DashStart();
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
        player.DashEnd(data.gravity);
    }          

    public override void Transitions() 
    {
        if (player.GroundCheck())
        {
            player.TransitionToState(new PlayerWalkState(player, input, data));
        }
        else
        {
            player.TransitionToState(new PlayerFallState(player, input, data));
        }
    }                   
}
#endregion


#region Attack State
public class PlayerAttackState : PlayerState
{
    public PlayerAttackState(Player player, PlayerInput input, PlayerData data) : base(player, input, data) {}

    float startTime = Time.time;    // When did the state start
    float forwardTime = 0.2f;       // How long does the player move forward
    float totalTime = 0.5f;         // How long does the state last
    float moveVelocity = 8f;      // How fast the player moves forward

    public override void OnEnter() 
    {
        player.rb.velocity = new Vector2(0f, 0f);
        player.SetAttackAnimator(true);

        if (!player.facingRight) //moves player left if facing left
        {
            moveVelocity = -moveVelocity;
        }
    }

    public override void StateUpdate() 
    {
        if (Time.time >= startTime + totalTime) // Transition to other states when timer is over
        {
            Transitions();
        }
    }

    public override void StateFixedUpdate() 
    {
        if ((Time.time <= startTime + forwardTime) & !player.EdgeCheck()) //Move player forward, stops on edge
        {
            player.rb.velocity = new Vector2(moveVelocity, player.rb.velocity.y);
        }
        else //Stops moving
        {
            player.rb.velocity = new Vector2(0f, player.rb.velocity.y);
        }
    }

    public override void OnExit() 
    {
        player.SetAttackAnimator(false);
        player.SetAltAttack();
    }

    public override void Transitions() 
    {
        player.TransitionToState(new PlayerWalkState(player, input, data));
    }
}
#endregion
