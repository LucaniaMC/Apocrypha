using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
	[Header("Parameters")]
	
	[SerializeField] private float runSpeed = 10f;				// Player horizontal velocity when running
	[SerializeField] private float movementSmoothing = .05f;	// How much to smooth out the player's movement with Smoothdamp
	[SerializeField] private float airMovementSmoothing = .1f;	// How much to smooth out the player's movement in air with Smoothdamp
	[SerializeField] private float jumpForce = 850f;			// Amount of force added when the player jumps
	[SerializeField] private float jumpCutRate = 0.5f;			// Multiplier for the player's vertical velocity if jump button is released during jump
	[SerializeField] private float dashSpeed = 30f;				// Player horizontal velocity when dashing
	[Space]

	[Header("Assists")]

	[SerializeField] private float coyoteTime = 0.1f;		// In seconds, time allowed for player to jump after leaving the ground
	[SerializeField] private float wallCoyoteTime = 0.2f;	// In seconds, coyote time for wall jump
	[Space]

	[Header("Checks")]

	[SerializeField] private LayerMask groundLayer;						// A mask determining what is ground to the character
	[SerializeField] private Transform groundCheck;						// A position marking where to check if the player is grounded.
	[SerializeField] Vector2 groundCheckSize = new Vector2(.68f, .1f);	// Dimensions of the ground check box size.
	[Space]
	[SerializeField] private LayerMask wallLayer;						// A mask determining what is wall to the character
	[SerializeField] private Transform wallCheck;						// A position marking where to check if the player is on wall.
	[SerializeField] Vector2 wallCheckSize = new Vector2(.1f, 1.5f);	// Dimensions of the wall check box size.
	[Space]

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;		//Functions to call when the player lands
	public UnityEvent OnJumpEvent;		//Functions to call when the player jumps
	public UnityEvent OnWallJumpEvent;	//Functions to call when the player jumps from wall
	public UnityEvent OnFlipEvent;		//Functions to call when the player flips

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }


	//Readable Variables
	public bool grounded {get; private set;} = false;			// Whether the player is grounded
	public bool onWall {get; private set;} = false;				// Whether the player is on wall
	public bool isJumping {get; private set;} = false;			// Is the player jumping
	public bool isWallJumping {get; private set;} = false;		// Is the player wall jumping
	public bool isDashing {get; private set;} = false;			// Is the player dashing
	public bool facingRight {get; private set;} = true;			// For determining which way the player is currently facing.
	public float fallTime {get; private set;} = 0f;				// In seconds, how long has the player been falling. Used by player's animator for landing animation variations
	
	//Timers
	public float coyoteTimeCounter {get; private set;} = 0f;		//Countdown timer for coyote time
	public float wallCoyoteTimeCounter {get; private set;} = 0f;	//Countdown timer for wall time
	public float knockbackTimeCounter {get; private set;} = 0f;		//Timer for player to regain control
	public float forceMoveXCounter {get; private set;} = 0f;		//Timer for forcing player horizontal movement

	//Private variables
	private Vector3 velocity = Vector3.zero;	//Used as ref for movement smoothdamp
	const float slideVelocity = -5f;			//Player's vertical velocity when sliding on wall
	const float limitVelocity = 25f;			//Player's vertical velocity limit when falling
	private float gravity = 4f;					//Player's gravity scale, used for dash, sets to 0 when dashing and back to 4

	//References
	[HideInInspector] public Rigidbody2D rb;
	[HideInInspector] HealthSystem health;


	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		health = GetComponent<HealthSystem>();
	}


    void FixedUpdate()
    {
		bool wasGrounded = grounded;	//This bool updates 1 fixed update after grounded to check if the player has just landed to call events.
		grounded = false;	//Grounded is false unless the ground check cast hits something

		onWall = false;

		//ground & wall check cast
		if (!isJumping) //if the player is jumping, no longer check if grounded to avoid multiple checks
		{
			// Ground check cast, the player is grounded if a cast to the groundcheck position hits anything designated as ground
			Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, groundCheckSize, 0, groundLayer);
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].gameObject != gameObject)	//Do not check for colliding with self
				{
					grounded = true;

					if (!wasGrounded) //If grounded is true, but wasGrounded is false, the player has just landed. Lasts for 1 fixed update
					{
						OnLandEvent.Invoke();
					}
				}
			}

			//Wall check cast. The player is on wall if a cast to the wallcheck position hits anything designated as wall
			Collider2D[] collidersWall = Physics2D.OverlapBoxAll(wallCheck.position, wallCheckSize, 0, wallLayer);
			for (int i = 0; i < collidersWall.Length; i++)
			{
				if (collidersWall[i].gameObject != gameObject)
				{
					onWall = true;
				}
			}
		}


		//Limit fall velocity by clambing the lower bound
		rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -limitVelocity));


		//Coyote time countdown
		if (grounded) 
		{
			coyoteTimeCounter = coyoteTime;	//reset coyote time when grounded
		} 
		else 
		{	//decrease coyote time when airborne, clamps value to prevent it from going above the maximum time, or continue counting down below 0
			coyoteTimeCounter = Mathf.Clamp(coyoteTimeCounter - Time.deltaTime, 0f, coyoteTime);
		}


		//Wall coyote time countdown
		if (onWall) 
		{
			wallCoyoteTimeCounter = wallCoyoteTime;	//reset coyote time when grounded
		} 
		else 
		{
			wallCoyoteTimeCounter = Mathf.Clamp(wallCoyoteTimeCounter - Time.deltaTime, 0f, wallCoyoteTime);
		}


		//Reset coyote timer if the player hits a wall to avoid triggering both wall jump and coyote time jump. Wall jump should have priority in this case.
		if (onWall && !grounded) 
		{
			coyoteTimeCounter = 0f;
		}


		//Wall slide, limits downward y velocity to slide speed on wall
		if (onWall && rb.velocity.y < 0) 
		{
			rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, slideVelocity));
		}


		//Fall time counter, records how long the player has been falling
		if (!grounded && !onWall && rb.velocity.y < 0) 
		{
			fallTime += Time.deltaTime;
		} 


		//If the player startst to fall after jumping, no longer flagged as jumping. Used to enable ground check and jump animation transitions
		if (isJumping && rb.velocity.y < 0f) 
		{
			isJumping = false;
			isWallJumping = false;
		}


		//Knockback time countdown, counts down to 0 over time, no upper bound
		knockbackTimeCounter = Mathf.Max(knockbackTimeCounter - Time.deltaTime, 0f);


		//Force move X countdown, counts down to 0 over time, no upper bound
		forceMoveXCounter = Mathf.Max(forceMoveXCounter - Time.deltaTime, 0f);

    }


	void LateUpdate() 
	{
		//Reset fall time counter when grounded and on wall, in LateUpdate so the animator can read it before it resetting
		if (grounded || onWall) 
		{
			fallTime = 0f;
		}
	}


    //Horizontally move player, takes player horizontal input float -1 to 1, and outputs horizontal velocity
    public void Move(float move)
	{
		//Only move, jump and change directions if not dashing, and if knockback is over
		if (!isDashing && knockbackTimeCounter == 0f && forceMoveXCounter == 0f)
		{
			//Actual movement smoothing amount
			float smoothing;
			// If the player's on ground, use movementSmoothing, otherwise use airMovementSmoothing
			// So the player has less horizontal control in air, for air movement to feel different than ground movement
			if(grounded) 
			{	
				smoothing = movementSmoothing;
			} 
			else 
			{	
				smoothing = airMovementSmoothing;
			}

			// Move the player by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * runSpeed, rb.velocity.y);
			// And then smoothing it out and applying it to the character
			rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, smoothing);

			// If the player is moving towards the opposite direction it's facing, flip the player
			// So it's clear which way the player's moving, and its attack would align with the movement direction 
			if (move > 0 && !facingRight)
			{
				Flip();
			}
			else if (move < 0 && facingRight)
			{
				Flip();
			}
		}
	}


	// Apply a vertical force to the player to jump. Takes player jump input as bool, if jump is true, the player is jumping
	public void Jump(bool jump) 
	{
		if (coyoteTimeCounter > 0f && jump)	//Coyote time functions as ground check
		{
			//Set flag
			isJumping = true;

			rb.velocity = new Vector2(rb.velocity.x, 0);	// Reset player veritcal velocity when jumping to prevent irregular jump heights.
			rb.AddForce(new Vector2(0f, jumpForce));		// Add a new vertical force to the player to jump.

			OnJumpEvent.Invoke();
			coyoteTimeCounter = 0f;	//No coyote time after jumping
		}
	}


	// Add a vertical force to the player to jump on wall. Takes player jump input as bool, if jump is true, the player is jumping
	public void WallJump(bool wallJump) 
	{
		if (wallCoyoteTimeCounter > 0f && wallJump)	//Wall coyote time functions as wall check
		{
			//Set flag
			isJumping = true;
			isWallJumping = true;

			rb.velocity = new Vector2(rb.velocity.x, 0);	// Reset player veritcal velocity when jumping to prevent irregular jump heights.
			rb.AddForce(new Vector2(0, jumpForce));			// Add a new vertical force to the player to jump.

			OnWallJumpEvent.Invoke();
			wallCoyoteTimeCounter = 0f;
		}
	}


	// Horizontally moves player at dash speed. Takes player dash input as bool, if dash is true, the player is dashing
	public void Dash(bool dash) 
	{
		if (dash) 
		{
			//Set flag
			isDashing = true;

			isJumping = false;		//If the player dashes while jumping, no longer jumping
			rb.gravityScale = 0f;	//No gravity during dash so the player stays on the same y axis

			if (facingRight) //Dash towards the direction the player is facing
			{
				rb.velocity = new Vector2(dashSpeed, 0f);
			}
			else if (!facingRight) 
			{
				rb.velocity = new Vector2(-dashSpeed, 0f);
			}
		}
	}


	// Called at the end of dash corontine to set player gravity scale back
	public void DashReset() 
	{
		isDashing = false;
		rb.gravityScale = gravity;
	}


	//lower vertical velocity if the player releases jump button early, so the player can control how high they jump. Called in PlayerController
	public void JumpCut() 
	{	
		if (knockbackTimeCounter == 0f)	//Don't perform jump cut during knockback, since the player should not have control over their movement.
		{
			rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutRate);
		}
	}


	//Flip player depending on movement direction, call in Move()
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		OnFlipEvent.Invoke();

		// Multiply the player's x local scale by -1 to achieve flipping.
		Vector3 flipScale = transform.localScale;
		flipScale.x *= -1;
		transform.localScale = flipScale;
	}

	
	//knock the player towards a direction and temporarily disables movement control. 
	//Takes input float x as horizontal velocity, float y >= 0 as vertical force, and float time >= 0 as time takes for the player to regain control
	public void Knockback(float x, float y, float time)
	{
		if (!health.isDead && !health.isInvincible) //If the player can take damage and isn't dead
		{
			//Sets knockback counter to a given time, player loses horizontal movement and jump control during this time
			knockbackTimeCounter = time;
			//Interrupts jumping
			isJumping = false;
			isWallJumping = false;
			
			rb.velocity = new Vector2(x, 0);	//Set x knockback force, reset y velocity to prevent irregular y velocity
			rb.AddForce(new Vector2(0f, y));	//Used addforce for y axis, because setting y velocity directly doesn't work
		}
	}


	//Force set the player's horizontal velocity, takes float x as velocity, and float time as how long the player is moved
	public void ForceMoveX(float x, float time) 
	{
		forceMoveXCounter = time;

		if (forceMoveXCounter > 0f) 
		{
			rb.velocity = new Vector2(x, rb.velocity.y);
		}
	}
}