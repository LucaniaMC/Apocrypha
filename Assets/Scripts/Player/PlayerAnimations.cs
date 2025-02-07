using UnityEngine;

//This is a partial class for Player.cs containing functions that handle player animations
//Only Player.cs needs to be attatched to an object

public partial class Player 
{
    [Header("Animators")]       //Used for PlayerAnimator
	public Animator animator;       //Player sprite animator
    public Animator chargeAnimator;	//Animator for charge effect
    [HideInInspector] public float fallTime {get; private set;}		// How long has the player been falling
    
    
    public void SetWalkAnimator(float moveInput) 
    {
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
    }

    public void SetWalkBoolAnimator(bool isWalking) 
    {
        animator.SetBool("IsWalking", isWalking);
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
    
    public void SetSitAnimator(bool isSitting) 
    {
        animator.SetBool("IsSitting", isSitting);
    }

    public void SetChargeEffectAnimator(bool isCharged) 
    {
        chargeAnimator.SetBool("IsCharged", isCharged);
    }

    public void SetKnockbackAnimator(bool knockedBack) 
    {
        animator.SetBool("KnockedBack", knockedBack);
    }

    #region Turn Trigger
    public void SetTurnAnimator()
    {
        if(GroundCheck()) 
        {
            animator.SetTrigger("Turn");
        }
    }

    public void ResetTurnAnimator() 
    {
        animator.ResetTrigger("Turn");
    }
    #endregion

    #region Attack
    public void SetAttackAnimator(bool isAttacking) 
    {
        animator.SetBool("IsAttacking", isAttacking);
    }

    public void SetAltAttack() 
    {
        bool altAttack = animator.GetBool("AltAttack"); //flip the animator bool
        altAttack = !altAttack;
        animator.SetBool("AltAttack", altAttack);
    }

    public void SetChargeAttackAnimator(bool isAttacking) 
    {
        animator.SetBool("IsChargeAttacking", isAttacking);
    }
    #endregion

	#region Fall Time
	public void CalculateFallTime() 
	{
		fallTime += Time.deltaTime;
        animator.SetFloat("FallTime", fallTime);
	}

	public void ResetFallTime()
	{
		fallTime = 0f;
        animator.SetFloat("FallTime", fallTime);
	}
	#endregion
    
}