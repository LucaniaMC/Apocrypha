using UnityEngine;

#region Default State
public abstract class EnemyState
{
    //References
    protected Enemy enemy;

    public EnemyState(Enemy enemy) 
    {
        this.enemy = enemy;
    }

    public abstract void OnEnter();              //Called once when the state is entered
    public abstract void StateUpdate();          //Called every frame in Update
    public abstract void StateFixedUpdate();     //Called in FixexUpdate
    public abstract void OnExit();               //Called once when the state is exited

    public virtual void Transitions() {}          //Called in State Update, organize all transitions
}
#endregion