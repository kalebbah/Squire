using System.Collections;
using UnityEngine;

public class FrostAura : BaseAbility, IAOE, IDOT
{
    public float AreaRadius { get; set; } // Radius of the aura's effect
    public int DamagePerTick { get; set; } // Damage dealt per tick
    public float TickInterval { get; set; } // Interval between damage ticks
    public float Duration { get; set; } // Duration (not used in this implementation)

    // Constructor matching BaseAbility's signature
    public FrostAura(string name, string description, float cooldown, bool isActive)
        : base(name, description, cooldown, isActive) {}

    public override void Activate(GameObject caster)
    {
        Debug.Log($"{Name} activated by {caster.name}. Applying damage over time.");
        ApplyDOT(caster);
    }

    public override void LevelUp()
    {
        base.LevelUp();
        int bonusDamage = DamagePerTick / 2;
        DamagePerTick += bonusDamage; // Increase damage
        AreaRadius += 1f; // Increase area radius
        Debug.Log($"{Name} leveled up! Damage per tick increased to {DamagePerTick} (+{bonusDamage}). Area radius increased to {AreaRadius}.");
    }
    public override string Print()
    {
        string abilityStats = $"{Name} (Level {Level})\n" +
                              $"{Description}\n" +
                              $"Damage Per Tick: {DamagePerTick} (+){DamagePerTick/2}\n" +
                              $"Area Radius: {AreaRadius} (+5) units\n" +
                              $"Tick Interval: {TickInterval} seconds\n" +
                              $"Cooldown: {Cooldown} seconds\n";

        Debug.Log(abilityStats);
        return abilityStats;
    }

    public void ApplyDOT(GameObject caster)
    {
        Debug.Log($"{Name} is applying a damage over time effect in an area.");
        ApplyAOE(caster);
    }

    public void ApplyAOE(GameObject caster)
    {
        Debug.Log($"{Name} is applying an area of effect with radius {AreaRadius}.");
        AbilityManager.Instance.StartAbilityCoroutine(ApplyAuraEffect(caster));
    }

    private IEnumerator ApplyAuraEffect(GameObject caster)
    {
        float elapsed = 0f;
        Debug.Log($"{Name} started. Applying damage every {TickInterval} seconds.");

        while (true) // Infinite duration as per the implementation
        {
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, AreaRadius, LayerMask.GetMask("Enemy"));
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out EnemyHandle enemyHandle))
                {
                    enemyHandle.GetHealthHandle().TakeDamage(DamagePerTick, hit.gameObject);
                    DamageTextManager.Instance.DisplayDamageNumber(DamagePerTick, hit.transform.position);
                    Debug.Log($"{hit.name} took {DamagePerTick} damage from {Name}.");
                }
            }

            elapsed += TickInterval;
            yield return new WaitForSeconds(TickInterval);
        }
    }
}
