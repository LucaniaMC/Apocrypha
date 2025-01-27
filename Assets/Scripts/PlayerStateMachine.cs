using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerState currentState;
    private PlayerState defaultState;

    private void Awake()
    {
        TransitionToState(defaultState);
    }

    private void Update()
    {
        currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void TransitionToState(PlayerState newState)
    {
        if (currentState != null)
            currentState.ExitState();

        currentState = newState;

        if (currentState != null)
            currentState.EnterState();
    }
}