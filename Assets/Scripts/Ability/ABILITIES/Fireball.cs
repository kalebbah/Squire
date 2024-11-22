using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Fireball : BaseAbility, IProjectile, IDOT
{
    public float ProjectileSpeed { get; set; }
    public int DamagePerTick { get; set; }
    public float TickInterval { get; set; }
    public float Duration { get; set; }
    public float EffectAmount { get; set; }
    public float SpawnOffset { get; set; } = 1.5f; // Distance in front of the caster

    public Fireball(string name, string description, float cooldown, bool isActive)
        : base(name, description, cooldown, isActive) {}

    public override void Activate(GameObject caster)
    {
        for(int i=0; i < StatManager.Instance.ProjectileCount; i++) {
            GameObject target = FindRandomTargetInRange(caster);
            if (target != null)
            {
                LaunchProjectile(caster, target);
                Debug.Log("Activated: Fireball"); 
            }
            else
            {
                Debug.Log("No enemies in range for Fireball.");
            }
        }
        
    }
    public override void LevelUp()
    {
        base.LevelUp();
        DamagePerTick += 5; // Increase damage per level
        EffectAmount += EffectAmount/2; // Increase speed per level
    }
    public override string Print()
    {
        string abilityStats = $"{Name} (Level {Level})\n" +
                              $"{Description}\n" +
                              $"Projectile Speed: {ProjectileSpeed}\n" +
                              $"Damage Per Tick: {DamagePerTick} (+2)\n" +
                              $"Tick Interval: {TickInterval} seconds\n" +
                              $"Duration: {Duration} seconds\n" +
                              $"Effect Amount: {EffectAmount} (+){EffectAmount/2}\n" +
                              $"Spawn Offset: {SpawnOffset}\n" +
                              $"Cooldown: {Cooldown} seconds\n";

        Debug.Log(abilityStats);
        return abilityStats;
    }

    private GameObject FindRandomTargetInRange(GameObject caster)
    {
        Collider[] hits = Physics.OverlapSphere(caster.transform.position, 250f, LayerMask.GetMask("Enemy"));
        return hits.Length > 0 ? hits[Random.Range(0, hits.Length)].gameObject : null;
    }

    public void LaunchProjectile(GameObject caster, GameObject target)
    {
        // Position the fireball slightly in front of the caster
        Vector3 spawnPosition = caster.transform.position + caster.transform.forward * SpawnOffset;
        GameObject fireballInstance = AbilityManager.Instance.InstantiateAbilityObject("FireballPrefab", spawnPosition, Quaternion.identity);

        if (fireballInstance != null)
        {
            // Initialize ProjectileMove with the target position
            ProjectileMove movement = fireballInstance.AddComponent<ProjectileMove>();
            movement.speed = (int)ProjectileSpeed;
            movement.Initialize(target.transform.position);

            // Set up collision handling
            fireballInstance.AddComponent<FireballCollision>().Initialize(this, target);

            // Destroy the fireball after 5 seconds
            UnityEngine.Object.Destroy(fireballInstance, 5f);
        }
    }

    public void ApplyDOT(GameObject target)
    {
        AbilityManager.Instance.StartAbilityCoroutine(DOTCoroutine(target));
    }

    private IEnumerator DOTCoroutine(GameObject target)
    {
        float elapsed = 0f;
        while (elapsed < Duration)
        {
            if (target == null) yield break;

            // Apply DOT damage to the target
            target.GetComponent<EnemyHandle>().GetHealthHandle().TakeDamage(DamagePerTick, target);
            DamageTextManager.Instance.DisplayDamageNumber(DamagePerTick, target.transform.position);

            elapsed += TickInterval;
            yield return new WaitForSeconds(TickInterval);
        }
    }
}

public class FireballCollision : MonoBehaviour
{
    private Fireball fireballAbility;
    private GameObject target;

    public void Initialize(Fireball fireball, GameObject target)
    {
        fireballAbility = fireball;
        this.target = target;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Apply damage and DOT on collision
            collision.gameObject.GetComponent<EnemyHandle>().GetHealthHandle().TakeDamage((int)fireballAbility.EffectAmount, collision.gameObject);
            DamageTextManager.Instance.DisplayDamageNumber((int)fireballAbility.EffectAmount, collision.transform.position);

            fireballAbility.ApplyDOT(collision.gameObject);

            // Destroy the fireball upon impact
            Destroy(gameObject);
        }
    }
}
