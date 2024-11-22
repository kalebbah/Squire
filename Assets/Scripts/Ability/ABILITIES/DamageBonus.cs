using UnityEngine;

public class DamageBonus : BaseAbility
{
    public float DamageMultiplier { get; private set; } = 0.1f;

    public DamageBonus(string name, string description)
        : base(name, description)
    {
    }

    public override void LevelUp()
    {
        base.LevelUp();
        StatManager.Instance.AddDamageBonus(DamageMultiplier);
        Debug.Log($"{Name} leveled up! Damage bonus multiplier is now {DamageMultiplier}.");
    }
    public override string Print()
    {
        // Generate a string representation of the ability's stats
        string abilityStats = $"{Name} (Level {Level})\n" +
                              $"{Description}\n" +
                              $"Current Damage Multiplier: {DamageMultiplier:F1}\n" +
                              $"Cooldown: {Cooldown} seconds\n";

        Debug.Log(abilityStats); // Log the stats for debugging
        return abilityStats; // Return the stats as a string
    }
}
