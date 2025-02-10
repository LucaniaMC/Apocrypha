using UnityEngine;

public partial class Player : MonoBehaviour
{
    [Header("State Machine References")]
    public PlayerData data;
    public PlayerInput input;
    public HealthSystem health;

    [HideInInspector] public PlayerState currentState {get; private set;}   
    [HideInInspector] public PlayerState defaultState {get; private set;}
    
    [Header("Attack Hitboxes")]     //Used for PlayerCombat
    public Collider2D attackCollider;
    public Collider2D chargeAttackCollider;


    #region Loop
    void Start() 
    {
        defaultState = new PlayerWalkState(this);
        Initialize();
        
        jumpForce = JumpHeightToImpulse(data.jumpHeight);
		wallJumpForce = JumpHeightToImpulse(data.wallJumpHeight);
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
