using System;
using UnityEngine;

[Serializable]
public class PlayerHandle : MonoBehaviour
{
    [Header("Currency Settings")]
    [SerializeField] private CurrencyHandle currencyHandle = new CurrencyHandle();
    
    [Header("Health Settings")]
    [SerializeField] private HealthHandle healthHandle = new HealthHandle(100, 100);

    [Header("Experience Settings")]
    [SerializeField] private ExperienceHandle experienceHandle = new ExperienceHandle();

    [Header("Ability Settings")]
    [SerializeField] private AbilityHandle abilityHandle;

    [Header("Collision Settings")]
    [SerializeField] private CollisionHandle collisionHandle;

    private void Awake()
    {
        abilityHandle = GetComponent<AbilityHandle>();
        collisionHandle = GetComponent<CollisionHandle>();
        collisionHandle.OnCollisionEnterAction = HandleCollisionEnter;
    }

    // Player puts damage on Enemy
    private void HandleCollisionEnter(GameObject collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            var enemyCollision = collider.GetComponent<EnemyHandle>();
            if (enemyCollision != null)
            {
                enemyCollision.GetHealthHandle().TakeDamage((int)collisionHandle.collisionDamage, collider);
                DamageTextManager.Instance.DisplayDamageNumber(collisionHandle.collisionDamage, enemyCollision.transform.position);
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        EventManager<HealthHandle>.TriggerEvent(EventKey.UPDATE_SLIDER_HEALTH_DISPLAY, healthHandle);

        if (healthHandle.TakeDamage(damageAmount, gameObject))
        {
            
            EventManager<EventArgs>.TriggerEvent(EventKey.SAVE_DATA, EventArgs.Empty);
            EventManager<object>.TriggerEvent(EventKey.SHOW_END_SCREEN, null);
            Destroy(gameObject);
        }
    }

    public void Heal(int healAmount)
    {
        healthHandle.Heal(healAmount);
    }

    public void Restart()
    {
        healthHandle.ResetHealth();
        experienceHandle.ResetExperienceHandle();
        abilityHandle.ClearAbilities();
    }
    // Public getters for other scripts to access the handles if needed
    public CurrencyHandle GetCurrencyHandle() => currencyHandle;
    public HealthHandle GetHealthHandle() => healthHandle;
    public ExperienceHandle GetExperienceHandle() => experienceHandle;
    public AbilityHandle GetAbilityHandle() => abilityHandle;
}
