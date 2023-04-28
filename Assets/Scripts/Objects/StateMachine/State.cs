public abstract class State 
{
    protected readonly StateMachine StateMachine;

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}
