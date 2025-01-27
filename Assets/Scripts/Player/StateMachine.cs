using UnityEngine;

public class StateMachine
{
    public State currentState {get; private set;}   
    public State defaultState {get; private set;}


    // Called in Awake on the player script
    public void Initialize () 
    {
        TransitionToState(defaultState);
    }


    public void TransitionToState(State newState)
    {
        if (currentState != null)
            currentState.OnExit();

        currentState = newState;

        if (currentState != null)
            currentState.OnEnter();
    }
}