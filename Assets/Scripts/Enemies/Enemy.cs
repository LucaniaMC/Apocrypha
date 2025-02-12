using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Player Detection")]
    public float pursueRange;    // Range to start pursuing the player
    public float attackRange;   // Range to start attacking the player
    public float meleeRange;    // Close range to start melee attacking the player

    protected Transform player;

    [HideInInspector] public EnemyState currentState {get; private set;}   
    [HideInInspector] public EnemyState defaultState {get; private set;}


    #region Loop Functions
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Initialize();
    }

    protected virtual void Update() {}
    protected virtual void FixedUpdate() {}
    #endregion


    #region State Machine Functions
    // Called in Awake/Start on the player script
    public void Initialize() 
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
    protected bool IsPlayerInPursueRange()
    {
        return Vector2.Distance(transform.position, player.position) <= pursueRange;
    }

    //returns true if the player is within attack range
    protected bool IsPlayerInAttackRange() 
    {
        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    //returns true if the player is within melee range
    protected bool IsPlayerInMeleeRange() 
    {
        return Vector2.Distance(transform.position, player.position) <= meleeRange;
    }

    //returns -1 if the player is on the left, 1 if the player is on the right
    protected float PlayerDirection()
    {
        return player.position.x < transform.position.x ? -1 : 1;
    }
    #endregion


    public virtual void OnDamage() {}
    public virtual void OnDeath() {}
}


#region Ground Enemy
public class GroundEnemy : Enemy
{
    [Header("Movement Parameters")]
    public float moveSpeed = 0f;
    public float knockbackModifier = 1f;
    [Range(0f, 1f)] public float movementSmoothing = 0f;

    [Header("Checks")]	//Used for PlayerMovement
	[SerializeField] private LayerMask groundLayer;		// A mask determining what is ground to the character
	[SerializeField] private Transform groundCheck;		// A position marking where to check if the player is grounded.
	[SerializeField] Vector2 groundCheckSize;	        // Dimensions of the ground check box size.
    [Space]
    [SerializeField] private LayerMask edgeLayer;						
	[SerializeField] private Transform edgeCheck;		// A position marking where to check if the player is on edge.	

    //Private variables
    protected Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;	// Used as ref for movement smoothdamp
    private bool facingRight = true;


    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }


    #region Movement
    // Move to a given position with a given speed (x-axis only)
    public void MoveToPosition(Vector2 targetPosition, float speed)
    {
        float direction; // Value is 1 if target's on the right, -1 on the left
        direction = targetPosition.x < transform.position.x ? -1 : 1;   

        // Move the object by finding the target velocity, then smooth it out
		Vector3 targetVelocity = new Vector2(direction * speed, rb.velocity.y);
		rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
    }


    // Jump to a given position with a given height and time
    public void JumpToPosition(Vector2 targetPosition, float jumpHeight, float timeToTarget)
    {
        if(jumpHeight <= 0 || timeToTarget <= 0)    // Jump Height and Time to target needs to be positive
        {
            Debug.LogError("Invalid jump parameters. jumpHeight or timeToTarget needs to be positive.");
            return;
        }

        Vector2 currentPosition = rb.position;
        
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float verticalVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
        float timeToPeak = verticalVelocity / gravity;
        float totalDescentTime = timeToTarget - timeToPeak;

        // Check if the totalDescentTime is valid (should be positive)
        if (totalDescentTime < 0)
        {
            Debug.LogError("Invalid jump parameters. Increase timeToTarget or decrease jumpHeight.");
            return;
        }
        
        float horizontalVelocity = (targetPosition.x - currentPosition.x) / timeToTarget;
        Vector2 jumpVelocity = new Vector2(horizontalVelocity, verticalVelocity);
        
        // Apply the force as an impulse
        rb.AddForce(jumpVelocity * rb.mass, ForceMode2D.Impulse);
    }
    

    public void KnockBack() 
    {

    }

    
    // Manually set velocity
    public void SetVelocity(Vector2 velocity)
	{
		rb.velocity = velocity;
	}


	// Hard fall velocity limit
    public void LimitFallVelocity(float limitVelocity) 
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -limitVelocity));
    }
    #endregion


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
public class AirEnemy : Enemy 
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