using UnityEngine;

//This is a partial class for Player.cs containing functions that handle player animations
//Only Player.cs needs to be attatched to an object

public partial class Player 
{
    public void SetWalkAnimator() 
    {
        animator.SetFloat("Speed", Mathf.Abs(input.moveInput));
    }

    public void SetTurnAnimator()
    {
        animator.SetTrigger("Turn");
    }

    public void SetJumpAnimator(bool isJumping) 
    {
        animator.SetBool("IsJumping", isJumping);
    }

    public void SetFallAnimator(bool isFalling) 
    {
        animator.SetBool("IsFalling", isFalling);
    }
    
}