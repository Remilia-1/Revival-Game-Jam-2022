using System;
using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    public event Action<CombatUnit> onDieEvent;

    [Header("Combat Unit")]
    [SerializeField] protected uint maxHealth;
    protected uint currentHealth;

    public uint CurrentHealth => currentHealth;


    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Heal(uint healTaken)
    {
        uint newHealth = currentHealth + healTaken;

        if (newHealth >= maxHealth)
            currentHealth = maxHealth;
        else
            currentHealth = newHealth;

        OnHealed();
    }

    /// <summary>
    /// Called when unit is healed
    /// </summary>
    public virtual void OnHealed() { }

    public void Damage(uint damageTaken)
    {
        if(damageTaken > currentHealth)
            damageTaken = currentHealth;

        uint newHealth = currentHealth - damageTaken;

        if (newHealth <= 0)
            KillSelf();
        else
            currentHealth = newHealth;

        OnDamaged();
    }

    /// <summary>
    /// Called when unit is damaged
    /// </summary>
    public virtual void OnDamaged() { }

    protected virtual void KillSelf()
    {
        onDieEvent?.Invoke(this);
        // Thats kinda problematic
        Destroy(this.gameObject);
    }
}