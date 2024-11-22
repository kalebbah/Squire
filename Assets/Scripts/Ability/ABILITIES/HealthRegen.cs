using System.Collections;
using UnityEngine;

public class HealthRegen : BaseAbility, IHOT, ISelfTarget
{
    public float Duration { get; set; }    // Duration of health regen
    public float TickInterval { get; set; } // Interval between healing ticks
    public int HealPerTick { get; set; }    // Amount healed per tick

    public HealthRegen(string name, string description)
        : base(name, description)
    {
    }

    public override void Activate(GameObject caster)
    {
        Debug.Log($"{Name} activated on {caster.name}. Applying health regeneration...");
        ApplySelf(caster);
    }
    public override void LevelUp()
    {
        base.LevelUp();
        HealPerTick = Mathf.RoundToInt(HealPerTick + 2); // Scale healing per tick
        Debug.Log($"{Name} leveled up! Heal per tick increased to {HealPerTick}.");
    }

    public void ApplySelf(GameObject caster)
    {
        ApplyHOT(caster);
    }

    public void ApplyHOT(GameObject target)
    {
        if (target.TryGetComponent(out HealthHandle healthHandle))
        {
            Debug.Log($"{Name} is applying a heal over time to {target.name}.");
            AbilityManager.Instance.StartAbilityCoroutine(HOTCoroutine(healthHandle));
        }
        else
        {
            Debug.LogWarning($"{Name} could not find a HealthHandle on {target.name}.");
        }
    }

    private IEnumerator HOTCoroutine(HealthHandle healthHandle)
    {
        float elapsed = 0f;
        if (Duration == 0)
        {
            Debug.Log($"{Name} has infinite duration. Healing will continue indefinitely.");
            while (true)
            {
                healthHandle.Heal(HealPerTick);
                //Debug.Log($"{Name} healed {healthHandle.gameObject.name} for {HealPerTick} HP.");
                elapsed += TickInterval;
                yield return new WaitForSeconds(TickInterval);
            }
        }
        else
        {
            //Debug.Log($"{Name} will heal {healthHandle.transform.gameObject.name} for {Duration} seconds.");
            while (elapsed < Duration)
            {
                healthHandle.Heal(HealPerTick);
                //Debug.Log($"{Name} healed {healthHandle.gameObject.name} for {HealPerTick} HP.");
                elapsed += TickInterval;
                yield return new WaitForSeconds(TickInterval);
            }
            Debug.Log($"{Name} has ended. Total duration: {Duration} seconds.");
        }
    }

    public override string Print()
    {
        string abilityStats = $"{Name} (Level {Level})\n" +
                              $"{Description}\n" +
                              $"Heal Per Tick: {HealPerTick}\n" +
                              $"Tick Interval: {TickInterval}s\n" +
                              $"Duration: {(Duration == 0 ? "Infinite" : $"{Duration}s")}\n";

        Debug.Log(abilityStats);
        return abilityStats;
    }
}
