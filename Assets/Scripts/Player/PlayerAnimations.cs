using UnityEngine;

//This is a partial class for Player.cs containing functions that handle player animations
//Only Player.cs needs to be attatched to an object

public partial class Player 
{
    public void SetWalkAnimator(float moveInput) 
    {
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
    }

    public void SetWalkBoolAnimator(bool isWalking) 
    {
        animator.SetBool("IsWalking", isWalking);
    }

    public void SetTurnAnimator()
    {
        if(GroundCheck()) 
        {
            animator.SetTrigger("Turn");
        }
    }

    public void SetJumpAnimator(bool isJumping) 
    {
        animator.SetBool("IsJumping", isJumping);
    }

    public void SetFallAnimator(bool isFalling) 
    {
        animator.SetBool("IsFalling", isFalling);
    }

    public void SetWallAnimator(bool isOnWall) 
    {
        animator.SetBool("OnWall", isOnWall);
    }

    public void SetDashAnimator(bool isDashing) 
    {
        animator.SetBool("IsDashing", isDashing);
    }
    
}