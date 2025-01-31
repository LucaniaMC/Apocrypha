using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a partial class for Player.cs containing functions that handle player movements
//Only Player.cs needs to be attatched to an object

public partial class Player
{	
    //Readable variables
    [HideInInspector] public bool facingRight {get; private set;} = true;		// For determining which way the player is currently facing.	
	[HideInInspector] public bool hasAirDashed {get; private set;}				// Keeps the player from dashing in air again if already dashed in air

	//Private variables
	[HideInInspector] public Vector3 velocity = Vector3.zero;	// Used as ref for movement smoothdamp
	private float lastDashTime;					// Used to calculate dash cooldown
	private float lastGroundedTime;	 			// Used for coyote time


    #region Ground Check
	//Ground check, call in fixed update, return true if the player's grounded
	public bool GroundCheck() 
	{
		bool grounded = false;	//Grounded is false unless the ground check cast hits something

		// Ground check cast, the player is grounded if a cast to the groundcheck position hits anything designated as ground
		Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, groundCheckSize, 0, groundLayer);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)	//Do not check for colliding with self
			{
				grounded = true;
			}
		}
        return grounded;
	}
    #endregion


    #region Wall Check
    //Wall check, called in fixed update, returns true if the player is on wall
	public bool WallCheck() 
	{
		bool onWall = false;

		//Wall check cast. The player is on wall if a cast to the wallcheck position hits anything designated as wall
		Collider2D[] collidersWall = Physics2D.OverlapBoxAll(wallCheck.position, wallCheckSize, 0, wallLayer);
		for (int i = 0; i < collidersWall.Length; i++)
		{
			if (collidersWall[i].gameObject != gameObject)
			{
				onWall = true;
			}
		}
        return onWall;
	}
    #endregion


	#region Edge Check
    //Edge check, called in fixed update, returns true if the player is near an edge
	public bool EdgeCheck() 
	{
		bool onEdge = true;

		//Edge check cast. The player is on edge if a cast to the wallcheck position does not hit anything
		Collider2D[] collidersEdge = Physics2D.OverlapCircleAll(edgeCheck.position, 0.1f, wallLayer);
		for (int i = 0; i < collidersEdge.Length; i++)
		{
			if (collidersEdge[i].gameObject != gameObject)
			{
				onEdge = false;
			}
		}
        return onEdge;
	}
    #endregion


    #region Coyote Time
	// returns true if coyote time is active, allowing the player to jump for a bit after falling off ground, allowing small input inaccuracy
    public bool CoyoteTime()
    {
        return Time.time - lastGroundedTime <= data.coyoteTime;
    }

	public void ResetCoyoteTime() 
	{
		lastGroundedTime = Time.time;
	}
    #endregion


    #region Horizontal Move
    //Horizontally move player, takes player horizontal input float -1 to 1, runSpeed and smoothing from PlayerData
    public void Move(float moveInput, float runSpeed, float smoothing)
	{
		// Move the player by finding the target velocity
		Vector3 targetVelocity = new Vector2(moveInput * runSpeed, rb.velocity.y);
		// And then smoothing it out and applying it to the character
		rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, smoothing);

		// If the player is moving towards the opposite direction it's facing, flip the player
		// So it's clear which way the player's moving, and its attack would align with the movement direction 
		if (moveInput > 0 && !facingRight)
		{
			Flip();
		}
		else if (moveInput < 0 && facingRight)
		{
			Flip();
		}
	}

    //Flip player depending on movement direction, call in Move()
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1 to achieve flipping.
		Vector3 flipScale = transform.localScale;
		flipScale.x *= -1;
		transform.localScale = flipScale;

		SetTurnAnimator();
	}
    #endregion


    #region Jump
    // Apply a vertical force to the player to jump
	public void Jump(float jumpForce) 
	{
		rb.velocity = new Vector2(rb.velocity.x, 0);	// Reset player veritcal velocity when jumping to prevent irregular jump heights.
		rb.AddForce(new Vector2(0f, jumpForce));		// Add a new vertical force to the player to jump.
	}
    #endregion


    #region Wall Jump
	// Add a vertical force to the player to jump on wall
	public void WallJump(float jumpForce) 
	{
		rb.velocity = new Vector2(rb.velocity.x, 0);	// Reset player veritcal velocity when jumping to prevent irregular jump heights.
		rb.AddForce(new Vector2(0, jumpForce));			// Add a new vertical force to the player to jump.W
	}
    #endregion


    #region Jump Cut
    //lower vertical velocity if the player releases jump button early, so the player can control how high they jump
	public void JumpCut(float jumpCutRate) 
	{	
		rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutRate);
	}
    #endregion


    #region Dash
	// Called at the beginning to dash to set gravity to 0, so the player stays on the same y axis
	public void DashStart() 
	{
		rb.gravityScale = 0f;
		hasAirDashed = true;
	}

	// Horizontally moves player at dash speed
	public void Dash(float speed) 
	{
        if (facingRight) //Dash towards the direction the player is facing
        {
            rb.velocity = new Vector2(speed, 0f);
        }
        else if (!facingRight) 
        {
            rb.velocity = new Vector2(-speed, 0f);
        }
	}

	// Called at the end of dash
	public void DashEnd(float gravity) 
	{
		rb.gravityScale = gravity;	// Reset player gravity
		lastDashTime = Time.time;	// Set dash cooldown
	}

	// Called when grounded to enable dash again
	public void DashRefill() 
	{
		hasAirDashed = false;
	}

	// can the player dash, return true if cooldown time is over
	public bool CanDash()
    {
        return !hasAirDashed && Time.time >= lastDashTime + data.dashCooldown;
    }
    #endregion


    #region Wall Slide
	// Limit vertical velocity when going down on wall
    public void WallSlide(float slideVelocity) 
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, slideVelocity));
    }
    #endregion


    #region Limit Velocity
	// Hard fall velocity limit
    public void LimitFallVelocity(float limitVelocity) 
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -limitVelocity));
    }
    #endregion


    #region Knockback
    //knock the player towards a direction and temporarily disables movement control. 
	//Takes input float x as horizontal velocity, y as vertical velocity, and float time >= 0 as time takes for the player to regain control
	public void Knockback(float x, float y, float time)
	{		
		rb.velocity = new Vector2(x, y);	//Set x and y to knockback velocity
	}
    #endregion
}
