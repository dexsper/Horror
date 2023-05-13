using System;
using UnityEngine;

public class ManiacChaseState : ManiacState
{
    private float _attackDelayTimer;

    public bool IsAttack { get; private set; }
    
    public ManiacChaseState(ManiacBehaviour behavior) : base(behavior)
    {
    }

    public void SetAttackDelayTimer(float value)
    {
        _attackDelayTimer = value;
    }
    
    public override void Update()
    {
        base.Update();

        if (Behavior.CurrentTarget == null)
        {
            Behavior.StateMachine.SwitchState(Behavior.PatrolState);
            return;
        }

        Vector3 destination = Behavior.CurrentTarget.transform.position;

        if (Behavior.Agent.destination != destination)
            Behavior.Agent.SetDestination(destination);

        if (_attackDelayTimer > 0f)
        {
            _attackDelayTimer -= Time.deltaTime;
            IsAttack = false;
            
            return;
        }

        if (Vector3.Distance(Behavior.transform.position, destination) <= Behavior.Settings.AttackDistance && _attackDelayTimer <= 0f)
        {
            Behavior.CurrentTarget.Health.Damage(Behavior.Settings.AttackDamage);
            _attackDelayTimer = Behavior.Settings.AttackDelayTime;
            IsAttack = true;
        }
        else
        {
            IsAttack = false;
        }
    }
}