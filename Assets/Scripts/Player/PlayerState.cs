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

    //Set references
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
    }

    public override void OnExit() 
    {
        player.SetFallAnimator(false);
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
        player.Dash(data.dashSpeed);

        if (Time.time >= dashStartTime + data.dashTime) //if dash ended, transition to other states
        {
            Transitions();
        }
    }         
    public override void StateFixedUpdate() {}    
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
