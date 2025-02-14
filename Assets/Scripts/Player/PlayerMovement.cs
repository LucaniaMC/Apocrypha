using UnityEngine;

//This is a partial class for Player.cs containing functions that handle player movements
//Only Player.cs needs to be attatched to an object

public partial class Player
{
	[Header("Checks")]	//Used for PlayerMovement
	[SerializeField] private LayerMask groundLayer;						// A mask determining what is ground to the character
	[SerializeField] private Transform groundCheck;						// A position marking where to check if the player is grounded.
	[SerializeField] Vector2 groundCheckSize = new Vector2(.68f, .1f);	// Dimensions of the ground check box size.
	[Space]
	[SerializeField] private LayerMask wallLayer;						// A mask determining what is wall to the character
	[SerializeField] private Transform wallCheck;						// A position marking where to check if the player is on wall.
	[SerializeField] Vector2 wallCheckSize = new Vector2(.1f, 1.5f);	// Dimensions of the wall check box size.
    [Space]
    [SerializeField] private LayerMask edgeLayer;						
	[SerializeField] private Transform edgeCheck;						// A position marking where to check if the player is on edge.	

	[Header("Rigidbody")]   //Used for PlayerMovement
    public Rigidbody2D rb;	

    //Readable variables
    public bool facingRight {get; private set;} = true;		// For determining which way the player is currently facing.	
	public bool hasAirDashed {get; private set;}				// Keeps the player from dashing in air again if already dashed in air

	//Private variables
	private Vector3 velocity = Vector3.zero;	// Used as ref for movement smoothdamp
	public float jumpForce {get; private set;}	// The force to apply for the player to jump
	public float wallJumpForce {get; private set;}	// The force to apply for the player to wall jump
	private float lastDashTime;					// Used to calculate dash cooldown
	private float lastGroundedTime;	 			// Used for coyote time
	private float lastOnWallTime;				// Used for wall coyote time


    #region Ground Check
	//Ground check, return true if the player's grounded
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
    //Wall check, returns true if the player is on wall
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
    //Edge check, returns true if the player is near an edge
	public bool EdgeCheck() 
	{
		bool onEdge = true;

		//Edge check cast. The player is on edge if a cast to the wallcheck position does not hit anything
		RaycastHit2D[] hitObject = Physics2D.CircleCastAll(edgeCheck.position, 0.1f, Vector2.zero, groundLayer);
		for (int i = 0; i < hitObject.Length; i++)
		{
			if (hitObject[i].collider.gameObject != gameObject)
			{
				onEdge = false;
			}
		}
        return onEdge;
	}
    #endregion


    #region Coyote Time
	// returns true if coyote time is active, allowing the player to jump for a bit after falling off ground
    public bool CoyoteTime()
    {
        return Time.time - lastGroundedTime <= data.coyoteTime;
    }

	// sets coyote time, called after exiting ground state to record last grounded time
	public void SetCoyoteTime() 
	{
		lastGroundedTime = Time.time;
	}
    #endregion


	#region Wall Coyote Time
	// returns true if coyote time is active, allowing the player to jump for a bit after falling off ground
    public bool WallCoyoteTime()
    {
        return Time.time - lastOnWallTime <= data.wallCoyoteTime;
    }

	// sets coyote time, called after exiting ground state to record last grounded time
	public void SetWallCoyoteTime() 
	{
		lastOnWallTime = Time.time;
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

		FlipToInput(moveInput);
	}
	#endregion


	#region Flip
	// If the player is moving towards the opposite direction it's facing, flip the player
	public void FlipToInput(float moveInput) 
	{
		if (moveInput > 0 && !facingRight)
		{
			Flip();
		}
		else if (moveInput < 0 && facingRight)
		{
			Flip();
		}
	}

    //Flip player depending on movement input direction.
	void Flip()
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


	#region Calculate Jump Force
	// Calculate the impulse force needed to reach a given jump height.
	float JumpHeightToImpulse(float jumpHeight) 
	{
		if(jumpHeight <= 0) 	//Jump Height needs to be positive
		{
			Debug.LogError("Invalid jump parameters. jumpHeight needs to be positive.");
			return 0;
		}
		float jumpForce = rb.mass * Mathf.Sqrt(-2f * (Physics2D.gravity.y * rb.gravityScale) * jumpHeight);
		return jumpForce;
	}
	#endregion


    #region Jump
    // Apply a vertical force to the player to jump
	public void Jump(float jumpForce) 
	{
		rb.velocity = new Vector2(rb.velocity.x, 0f);	// Reset player veritcal velocity when jumping to prevent irregular jump heights.
		rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);		// Add a new vertical force.
	}
    #endregion


    #region Jump Cut
    //lower vertical velocity if the player releases jump button early (not holding jump button), so the player can control how high they jump
	public void JumpCut(float jumpCutRate) 
	{	
		rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutRate);
	}
    #endregion


    #region Dash
	// Called at the beginning of dash to set gravity to 0, so the player stays on the same y axis
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

	// Called when entering ground/wall state to enable dash again
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


	#region Set Velocity
	public void SetVelocity(Vector2 velocity)
	{
		rb.velocity = velocity;
	}
	#endregion
}
