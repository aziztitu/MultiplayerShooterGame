using System;
using Bolt;
using UnityEngine;
using UnityEngine.Events;

public class Health<T> : Bolt.EntityEventListener<IState> where T : IState
{
    public float health = 100f;
    public string healthProperty = "Health";
    public UnityEvent OnDeath;

    private float maxhealth = 100f;

    public bool isAlive { get; private set; } = true;

    public override void Attached()
    {
        base.Attached();

        if (entity.IsOwner)
        {
            state.SetDynamic(healthProperty, health);    
        }
        
        state.AddCallback(healthProperty, () =>
        {
            float stateHealth = (float) state.GetDynamic(healthProperty);
            if (stateHealth < health)
            {
                // Damage Taken
            } else if (stateHealth > health)
            {
                // Healed
            }
            
            health = stateHealth;
        });
    }

    private void Awake()
    {
        ClampHealth();
    }

    private void Update()
    {
        ClampHealth();

        if (isAlive)
        {
            CheckForDeath();
        }
    }

    private void OnValidate()
    {
        ClampHealth();
    }

    public void TakeDamage(float damage, Vector3 hitPos)
    {
        float stateHealth = (float) state.GetDynamic(healthProperty);
        SetStateHealth(stateHealth - damage);
    }

    public void Heal(float healthGained)
    {
        float stateHealth = (float) state.GetDynamic(healthProperty);
        SetStateHealth(stateHealth + healthGained);
    }

    private void SetStateHealth(float stateHealth)
    {
        stateHealth = Mathf.Clamp(stateHealth, 0, maxhealth);
        if (entity.IsOwner)
        {
            state.SetDynamic(healthProperty, stateHealth);
        }
        else
        {
            UpdatePlayerHealthEvent e = UpdatePlayerHealthEvent.Create(entity.Source, ReliabilityModes.ReliableOrdered);
            e.UpdatedHealth = stateHealth;
            e.TargetPlayerEntity = entity;
            e.Send();
        }
    }

    private void CheckForDeath()
    {
        if (health <= 0)
        {
            isAlive = false;
            OnDeath.Invoke();
        }
    }

    private void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxhealth);
    }

    public override void OnEvent(UpdatePlayerHealthEvent evnt)
    {
        base.OnEvent(evnt);
        if (entity.IsOwner)
        {
            SetStateHealth(evnt.UpdatedHealth);
        }
    }
}

public class Health : Health<IState>
{
}