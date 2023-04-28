public class StateMachine
{
    public State CurrentState { get; private set; }

    public StateMachine(State startState)
    {
        CurrentState = startState;
        CurrentState.Enter();
    }

    public void SwitchState(State state)
    {
        CurrentState.Exit();

        CurrentState = state;
        CurrentState.Enter();
    }
}