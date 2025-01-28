using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Checks")]

	[SerializeField] private LayerMask groundLayer;						// A mask determining what is ground to the character
	[SerializeField] private Transform groundCheck;						// A position marking where to check if the player is grounded.
	[SerializeField] Vector2 groundCheckSize = new Vector2(.68f, .1f);	// Dimensions of the ground check box size.
	[Space]
	[SerializeField] private LayerMask wallLayer;						// A mask determining what is wall to the character
	[SerializeField] private Transform wallCheck;						// A position marking where to check if the player is on wall.
	[SerializeField] Vector2 wallCheckSize = new Vector2(.1f, 1.5f);	// Dimensions of the wall check box size.


    //Readable variables
    public bool facingRight {get; private set;} = true;			// For determining which way the player is currently facing.

	//Private variables
	private Vector3 velocity = Vector3.zero;	//Used as ref for movement smoothdamp

	//References
	public Rigidbody2D rb;


	private void Update() 
	{
		GroundCheck();
		WallCheck();
	}


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
	}


    // Apply a vertical force to the player to jump. Takes player jump input as bool, if jump is true, the player is jumping
	public void Jump(float jumpForce) 
	{
		rb.velocity = new Vector2(rb.velocity.x, 0);	// Reset player veritcal velocity when jumping to prevent irregular jump heights.
		rb.AddForce(new Vector2(0f, jumpForce));		// Add a new vertical force to the player to jump.

		//coyoteTimeCounter = 0f;	//No coyote time after jumping
	}


	// Add a vertical force to the player to jump on wall. Takes player jump input as bool, if jump is true, the player is jumping
	public void WallJump(float jumpForce) 
	{
		rb.velocity = new Vector2(rb.velocity.x, 0);	// Reset player veritcal velocity when jumping to prevent irregular jump heights.
		rb.AddForce(new Vector2(0, jumpForce));			// Add a new vertical force to the player to jump.W
	}


	// Horizontally moves player at dash speed. Takes player dash input as bool, if dash is true, the player is dashing continously
	public void Dash(float speed) 
	{
        rb.gravityScale = 0f;	//No gravity during dash so the player stays on the same y axis

        if (facingRight) //Dash towards the direction the player is facing
        {
            rb.velocity = new Vector2(speed, 0f);
        }
        else if (!facingRight) 
        {
            rb.velocity = new Vector2(-speed, 0f);
        }
	}


	// Called at the end of dash to set player gravity scale back
	public void DashReset(float gravity) 
	{
		rb.gravityScale = gravity;
	}


	//lower vertical velocity if the player releases jump button early, so the player can control how high they jump. Called in PlayerController
	public void JumpCut(float jumpCutRate) 
	{	
		rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutRate);
	}


    public void WallSlide(float slideVelocity) 
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, slideVelocity));
    }


    public void LimitFallVelocity(float limitVelocity) 
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -limitVelocity));
    }


    //knock the player towards a direction and temporarily disables movement control. 
	//Takes input float x as horizontal velocity, y as vertical velocity, and float time >= 0 as time takes for the player to regain control
	public void Knockback(float x, float y, float time)
	{		
		rb.velocity = new Vector2(x, y);	//Set x and y to knockback velocity
	}
}
