using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    [SerializeField] protected uint maxHealth;
    protected uint currentHealth;

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
    }

    public void Damage(uint damageTaken)
    {
        uint newHealth = currentHealth - damageTaken;

        if (newHealth <= 0)
            KillSelf();
        else
            currentHealth = newHealth;
    }

    private void KillSelf()
    {
        // Thats kinda problematic
        Destroy(this.gameObject);
    }
}