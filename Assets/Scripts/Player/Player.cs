using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement movement;
    public PlayerData data;
    public PlayerState currentState {get; private set;}   
    public PlayerState defaultState {get; private set;}

    public void Start() 
    {
        defaultState = new PlayerWalkState();
        Initialize();
    }


    public void Update() 
    {
        currentState.StateUpdate(this, movement, data);
    }


    public void FixedUpdate() 
    {
        currentState.StateFixedUpdate(this, movement, data);
        print(currentState);
    }


    // Called in Awake/Start on the player script
    public void Initialize () 
    {
        TransitionToState(defaultState);
        defaultState.OnEnter(this, movement, data);
    }


    public void TransitionToState(PlayerState newState)
    {
        if (currentState != null)
            currentState.OnExit(this, movement, data);

        currentState = newState;

        if (currentState != null)
            currentState.OnEnter(this, movement, data);
    }
}
