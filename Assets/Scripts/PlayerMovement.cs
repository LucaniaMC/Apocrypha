using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
	[Header("Parameters")]
	
    [SerializeField] private float jumpForce = 850f;			// Amount of force added when the player jumps.
	[SerializeField] private float movementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private float dashSpeed = 35f;			//How fast the player can dash
	[Space]
	[SerializeField] private float coyoteTime = 0.1f;			// Time allowed for player to jump after leaving the ground
	[SerializeField] private float wallCoyoteTime = 0.2f;		// Coyote time for wall jump
	[Space]
	[SerializeField] private LayerMask groundLayer;				// A mask determining what is ground to the character
	[SerializeField] private Transform groundCheck;				// A position marking where to check if the player is grounded.

	[SerializeField] private LayerMask wallLayer;				// A mask determining what is wall to the character
	[SerializeField] private Transform wallCheck;				// A position marking where to check if the player is on wall.

	//None of these should show up in inspector
    Vector2 groundCheckSize = new Vector2(.48f, .1f);					//Dimensions of the ground check box size. .48 is the biggest size to not touching walls
	Vector2 wallCheckSize = new Vector2(.1f, 1.5f);						//Dimensions of the wall check box size.
	[HideInInspector] public bool grounded = false;				// Whether or not the player is grounded.
	[HideInInspector] public bool onWall = false;				//Whether the player is on wall.
	[HideInInspector] public bool facingRight = true;	// For determining which way the player is currently facing.
	[HideInInspector] public Rigidbody2D rb;

	[HideInInspector] public float coyoteTimeCounter = 0f;	//Countdown timer for coyote time
	[HideInInspector] public float wallCoyoteTimeCounter = 0f;	//Countdown timer for wall time
	
	private Vector3 velocity = Vector3.zero;
	const float slideVelocity = -5f;	//Wall slide speed
	const float limitVelocity = 25f;	//Limit fall velocity

	private float gravity = 4f;	//Gravity scale for dash, sets to 0 when dashing and back to 4

	[HideInInspector] public float fallTime = 0f;	//Used by player's animator for landing animation variations


	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	public UnityEvent OnJumpEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }


	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnJumpEvent == null)
			OnJumpEvent = new UnityEvent();
	}


    void FixedUpdate()
    {
        //Ground check
		bool wasGrounded = grounded;
		grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, groundCheckSize, 0, groundLayer);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				grounded = true;

				if (!wasGrounded) 
				{
					OnLandEvent.Invoke();
				}
			}
		}

		//Wall check, same mechanics as ground check
		onWall = false;

		Collider2D[] collidersWall = Physics2D.OverlapBoxAll(wallCheck.position, wallCheckSize, 0, wallLayer);
		for (int i = 0; i < collidersWall.Length; i++)
		{
			if (collidersWall[i].gameObject != gameObject)
			{
				onWall = true;
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
		{
			coyoteTimeCounter = Mathf.Clamp(coyoteTimeCounter - Time.deltaTime, 0f, coyoteTime);	//decrease coyote time when airborne, clamp value
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

		//Fall time counter
		if (!grounded && rb.velocity.y < 0) 
		{
			fallTime += Time.deltaTime;
		} 

		//Reset coyote timer if the player hits a wall to avoid triggering both wall jump and coyote time jump. Wall jump should have priority.
		if (onWall && !grounded) 
		{
			coyoteTimeCounter = 0f;
		}

		//Wall slide
		if (onWall) 
		{
			rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, slideVelocity)); //limit downward y velocity to slide speed
		}
    }


	//Reset fall time when grounded and on wall, in LateUpdate so the animator can process it properly before resetting
	void LateUpdate() 
	{
		if (grounded || onWall) 
		{
			fallTime = 0f;
		}
	}


    //Move function
    public void Move(float move, bool jump, bool wallJump, bool dash)
	{
		//Dash
		
		if (dash) 
		{
			if (facingRight) //Dash towards the direction the player is facing
			{
				rb.velocity = new Vector2(dashSpeed, 0f);
				rb.gravityScale = 0f;
			}
			else if (!facingRight) 
			{
				rb.velocity = new Vector2(-dashSpeed, 0f);
				rb.gravityScale = 0f;
			}
		}
		else
		{
			rb.gravityScale = gravity;
		}

		//Only move and change directions if not dashing
		if (!dash)
		{	
			// Move
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move, rb.velocity.y);
			// And then smoothing it out and applying it to the character
			rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !facingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && facingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		
		// If the player should jump...
		if (coyoteTimeCounter > 0f && jump)	//Replaced grounded state check with coyote timer
		{
			// Add a vertical force to the player.
			rb.velocity = new Vector2(rb.velocity.x, 0);	//Reset player veritcal velocity when jumping to prevent irregular jump heights
			rb.AddForce(new Vector2(0f, jumpForce));

			OnJumpEvent.Invoke();
			coyoteTimeCounter = 0f;	//No coyote time after jumping
		}

		//Wall jump
		if (wallCoyoteTimeCounter > 0f && wallJump)
		{
			rb.velocity = new Vector2(rb.velocity.x, 0);	//Reset player veritcal velocity
			rb.AddForce(new Vector2(0f, jumpForce));
		}
	}

	//lower vertical velocity if the player releases jump button early, called in PlayerController
	public void VariableJump() 
	{
		rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 flipScale = transform.localScale;
		flipScale.x *= -1;
		transform.localScale = flipScale;
	}
}