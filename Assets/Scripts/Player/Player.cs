using UnityEngine;

public partial class Player : MonoBehaviour
{
    [Header("State Machine References")]
    public PlayerData data;
    public PlayerInput input;
    public HealthSystem health;

    [HideInInspector] public PlayerState currentState {get; private set;}   
    [HideInInspector] public PlayerState defaultState {get; private set;}


    #region Loop
    void Start() 
    {
        InitializeMovement();
        InitializeStateMachine();
    }


    void Update() 
    {
        currentState.StateUpdate();
        
        //Universal animators
        SetWalkAnimator(input.MoveInput());   //Needs to be used for state transitions
        SetChargeEffectAnimator(input.CanChargeAttack());   //Is not limited to any state
    }


    void FixedUpdate() 
    {
        currentState.StateFixedUpdate();
    }
    #endregion


    #region State Machine Functions
    // Called in Awake/Start on the player script
    public void InitializeStateMachine() 
    {
        defaultState = new PlayerWalkState(this);
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
