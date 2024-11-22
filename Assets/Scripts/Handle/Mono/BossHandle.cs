using UnityEngine;

public class BossHandle : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private HealthHandle healthHandle = new HealthHandle(100, 100);

    private void Start()
    {
        // Notify listeners that the boss has spawned
        EventManager<string>.TriggerEvent(EventKey.BOSS_SPAWNED, gameObject.name);

        // Trigger the initial health update to ensure UI displays the starting health
        EventManager<HealthHandle>.TriggerEvent(EventKey.BOSS_HEALTH_UPDATED, healthHandle);
    }

    public void TakeDamage(int damage)
    {
        // Apply damage to the boss and check if it is dead
        bool isDead = healthHandle.TakeDamage(damage, gameObject);

        // Notify listeners about health updates
        EventManager<HealthHandle>.TriggerEvent(EventKey.BOSS_HEALTH_UPDATED, healthHandle);

        // Handle boss defeat
        if (isDead)
        {
            DefeatBoss();
        }
    }

    private void DefeatBoss()
    {
        // Notify listeners that the boss has been defeated
        EventManager<string>.TriggerEvent(EventKey.BOSS_DEFEATED, gameObject.name);

        // Destroy the boss game object
        Destroy(gameObject);
    }

    public HealthHandle GetHealthHandle()
    {
        // Provide access to the boss's health handle
        return healthHandle;
    }
}
