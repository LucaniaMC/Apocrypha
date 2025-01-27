using UnityEngine;

public abstract class State
{
    public abstract void OnEnter();          //Called once when the state is entered
    public abstract void StateUpdate();         //Called every frame in Update
    public abstract void StateFixedUpdate();    //Called in FixexUpdate
    public abstract void OnExit();           //Called once when the state is exited
}