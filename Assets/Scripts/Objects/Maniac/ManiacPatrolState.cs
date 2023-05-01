using UnityEngine;

public class ManiacPatrolState : ManiacState
{
    private Generator _targetGenerator;
    private int _nextGeneratorIndex = 0;
    private float _sleepTime = 0f;

    public ManiacPatrolState(ManiacBehaviour behavior) : base(behavior)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _sleepTime = Behavior.Settings.PatrolSleepTime;

        NextGenerator();
    }

    public override void Exit()
    {
        base.Exit();

        _sleepTime = 0f;
    }

    public override void Update()
    {
        base.Update();

        if (Behavior.CurrentTarget != null)
        {
            Behavior.StateMachine.SwitchState(Behavior.ChaseState);
            return;
        }

        if (_sleepTime > 0f)
        {
            _sleepTime -= Time.deltaTime;
            return;
        }

        if (_targetGenerator == null)
            NextGenerator();

        if (Behavior.Agent.destination != _targetGenerator.transform.position)
            Behavior.Agent.SetDestination(_targetGenerator.transform.position);

        if (Vector3.Distance(Behavior.transform.position, _targetGenerator.transform.position) <= (Behavior.Agent.stoppingDistance * 1.5f))
        {
            NextGenerator();

            _sleepTime = Behavior.Settings.PatrolSleepTime;
        }
    }

    private void NextGenerator()
    {
        _targetGenerator = Generator.Generators[_nextGeneratorIndex];

        _nextGeneratorIndex = (_nextGeneratorIndex + 1) % Generator.Generators.Count;
    }
}