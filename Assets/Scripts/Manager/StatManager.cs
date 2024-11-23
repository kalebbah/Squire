using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static StatManager Instance { get; private set; }

    //[Header("Global Stat Boost Modifiers")]
    public float DamageBoost { get; set; } = .1f; // HomeScreen boost modifier
    public float HealthBoost { get; set; } = .1f; // HomeScreen boost modifier
    public float LuckBoost { get; set; } = .1f; // HomeScreen boost modifier
    public int DamageBoostLevel { get; set; } = 1; // HomeScreen boost level
    public int HealthBoostLevel { get; set; } = 1; // HomeScreen boost level
    public int LuckBoostLevel { get; set; } = 1; // HomeScreen boost level

    //[Header("Game Passive Stat Modifiers")]
    public float DamageBonus { get; set; }   // Additive damage boost (e.g., +20%)
    public float CooldownReduction { get; set; } // Cooldown reduction percentage (e.g., -10%)
    public int MovementSpeed { get; set; }// Base multiplier for movement speed
    public float HealthRegen { get; set; }   // Health regeneration per second
    public int ProjectileCount { get; set; }  = 2; // Number of additional projectiles

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        EventManager<GameObject>.RegisterEvent(EventKey.PLAYER_INSTANTIATE, PlayerDependency);
        ProjectileCount = 1;
    }

    private void PlayerDependency(GameObject player) {
        MovementSpeed = player.GetComponent<WASDMove>().speed;
    }
    private void Update()
    {
            
    }

    public void AddDamageBoost() {
        DamageBoost += .1f;
        DamageBoostLevel++;
    }

    public void AddHealthBoost() {
        HealthBoost += .1f;
        HealthBoostLevel++;
    }

    public void AddLuckBoost() {
        LuckBoost += .1f;
        LuckBoostLevel++;
    }
    
    public float CalculateHealth(float baseHealth) {
        return baseHealth + (baseHealth * HealthBoost);
    }

    public float CalculateDamage(float baseDamage)
    {
        return baseDamage * (1 + DamageBonus + DamageBoost);
    }

    public float CalculateCooldown(float baseCooldown) {
        return baseCooldown - (baseCooldown * CooldownReduction);
    }

    public void AddDamageBonus(float bonus)
    {
        DamageBonus += bonus;
        Debug.Log($"Damage Bonus increased to {DamageBonus * 100}%");
    }

    public void RemoveDamageBonus(float bonus)
    {
        DamageBonus = Mathf.Max(0, DamageBonus - bonus);
        Debug.Log($"Damage Bonus reduced to {DamageBonus * 100}%");
    }

    public void AddCooldownReduction(float reduction)
    {
        CooldownReduction += reduction;
        Debug.Log($"Cooldown Reduction increased to {CooldownReduction * 100}%");
    }

    public void RemoveCooldownReduction(float reduction)
    {
        CooldownReduction = Mathf.Max(0, CooldownReduction - reduction);
        Debug.Log($"Cooldown Reduction reduced to {CooldownReduction * 100}%");
    }

    public void AddMovementSpeed(int boost)
    {
        MovementSpeed += boost;
        EventManager<float>.TriggerEvent(EventKey.MOVEMENT_SPEED_LVLUP, MovementSpeed);

        Debug.Log($"Movement Speed multiplier increased to {MovementSpeed}");
    }

    public void RemoveMovementSpeed(int boost)
    {
        MovementSpeed = Mathf.Max(1, MovementSpeed - boost);
        Debug.Log($"Movement Speed multiplier reduced to {MovementSpeed}");
    }

    public void AddHealthRegen(float regen)
    {
        HealthRegen += regen;
        Debug.Log($"Health Regeneration increased to {HealthRegen} per second");
    }

    public void RemoveHealthRegen(float regen)
    {
        HealthRegen = Mathf.Max(0, HealthRegen - regen);
        Debug.Log($"Health Regeneration reduced to {HealthRegen} per second");
    }

    public void AddProjectileCount(int count)
    {
        Debug.Log(this.ProjectileCount);
        Debug.Log(count);
        this.ProjectileCount += count;
        Debug.Log($"Projectile count increased to {ProjectileCount}");
    }

    public void RemoveProjectileCount(int count)
    {
        this.ProjectileCount = Mathf.Max(1, ProjectileCount - count);
        Debug.Log($"Projectile count reduced to {ProjectileCount}");
    }
}
