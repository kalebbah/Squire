using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : BaseAbility, ISingleTarget
{
    public int EffectAmount { get; set; }
    private const float Range = 100.0f;

    public LightningStrike(string name, string description, float cooldown, bool isActive) 
            : base(name, description, cooldown, isActive) {}
    public override void Activate(GameObject caster)
    {
        for(int i=0; i < StatManager.Instance.ProjectileCount; ++i) {
            GameObject target = FindRandomTargetInRange(caster);
            if (target != null)
            {
                ApplySingleTarget(target);
                Debug.Log("Activated: LightningStrike");
            }
            else
            {
                Debug.Log("No enemies in range for LightningStrike.");
            }
        }
        
    }
    public override void LevelUp()
    {
        base.LevelUp();
        EffectAmount += EffectAmount/2; // Increase damage per level
    }
    public override string Print()
    {
        // Display ability stats, level, and the next-level bonus
        string abilityStats = $"{Name} (Level {Level})\n" +
                              $"{Description}\n" +
                              $"Damage: {EffectAmount} (+{EffectAmount / 2} at next level)\n" +
                              $"Range: {Range}\n" +
                              $"Cooldown: {Cooldown}s\n";

        // Print to the console for debugging
        Debug.Log(abilityStats);

        return abilityStats;
    }

    

    private GameObject FindRandomTargetInRange(GameObject caster)
    {
        Collider[] hits = Physics.OverlapSphere(caster.transform.position, Range, LayerMask.GetMask("Enemy"));
        List<GameObject> enemiesInRange = new List<GameObject>();

        foreach (var hit in hits)
        {
            enemiesInRange.Add(hit.gameObject);
        }

        return enemiesInRange.Count > 0 ? enemiesInRange[Random.Range(0, enemiesInRange.Count)] : null;
    }

    public void ApplySingleTarget(GameObject target)
    {
        target.GetComponent<EnemyHandle>().GetHealthHandle().TakeDamage(EffectAmount, target);
        DamageTextManager.Instance.DisplayDamageNumber(EffectAmount, target.transform.position);
        Debug.Log($"{target.name} struck by LightningStrike for {EffectAmount} damage.");
    }
}
