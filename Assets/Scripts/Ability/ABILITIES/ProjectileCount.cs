using UnityEngine;

public class ProjectileCount : BaseAbility
{
    public int projectileCount { get; set; } = 1;

    public ProjectileCount(string name, string description)
        : base(name, description)
    {
        projectileCount = 2;
    }

    public override void LevelUp()
    {
        base.LevelUp();
        StatManager.Instance.AddProjectileCount(projectileCount);
        Debug.Log($"{Name} leveled up! Projectile count increased to {StatManager.Instance.ProjectileCount}.");
    }
    public override string Print()
    {
        // Generate a string with current level stats and next level bonus
        string abilityStats = $"{Name} (Level {Level})\n" +
                              $"{Description}\n" +
                              $"Projectile Count: {StatManager.Instance.ProjectileCount} (+{projectileCount} at next level)\n";

        // Print the stats to the console
        Debug.Log(abilityStats);

        return abilityStats;
    }
}
