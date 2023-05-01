public class ManiacChaseState : ManiacState
{
    public ManiacChaseState(ManiacBehaviour behavior) : base(behavior)
    {
    }

    public override void Update()
    {
        base.Update();

        if(Behavior.CurrentTarget == null)
        {
            Behavior.StateMachine.SwitchState(Behavior.PatrolState);
            return;
        }

        Behavior.Agent.SetDestination(Behavior.CurrentTarget.transform.position);
    }
}