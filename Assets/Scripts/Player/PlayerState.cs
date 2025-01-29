using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region Default State
public abstract class PlayerState
{
    public abstract void OnEnter(Player player, PlayerData data);                                 //Called once when the state is entered
    public abstract void StateUpdate(Player player, PlayerInput input, PlayerData data);          //Called every frame in Update
    public abstract void StateFixedUpdate(Player player, PlayerInput input, PlayerData data);     //Called in FixexUpdate
    public abstract void OnExit(Player player, PlayerData data);                                  //Called once when the state is exited
}
#endregion


#region Walk State
public class PlayerWalkState : PlayerState
{
    public override void OnEnter(Player player, PlayerData data) 
    {
        player.SetWalkBoolAnimator(true);
    }

    public override void StateUpdate(Player player, PlayerInput input, PlayerData data) 
    {
        //State transitions
        if (input.jumpHoldInput) // To jump state
        {
            player.TransitionToState(new PlayerJumpState());
        }
        if (!player.GroundCheck())  //To fall state
        {
            player.TransitionToState(new PlayerFallState());
        }
        if (input.dashInput && player.CanDash())    //To dash state
        {
            player.TransitionToState(new PlayerDashState());
        }
    }

    public override void StateFixedUpdate(Player player, PlayerInput input, PlayerData data) 
    {
        player.Move(input.moveInput, data.runSpeed, data.movementSmoothing);
    }

    public override void OnExit(Player player, PlayerData data) 
    {
        player.SetWalkBoolAnimator(false);
    }
}
#endregion


#region Jump State
public class PlayerJumpState : PlayerState
{
    public override void OnEnter(Player player, PlayerData data) 
    {
        player.Jump(data.jumpForce);
        player.SetJumpAnimator(true);
    }

    public override void StateUpdate(Player player, PlayerInput input, PlayerData data) 
    {   
        //State transitions
        if (player.rb.velocity.y < 0) //To fall state
        {
            player.TransitionToState(new PlayerFallState());
        }
        if (player.rb.velocity.y < 0 && player.WallCheck()) //To wall state
        {
            player.TransitionToState(new PlayerWallState());
        }
        if (input.dashInput && player.CanDash())    //To dash state
        {
            player.TransitionToState(new PlayerDashState());
        }
    }

    public override void StateFixedUpdate(Player player, PlayerInput input, PlayerData data) 
    {
        //Enable air movement
        player.Move(input.moveInput, data.runSpeed, data.airMovementSmoothing);

        //Jump cut
        if(!input.jumpHoldInput) 
        {
            player.JumpCut(data.jumpCutRate);
        }
    }

    public override void OnExit(Player player, PlayerData data) 
    {
        player.SetJumpAnimator(false);
    }
}
#endregion


#region Fall State
public class PlayerFallState : PlayerState
{
    public override void OnEnter(Player player, PlayerData data) 
    {
        player.SetFallAnimator(true);
    }

    public override void StateUpdate(Player player, PlayerInput input, PlayerData data) 
    {
        //State transitions
        if (player.GroundCheck()) //To walk state
        {
            player.TransitionToState(new PlayerWalkState());
        }
        if (player.WallCheck()) //To wall state
        {
            player.TransitionToState(new PlayerWallState());
        }
        if (player.CoyoteTime() && input.jumpHoldInput) //To jump state, if jumped during coyote time
        {
            player.TransitionToState(new PlayerJumpState());
        }
        if (input.dashInput && player.CanDash())    //To dash state
        {
            player.TransitionToState(new PlayerDashState());
        }
    }

    public override void StateFixedUpdate(Player player, PlayerInput input, PlayerData data) 
    {
        //Enable air movement
        player.Move(input.moveInput, data.runSpeed, data.airMovementSmoothing);
        player.LimitFallVelocity(data.limitVelocity);
    }

    public override void OnExit(Player player, PlayerData data) 
    {
        player.SetFallAnimator(false);
    }
}
#endregion


#region Wall State
public class PlayerWallState : PlayerState
{
    public override void OnEnter(Player player, PlayerData data) 
    {
        player.SetWallAnimator(true);
    }

    public override void StateUpdate(Player player, PlayerInput input, PlayerData data) 
    {
        //State transitions
        if (player.GroundCheck()) //To walk state
        {
            player.TransitionToState(new PlayerWalkState());
        }
        if (!player.WallCheck()) //To wall state
        {
            player.TransitionToState(new PlayerFallState());
        }
        if (input.jumpHoldInput) // To wall jump state
        {
            player.TransitionToState(new PlayerWallJumpState());
        }
    }

    public override void StateFixedUpdate(Player player, PlayerInput input, PlayerData data) 
    {
        //Enable air movement
        player.WallSlide(data.slideVelocity);
        player.Move(input.moveInput, data.runSpeed, data.movementSmoothing);
    }

    public override void OnExit(Player player, PlayerData data) 
    {
        player.SetWallAnimator(false);
    }
}
#endregion


#region Wall Jump State
public class PlayerWallJumpState : PlayerState
{
    public override void OnEnter(Player player, PlayerData data) 
    {
        player.WallJump(data.jumpForce);
        player.SetJumpAnimator(true);
    }

    public override void StateUpdate(Player player, PlayerInput input, PlayerData data) 
    {   
        //State transitions
        if (player.rb.velocity.y < 0) //To fall state
        {
            player.TransitionToState(new PlayerFallState());
        }
        if (input.dashInput && player.CanDash())    //To dash state
        {
            player.TransitionToState(new PlayerDashState());
        }
    }

    public override void StateFixedUpdate(Player player, PlayerInput input, PlayerData data) 
    {
        //Enable air movement
        player.Move(input.moveInput, data.runSpeed, data.airMovementSmoothing);

        //Jump cut
        if(!input.jumpHoldInput) 
        {
            player.JumpCut(data.jumpCutRate);
        }
    }

    public override void OnExit(Player player, PlayerData data) 
    {
        player.SetJumpAnimator(false);
    }
}
#endregion


#region Dash State
public class PlayerDashState : PlayerState
{
    private float dashStartTime;

    public override void OnEnter(Player player, PlayerData data) 
    {
        player.SetDashAnimator(true);
        dashStartTime = Time.time;
        player.DashStart();
    }                                
    public override void StateUpdate(Player player, PlayerInput input, PlayerData data) 
    {
        player.Dash(data.dashSpeed);

        if (Time.time >= dashStartTime + data.dashTime) //if dash ended
        {
            if (player.GroundCheck())
            {
                player.TransitionToState(new PlayerWalkState());
            }
            else
            {
                player.TransitionToState(new PlayerFallState());
            }
        }
    }         
    public override void StateFixedUpdate(Player player, PlayerInput input, PlayerData data) {}    
    public override void OnExit(Player player, PlayerData data) 
    {
        player.SetDashAnimator(false);
        player.DashEnd(data.gravity);
    }                             
}
#endregion
