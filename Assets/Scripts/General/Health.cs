using System;
using Bolt;
using UnityEngine;
using UnityEngine.Events;

public class Health<T> : Bolt.EntityEventListener<T> where T : IHealthState
{
    public float health = 100f;

//    public string healthProperty = "Health";
    public DeathEvent OnDeath;
    
    /// <summary>
    /// The arguments are: float damage, int attackerPlayerId [Only valid in owner]
    /// </summary>
    public event Action<float, int> onDamageTaken;

    public float maxhealth { get; private set; } = 100f;

    public bool isAlive { get; private set; } = true;

    public override void Attached()
    {
        base.Attached();

        if (entity.IsOwner)
        {
            state.Health = health;
        }

        state.AddCallback("Health", () =>
        {
            if (state.Health < health)
            {
                // Damage Taken
            }
            else if (state.Health > health)
            {
                // Healed
            }

            health = state.Health;
        });
    }

    private void Awake()
    {
        ClampHealth();
    }

    private void Update()
    {
        ClampHealth();
    }

    private void OnValidate()
    {
        ClampHealth();
    }

    public void TakeDamage(float damage, int attackerPlayerId)
    {
        UpdateStateHealth(-damage, attackerPlayerId);
    }

    public void Heal(float healthGained)
    {
        UpdateStateHealth(healthGained, -1);
    }

    private void UpdateStateHealth(float deltaHealth, int attackerPlayerId)
    {
        if (entity.IsOwner)
        {
            if (isAlive)
            {
                if (deltaHealth < 0)
                {
                    onDamageTaken?.Invoke(Mathf.Min(health, Mathf.Abs(deltaHealth)), attackerPlayerId);
                }

                state.Health = Mathf.Clamp(state.Health + deltaHealth, 0, maxhealth);
                
                if (state.Health <= 0)
                {
                    isAlive = false;
                    OnDeath.Invoke(attackerPlayerId);
                }
            }
        }
        else
        {
            UpdateEntityHealthEvent e = UpdateEntityHealthEvent.Create(entity.Source, ReliabilityModes.ReliableOrdered);
            e.DeltaHealth = deltaHealth;
            e.Target = entity;
            e.AttackerPlayerId = attackerPlayerId;
            e.Send();
        }
    }

    private void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxhealth);
    }

    public override void OnEvent(UpdateEntityHealthEvent evnt)
    {
        base.OnEvent(evnt);
        if (entity.IsOwner && entity.IsAttached)
        {
            UpdateStateHealth(evnt.DeltaHealth, evnt.AttackerPlayerId);
        }
    }
}

public class Health : Health<IHealthState>
{
}

[Serializable]
public class DeathEvent : UnityEvent<int>
{
}
