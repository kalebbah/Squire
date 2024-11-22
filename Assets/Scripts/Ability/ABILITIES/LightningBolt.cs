using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBolt : BaseAbility, IProjectile, IChainTarget
{
    public float ProjectileSpeed { get; set; }
    public float EffectAmount { get; set; } = 10; // Default damage
    public int MaxChains { get; set; } = 5; // Maximum chains
    public float ChainRange { get; set; } = 20f; // Range for chaining
    public float SpawnOffset { get; set; } = 1.5f; // Distance in front of the caster

    public LightningBolt(string name, string description, float cooldown, bool isActive)
        : base(name, description, cooldown, isActive) {}

    public override void Activate(GameObject caster)
    {
        for(int i=0; i<StatManager.Instance.ProjectileCount; i++) {
            Debug.Log($"Activating {Name} with max chains: {MaxChains}");

            GameObject target = FindInitialTarget(caster);
            if (target != null)
            {
                LaunchProjectile(caster, target);
            }
            else
            {
                Debug.Log("No enemies in range for LightningBolt.");
            }
        }
        
    }

    public override void LevelUp()
    {
        base.LevelUp();
        int bonus = Mathf.RoundToInt(EffectAmount / 2);
        EffectAmount += bonus; // Increase damage
        MaxChains++;
        Debug.Log($"{Name} leveled up! Effect amount increased to {EffectAmount} (+{bonus}). Max chain effect increased to " + MaxChains);
    }

    public void LaunchProjectile(GameObject caster, GameObject target)
    {
        // Spawn the projectile slightly in front of the caster
        Vector3 spawnPosition = caster.transform.position + caster.transform.forward * SpawnOffset;
        GameObject lightningInstance = AbilityManager.Instance.InstantiateAbilityObject("LightningBoltPrefab", spawnPosition, Quaternion.identity);

        if (lightningInstance != null)
        {
            // Initialize projectile movement toward the target
            ProjectileMove movement = lightningInstance.AddComponent<ProjectileMove>();
            movement.speed = (int)ProjectileSpeed;
            movement.Initialize(target.transform.position);

            // Set up collision handling for chaining
            LightningBoltCollision collision = lightningInstance.AddComponent<LightningBoltCollision>();
            collision.Initialize(this, EffectAmount, MaxChains);

            // Destroy the projectile after 5 seconds
            UnityEngine.Object.Destroy(lightningInstance, 5f);
        }
    }

    private GameObject FindInitialTarget(GameObject caster)
    {
        Collider[] hits = Physics.OverlapSphere(caster.transform.position, 250f, LayerMask.GetMask("Enemy"));
        return hits.Length > 0 ? hits[Random.Range(0, hits.Length)].gameObject : null;
    }

    public void ApplyChain(GameObject initialTarget)
    {
        // Wrapper to start chaining with full chain count
        HashSet<GameObject> hitTargets = new HashSet<GameObject> { initialTarget };
        AbilityManager.Instance.StartAbilityCoroutine(ChainTargets(initialTarget, MaxChains - 1, hitTargets));
    }

    private IEnumerator ChainTargets(GameObject currentTarget, int remainingChains, HashSet<GameObject> hitTargets)
    {
        if (currentTarget == null || remainingChains <= 0) yield break;

        ApplyDamage(currentTarget);

        while (remainingChains > 0)
        {
            GameObject nextTarget = FindNextTarget(currentTarget, hitTargets);
            if (nextTarget == null) yield break;

            hitTargets.Add(nextTarget);
            ApplyDamage(nextTarget);

            // Launch a new projectile for the chain
            LaunchProjectile(currentTarget, nextTarget);

            currentTarget = nextTarget;
            remainingChains--;

            yield return new WaitForSeconds(0.2f); // Delay between chains
        }
    }

    private void ApplyDamage(GameObject target)
    {
        if (target == null) return;

        var healthHandle = target.GetComponent<EnemyHandle>()?.GetHealthHandle();
        if (healthHandle == null)
        {
            Debug.LogWarning($"Target {target.name} has no HealthHandle. Skipping damage.");
            return;
        }

        int damage = Mathf.RoundToInt(StatManager.Instance.CalculateDamage(EffectAmount));
        healthHandle.TakeDamage(damage, target);
        DamageTextManager.Instance.DisplayDamageNumber(damage, target.transform.position);
        Debug.Log($"{target.name} struck by LightningBolt for {damage} damage.");
    }


    private GameObject FindNextTarget(GameObject currentTarget, HashSet<GameObject> hitTargets)
    {
        Collider[] hits = Physics.OverlapSphere(currentTarget.transform.position, ChainRange, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
        {
            if (!hitTargets.Contains(hit.gameObject))
            {
                return hit.gameObject;
            }
        }
        Debug.Log("No valid targets found for chaining.");
        return null;
    }


    public override string Print()
    {
        // Generate a string with current stats and next-level bonus
        string abilityStats = $"{Name} (Level {Level})\n" +
                              $"{Description}\n" +
                              $"Damage: {EffectAmount}\n" +
                              $"Max Chains: {MaxChains}\n" +
                              $"Chain Range: {ChainRange} units\n" +
                              $"Cooldown: {Cooldown}s\n";

        Debug.Log(abilityStats);

        return abilityStats;
    }
}
public class LightningBoltCollision : MonoBehaviour
{
    private LightningBolt lightningAbility;
    private float effectAmount;
    private int remainingChains;

    public void Initialize(LightningBolt lightning, float effectAmount, int maxChains)
    {
        lightningAbility = lightning;
        this.effectAmount = effectAmount;
        this.remainingChains = maxChains;
        Debug.Log($"LightningBoltCollision initialized with effectAmount: {effectAmount}, maxChains: {maxChains}");
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject enemy = collision.gameObject;

            // Apply damage directly to the first enemy
            int damage = Mathf.RoundToInt(StatManager.Instance.CalculateDamage(effectAmount));
            enemy.GetComponent<EnemyHandle>().GetHealthHandle().TakeDamage(damage, enemy);
            DamageTextManager.Instance.DisplayDamageNumber(damage, enemy.transform.position);
            Debug.Log($"{enemy.name} struck by LightningBolt for {damage} damage.");

            // Start chaining from the hit target
            if (remainingChains > 0)
            {
                lightningAbility.ApplyChain(enemy);
            }

            // Destroy the projectile after hitting the first enemy in the chain
            Destroy(gameObject);
        }
    }

}
