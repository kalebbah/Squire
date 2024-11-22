using System;
using UnityEngine;

[Serializable]
public class HealthHandle
{
    [Header("Health Settings")]
    [SerializeField] private int baseMaxHealth = 100; // Base maximum health
    [SerializeField] private int currentHealth = 100;

    public event Action OnDeath; // Event triggered on death

    public int MaxHealth => baseMaxHealth;
    public int CurrentHealth => currentHealth;

    public HealthHandle(int initialMaxHealth, int initialHealth)
    {
        baseMaxHealth = initialMaxHealth;
        currentHealth = Mathf.Clamp(initialHealth, 0, baseMaxHealth);
    }

    public void ResetHealth()
    {
        currentHealth = MaxHealth;
        Debug.Log($"Health reset to max: {MaxHealth}");
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);
        Debug.Log($"Healed for {amount}. Current health: {currentHealth}");
    }

    public bool TakeDamage(int damage, GameObject owner)
    {
        float finalDamage = damage;

        // Apply specific logic if the damage is coming from an enemy
        if (owner.CompareTag("Enemy"))
        {
            finalDamage = StatManager.Instance.CalculateDamage(damage);
        }

        currentHealth = Mathf.Clamp(currentHealth - Mathf.RoundToInt(finalDamage), 0, MaxHealth);
        Debug.Log("Current health: " + currentHealth + "\n" + "damage taken: " + finalDamage);
        // Trigger health slider update event
        if(owner.CompareTag("Player")) {
            EventManager<HealthHandle>.TriggerEvent(EventKey.UPDATE_SLIDER_HEALTH_DISPLAY, PlayerManager.Instance.User.GetHealthHandle());

        }

        if (IsDead())
        {
            if (owner.CompareTag("Player"))
            {
                // Trigger player-specific death events
                EventManager<EventArgs>.TriggerEvent(EventKey.SAVE_DATA, EventArgs.Empty);
                EventManager<object>.TriggerEvent(EventKey.SHOW_END_SCREEN, null);
                UnityEngine.Object.Destroy(owner);
            }

            // Trigger OnDeath event and return true
            OnDeath?.Invoke();
            Debug.Log($"{owner.name} has died.");
            return true;
        }

        // If not dead, return false
        return false;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}
