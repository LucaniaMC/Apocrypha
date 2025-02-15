using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    [Header("Player Detection")]
    public float pursueRange;    // Range to start pursuing the player
    public float attackRange;   // Range to start attacking the player
    public float meleeRange;    // Close range to start melee attacking the player


    public Transform player {get; private set;}
    protected bool facingRight = true;            // Is the character facing right

    protected EnemyState currentState;  
    protected EnemyState defaultState;


    #region Loop Functions
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;  // Set reference to the player
        Initialize();
    }

    protected virtual void Update() 
    {
        if (currentState != null)
        currentState.StateUpdate();
    }

    protected virtual void FixedUpdate() 
    {
        if (currentState != null)
        currentState.StateFixedUpdate();
    }
    #endregion


    #region State Machine Functions
    // Called in Awake/Start on the player script
    public virtual void Initialize() 
    {
        TransitionToState(defaultState);
    }


	//Exit current state, and enter new state
    public void TransitionToState(EnemyState newState)
    {
        if (currentState != null)
            currentState.OnExit();

        currentState = newState;

        if (currentState != null)
            currentState.OnEnter();
    }
    #endregion


    #region Player Detection
    //returns true if the player is within detection range
    public bool IsPlayerInPursueRange()
    {
        return DistanceToPlayer() <= pursueRange;
    }

    //returns true if the player is within attack range
    public bool IsPlayerInAttackRange() 
    {
        return DistanceToPlayer() <= attackRange;
    }

    //returns true if the player is within melee range
    public bool IsPlayerInMeleeRange() 
    {
        return DistanceToPlayer() <= meleeRange;
    }

    //returns -1 if the player is on the left, 1 if the player is on the right
    public float PlayerDirection()
    {
        return player.position.x < transform.position.x ? -1 : 1;
    }

    //return the distance between the player and enemy
    public float DistanceToPlayer() 
    {
        return Vector2.Distance(transform.position, player.position);
    }
    #endregion


    #region Attack Selection
    public int SelectRandomAttack(int numberOfAttack) 
    {
        int selectedAttack = Random.Range(1, numberOfAttack + 1);
        return selectedAttack;
    }
    #endregion


    // returns -1 when facing left, 1 when facing right
    #region Other
    public float GetFacingDirection() 
    {
        return facingRight ? -1 : 1;
    }


    public virtual void OnDamage() {}
    public virtual void OnDeath() {}
    #endregion
}


#region Ground Enemy
public abstract class GroundEnemy : Enemy
{
    [Header("Movement Parameters")]
    public float moveSpeed = 0f;
    public float knockbackModifier = 1f;
    [Range(0f, 1f)] public float movementSmoothing = 0f;
    public float maxFallVelocity = -25f;

    [Header("Checks")]	//Used for PlayerMovement
	[SerializeField] private LayerMask groundLayer;		// A mask determining what is ground to the character
	[SerializeField] private Transform groundCheck;		// A position marking where to check if the character is grounded.
	[SerializeField] Vector2 groundCheckSize;	        // Dimensions of the ground check box size.
    [Space]
    [SerializeField] private LayerMask edgeLayer;						
	[SerializeField] private Transform edgeCheck;		// A position marking where to check if the player is on edge.	

    //Private variables
    public Rigidbody2D rb {get; private set;}
    protected Vector3 velocity = Vector3.zero;	// Used as ref for movement smoothdamp


    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        base.Start();
    }


    protected override void Update()
    {
        base.Update();
    }


    #region Movement
    // Pursue the player with a given speed with smoothing (x-axis only)
    public void PursuePlayer(float speed)
    {
        float direction; // Value is 1 if target's on the right, -1 on the left
        direction = player.position.x < transform.position.x ? -1 : 1;   

        // Move the object by finding the target velocity, then smooth it out
		Vector3 targetVelocity = new Vector2(direction * speed, rb.velocity.y);
		rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
    }


    // Move to a set position (x-axis only)
    public void MoveToPosition(Vector2 targetPosition, float speed)
    {
        float newX = Mathf.MoveTowards(rb.position.x, targetPosition.x, speed * Time.deltaTime);
        Vector2 newPosition = new Vector2(newX, rb.position.y);
        rb.MovePosition(newPosition);
    }
    #endregion


    #region Jump
    //Jump up vertically
    public void Jump(float jumpHeight) 
    {
        if(jumpHeight <= 0) 	//Jump Height needs to be positive
			Debug.LogError("Invalid jump parameters. jumpHeight needs to be positive.");
		
		float jumpForce = rb.mass * Mathf.Sqrt(-2f * (Physics2D.gravity.y * rb.gravityScale) * jumpHeight);
        rb.velocity = new Vector2(rb.velocity.x, 0f);	// Reset vertical velocity to prevent irregular jump heights.
		rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);		// Add a new vertical force.
    }


    // Jump to a given position with a given height and time
    public void JumpToPosition(float jumpHeight, Vector2 targetPosition)
    {
        if(jumpHeight <= 0) 	//Jump Height needs to be positive
			Debug.LogError("Invalid jump parameters. jumpHeight needs to be positive.");

        //Jump force calculation
        float effectiveGravity = Physics2D.gravity.y * rb.gravityScale;
        float jumpSpeed = Mathf.Sqrt(-2f * effectiveGravity * jumpHeight);
		float jumpForce = rb.mass * jumpSpeed;

        rb.velocity = new Vector2(rb.velocity.x, 0f);
		rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);   //Apply a new vertical force to jump

        //Calculate jump time
        float timeToApex = jumpSpeed / -effectiveGravity;
        float jumpDuration = timeToApex * 2f;

        //Calculate horizontal velocity required to move to the target position during jump duration
        float distanceToTarget = targetPosition.x - transform.position.x;
        float horizontalVelocity = distanceToTarget / jumpDuration;

        //Set horizontal velocity
        rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);
    }
    #endregion


    public void KnockBack() 
    {

    }

    
    // Manually set velocity
    public void SetVelocity(Vector2 velocity)
	{
		rb.velocity = velocity;
	}


	// Hard fall velocity limit
    public void LimitFallVelocity() 
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, maxFallVelocity));
    }


    #region Flip
    // Flip the object to face a target position
	public void FlipToTarget(Vector2 targetPosition) 
	{
        float targetSide = targetPosition.x < transform.position.x ? -1 : 1;

		if (targetSide > 0 && !facingRight)
		{
			Flip();
		}
		else if (targetSide < 0 && facingRight)
		{
			Flip();
		}
	}


    private void Flip()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1 to achieve flipping.
		Vector3 flipScale = transform.localScale;
		flipScale.x *= -1;
		transform.localScale = flipScale;
	}
    #endregion


    #region Ground Check
	//Ground check, return true if the player's grounded
	public bool GroundCheck() 
	{
        if(rb.velocity.y > 0.1f)    //Prevents being immediately grounded after jumping
            return false;

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
}
#endregion


#region Air Enemy
public abstract class AirEnemy : Enemy 
{
    [Header("Movement Parameters")]
    public float moveSpeed = 0f;
    public float knockbackModifier = 1f;
    
    protected Rigidbody2D rb;


    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }


    #region Movement Functions
    // Move to a given position with a given speed
    public void MoveToPosition(Vector2 targetPosition, float speed)
    {
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
        rb.MovePosition(newPosition);
    }


    public void Knockback() 
    {

    }
    #endregion
}
#endregion