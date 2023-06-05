using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField, Range(1f, 1000f)] private float _maxHealth = 100f;

    [field: HideLabel]
    [field: ProgressBar(0, nameof(_maxHealth), ColorGetter = nameof(GetHealthBarColor), Height = 30)]
    [field: SyncVar(OnChange = nameof(On_HealthChange))]
    public float CurrentHealth { get; private set; }

    public bool IsDead { get; private set; }
    public event Action OnDead;
    public event Action OnRestored;

    [Server]
    public void Damage(float amount)
    {
        if (IsDead)
            return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, 100f);
    }

    [Server]
    public void Restore()
    {
        CurrentHealth = _maxHealth;
    }

    private void On_HealthChange(float prev, float next, bool asServer)
    {
        if (next <= 0f && !IsDead)
        {
            IsDead = true;
            AnalyticsEventManager.OnEvent("Enter the jail","Jailed","1");
            OnDead?.Invoke();
        }

        if (next > 0f && IsDead)
        {
            IsDead = false;
            
            OnRestored?.Invoke();
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        if (CurrentHealth > _maxHealth)
        {
            CurrentHealth = _maxHealth;
        }
    }
    private Color GetHealthBarColor(float value)
    {
        return Color.Lerp(Color.red, Color.green, Mathf.Pow(value / _maxHealth, 2));
    }
}