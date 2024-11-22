using UnityEngine;

public class MovementSpeed : BaseAbility, ISelfTarget
{
    public int SpeedMultiplier { get; set; } = 1; // Default multiplier

    public MovementSpeed(string name, string description)
        : base(name, description)
    {
    }

    public override void Activate(GameObject caster) {
        ApplySelf(caster);
    }

    public void ApplySelf(GameObject caster)
    {

    }

    public override void LevelUp()
    {
        base.LevelUp();
        StatManager.Instance.AddMovementSpeed(SpeedMultiplier);
        Debug.Log($"{Name} leveled up! Movement speed multiplier is now {SpeedMultiplier}.");
    }
    public override string Print()
    {
        // Generate a string with current level stats and next level bonus
        string abilityStats = $"{Name} (Level {Level})\n" +
                              $"{Description}\n" +
                              $"Speed Multiplier: {StatManager.Instance.MovementSpeed} (+{SpeedMultiplier} at next level)\n";

        // Print the stats to the console
        Debug.Log(abilityStats);

        return abilityStats;
    }
}
