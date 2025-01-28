using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Walk State
public class PlayerWalkState : PlayerState
{
    public override void OnEnter(Player player, PlayerMovement movement, PlayerData data) {}

    public override void StateUpdate(Player player, PlayerMovement movement, PlayerData data) 
    {
        //State transitions
        if (Input.GetButtonDown("Jump")) // To jump state
        {
            player.TransitionToState(new PlayerJumpState());
        }
        if (!movement.GroundCheck()) //To fall state
        {
            player.TransitionToState(new PlayerFallState());
        }
    }

    public override void StateFixedUpdate(Player player, PlayerMovement movement, PlayerData data) 
    {
        movement.Move(Input.GetAxisRaw("Horizontal"), data.runSpeed, data.movementSmoothing);
    }

    public override void OnExit(Player player, PlayerMovement movement, PlayerData data) {}
}
#endregion


#region Jump State
public class PlayerJumpState : PlayerState
{
    public override void OnEnter(Player player, PlayerMovement movement, PlayerData data) 
    {
        movement.Jump(data.jumpForce);
    }

    public override void StateUpdate(Player player, PlayerMovement movement, PlayerData data) 
    {   
        //State transitions
        if (movement.rb.velocity.y < 0) //To fall state
        {
            player.TransitionToState(new PlayerFallState());
        }
        if (movement.WallCheck()) //To wall state
        {
            player.TransitionToState(new PlayerWallState());
        }
    }

    public override void StateFixedUpdate(Player player, PlayerMovement movement, PlayerData data) 
    {
        //Enable air movement
        movement.Move(Input.GetAxisRaw("Horizontal"), data.runSpeed, data.airMovementSmoothing);

        if(!Input.GetButton("Jump")) 
        {
            movement.JumpCut(data.jumpCutRate);
        }
    }

    public override void OnExit(Player player, PlayerMovement movement, PlayerData data) {}
}
#endregion


#region Fall State
public class PlayerFallState : PlayerState
{
    public override void OnEnter(Player player, PlayerMovement movement, PlayerData data) {}

    public override void StateUpdate(Player player, PlayerMovement movement, PlayerData data) 
    {
        //State transitions
        if (movement.GroundCheck()) //To walk state
        {
            player.TransitionToState(new PlayerWalkState());
        }
        if (movement.WallCheck()) //To wall state
        {
            player.TransitionToState(new PlayerWallState());
        }
    }

    public override void StateFixedUpdate(Player player, PlayerMovement movement, PlayerData data) 
    {
        //Enable air movement, and ground check for transitioning back to walk
        movement.Move(Input.GetAxisRaw("Horizontal"), data.runSpeed, data.airMovementSmoothing);
        movement.LimitFallVelocity(data.limitVelocity);
    }

    public override void OnExit(Player player, PlayerMovement movement, PlayerData data) {}
}
#endregion


#region Wall State
public class PlayerWallState : PlayerState
{
    public override void OnEnter(Player player, PlayerMovement movement, PlayerData data) {}

    public override void StateUpdate(Player player, PlayerMovement movement, PlayerData data) 
    {
        //State transitions
        if (movement.GroundCheck()) //To walk state
        {
            player.TransitionToState(new PlayerWalkState());
        }
        if (!movement.WallCheck()) //To wall state
        {
            player.TransitionToState(new PlayerFallState());
        }
        if (Input.GetButtonDown("Jump")) // To wall jump state
        {
            player.TransitionToState(new PlayerWallJumpState());
        }
    }

    public override void StateFixedUpdate(Player player, PlayerMovement movement, PlayerData data) 
    {
        //Enable air movement, and ground check for transitioning back to walk
        movement.WallSlide(data.slideVelocity);
        movement.Move(Input.GetAxisRaw("Horizontal"), data.runSpeed, data.movementSmoothing);
    }

    public override void OnExit(Player player, PlayerMovement movement, PlayerData data) {}
}
#endregion


#region Wall Jump State
public class PlayerWallJumpState : PlayerState
{
    public override void OnEnter(Player player, PlayerMovement movement, PlayerData data) 
    {
        movement.WallJump(data.jumpForce);
    }

    public override void StateUpdate(Player player, PlayerMovement movement, PlayerData data) 
    {   
        //State transitions
        if (movement.rb.velocity.y < 0) //To fall state
        {
            player.TransitionToState(new PlayerFallState());
        }
    }

    public override void StateFixedUpdate(Player player, PlayerMovement movement, PlayerData data) 
    {
        //Enable air movement
        movement.Move(Input.GetAxisRaw("Horizontal"), data.runSpeed, data.airMovementSmoothing);

        if(!Input.GetButton("Jump")) 
        {
            movement.JumpCut(data.jumpCutRate);
        }
    }

    public override void OnExit(Player player, PlayerMovement movement, PlayerData data) {}
}
#endregion
