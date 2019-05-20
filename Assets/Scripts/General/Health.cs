using System;
using Bolt;
using UnityEngine;
using UnityEngine.Events;

public class Health<T> : Bolt.EntityEventListener<T> where T : IHealthState
{
    public float health = 100f;
//    public string healthProperty = "Health";
    public UnityEvent OnDeath;

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
            } else if (state.Health > health)
            {
                // Healed
            }
            
            Debug.Log("Health Callback: New Health --> " + state.Health);
            
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
        UpdateStateHealth(-damage);
    }

    public void Heal(float healthGained)
    {
        UpdateStateHealth(healthGained);
    }

    private void UpdateStateHealth(float deltaHealth)
    {
        if (entity.IsOwner)
        {
            Debug.Log("Original health property: " + state.Health);
            Debug.Log("Max health: " + maxhealth);
            state.Health = Mathf.Clamp(state.Health + deltaHealth, 0, maxhealth);
            Debug.Log("Setting health property: " + state.Health);
        }
        else
        {
            UpdateEntityHealthEvent e = UpdateEntityHealthEvent.Create(entity.Source, ReliabilityModes.ReliableOrdered);
            e.DeltaHealth = deltaHealth;
            e.Target = entity;
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

    public override void OnEvent(UpdateEntityHealthEvent evnt)
    {
        base.OnEvent(evnt);
        if (entity.IsOwner)
        {
            UpdateStateHealth(evnt.DeltaHealth);
        }
    }
}

public class Health : Health<IHealthState>
{
}