using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
	[Header("Parameters")]
	
    [SerializeField] private float jumpForce = 500f;			// Amount of force added when the player jumps.
	[SerializeField] private float movementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private float coyoteTime = 0.2f;			// Time allowed for player to jump after leaving the ground
	[Space]
	[SerializeField] private LayerMask groundLayer;				// A mask determining what is ground to the character
	[SerializeField] private Transform groundCheck;				// A position marking where to check if the player is grounded.

	//None of these should show up in inspector
    const float groundedRadius = .2f;					// Radius of the overlap circle to determine if grounded
	[HideInInspector] public bool grounded = false;				// Whether or not the player is grounded.
	[HideInInspector] public bool facingRight = true;	// For determining which way the player is currently facing.
	[HideInInspector] public Rigidbody2D rb;

	[HideInInspector] public float coyoteTimeCounter = 0f;	//Countdown timer for coyote time
	
	private Vector3 velocity = Vector3.zero;
	const float limitVelocity = 25f;	//Limit fall velocity

	[HideInInspector] public float fallTime = 0f;


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
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, groundLayer);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				grounded = true;

				if (!wasGrounded) 
				{
					OnLandEvent.Invoke();
				}

				//Reset fall time when grounded
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

		//Fall time counter
		if (!grounded && rb.velocity.y < 0) 
		{
			fallTime += Time.deltaTime;
		} 
    }


	//Reset fall time when grounded
	void LateUpdate() 
	{
		if (grounded) 
		{
			fallTime = 0f;
		}
	}


    //Move function
    public void Move(float move, bool jump)
	{
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
		
		// If the player should jump...
		if (coyoteTimeCounter > 0f && jump)	//Replaced grounded state check with coyote timer
		{
			// Add a vertical force to the player.
			rb.velocity = new Vector2(rb.velocity.x, 0);	//Reset player veritcal velocity when jumping to prevent irregular jump heights
			rb.AddForce(new Vector2(0f, jumpForce));

			OnJumpEvent.Invoke();
			coyoteTimeCounter = 0f;	//No coyote time after jumping
		}
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