using UnityEngine;

public class CooldownReduction : BaseAbility
{
    public float ReductionMultiplier { get; set; } = .05f;

    public CooldownReduction(string name, string description)
        : base(name, description)
    {
    }

    public override void LevelUp()
    {
        base.LevelUp();
        StatManager.Instance.AddCooldownReduction(ReductionMultiplier);
        Debug.Log($"{Name} leveled up! Cooldown reduction multiplier is now {ReductionMultiplier}.");
    }
    public override string Print()
    {
        // Generate a string representation of the ability's stats
        string abilityStats = $"{Name} (Level {Level})\n" +
                              $"{Description}\n" +
                              $"Current Cooldown Reduction Multiplier: {ReductionMultiplier:P1}\n" + // Display as percentage
                              $"Cooldown: {Cooldown} seconds\n";

        Debug.Log(abilityStats); // Log the stats for debugging
        return abilityStats; // Return the stats as a string
    }
}
