using UnityEngine;

public abstract class PlayerState
{
    public abstract void OnEnter(Player player, PlayerMovement movement, PlayerData data);          //Called once when the state is entered
    public abstract void StateUpdate(Player player, PlayerMovement movement, PlayerData data);         //Called every frame in Update
    public abstract void StateFixedUpdate(Player player, PlayerMovement movement, PlayerData data);    //Called in FixexUpdate
    public abstract void OnExit(Player player, PlayerMovement movement, PlayerData data);           //Called once when the state is exited
}