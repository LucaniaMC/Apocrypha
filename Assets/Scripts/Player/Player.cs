using UnityEngine;

public partial class Player : MonoBehaviour
{
    [Header("State Machine References")]

    public PlayerData data;
    public PlayerInput input;
    public HealthSystem health;

    [HideInInspector] public PlayerState currentState {get; private set;}   
    [HideInInspector] public PlayerState defaultState {get; private set;}

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
    
    [Header("Attack Hitboxes")]     //Used for PlayerCombat
    public Collider2D attackCollider;
    public Collider2D chargeAttackCollider;


    #region Loop
    void Start() 
    {
        defaultState = new PlayerWalkState(this);
        Initialize();
    }


    void Update() 
    {
        currentState.StateUpdate();
        
        //Universal animators
        SetWalkAnimator(input.moveInput);   //Needs to be used for state transitions
        SetChargeEffectAnimator(input.CanChargeAttack());   //Is not limited to any state
    }


    void FixedUpdate() 
    {
        currentState.StateFixedUpdate();
    }
    #endregion


    #region State Machine Functions
    // Called in Awake/Start on the player script
    public void Initialize() 
    {
        TransitionToState(defaultState);
    }


	//Exit current state, and enter new state
    public void TransitionToState(PlayerState newState)
    {
        if (currentState != null)
            currentState.OnExit();

        currentState = newState;

        if (currentState != null)
            currentState.OnEnter();
    }
    #endregion
}
