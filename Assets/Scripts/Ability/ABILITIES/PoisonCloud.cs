using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCloud : BaseAbility, IAOE, IDOT
{
    public int DamagePerTick { get; set; }
    public float TickInterval { get; set; }
    public float Duration { get; set; }
    public float AreaRadius { get; set; }
    public float SpawnRadius { get; set; }

    public PoisonCloud(string name, string description, float cooldown, bool isActive)
        : base(name, description, cooldown, isActive)
    {
        SpawnRadius = 40f; // Define how far the poison cloud can spawn from the caster
    }

    public override void Activate(GameObject caster)
    {
        ApplyAOE(caster); // Fulfill the contract by calling ApplyAOE
        Debug.Log("Activated: PoisonCloud");
    }
    public override void LevelUp()
    {
        base.LevelUp();
        DamagePerTick += 5; // Increase damage per level
        AreaRadius += 2f; // Increase speed per level
    }
    public override string Print()
    {
        // Generate a string with the current stats and next level bonuses
        string abilityStats = $"{Name} (Level {Level})\n" +
                              $"{Description}\n" +
                              $"Damage Per Tick: {DamagePerTick} (+5)\n" +
                              $"Area Radius: {AreaRadius} (+2)\n" +
                              $"Duration: {Duration}s\n" +
                              $"Cooldown: {Cooldown}s\n";

        // Print the stats to the console
        Debug.Log(abilityStats);

        return abilityStats;
    }


    public void ApplyDOT(GameObject caster)
    {
        ApplyAOE(caster); // DOT effects are tied to AOE in this case
    }

    public void ApplyAOE(GameObject caster)
    {
        Vector3 randomPosition = GetRandomPosition(caster.transform.position, SpawnRadius);
        LaunchPoisonCloud(randomPosition);
    }

    private void LaunchPoisonCloud(Vector3 position)
    {
        GameObject poisonCloudInstance = AbilityManager.Instance.InstantiateAbilityObject("PoisonCloudPrefab", position, Quaternion.identity);
        if (poisonCloudInstance != null)
        {
            // Scale the cloud to match the area radius
            float diameter = AreaRadius * 2;
            poisonCloudInstance.transform.localScale = new Vector3(diameter, diameter, diameter);

            // Attach and initialize the PoisonCloudTrigger component
            PoisonCloudTrigger trigger = poisonCloudInstance.AddComponent<PoisonCloudTrigger>();
            trigger.Initialize(this);

            // Destroy the cloud after its duration
            UnityEngine.Object.Destroy(poisonCloudInstance, Duration);
        }
    }

    private Vector3 GetRandomPosition(Vector3 center, float radius)
    {
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        return new Vector3(center.x + randomPoint.x, center.y, center.z + randomPoint.y);
    }
}

public class PoisonCloudTrigger : MonoBehaviour
{
    private PoisonCloud ability;
    private Dictionary<GameObject, Coroutine> activeDOTs = new Dictionary<GameObject, Coroutine>();

    public void Initialize(PoisonCloud ability)
    {
        this.ability = ability;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !activeDOTs.ContainsKey(other.gameObject))
        {
            Coroutine dotCoroutine = StartCoroutine(ApplyDOT(other.gameObject));
            activeDOTs.Add(other.gameObject, dotCoroutine);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && activeDOTs.ContainsKey(other.gameObject))
        {
            StopCoroutine(activeDOTs[other.gameObject]);
            activeDOTs.Remove(other.gameObject);
        }
    }

    private IEnumerator ApplyDOT(GameObject enemy)
    {
        float dotDuration = ability.Duration; // Initial DOT duration
        float tickTimer = 0f;

        while (dotDuration > 0)
        {
            if (enemy == null)
            {
                activeDOTs.Remove(enemy);
                yield break;
            }

            // Apply damage at each tick interval
            if (tickTimer <= 0f)
            {
                EnemyHandle enemyHandle = enemy.GetComponent<EnemyHandle>();
                if (enemyHandle != null)
                {
                    enemyHandle.GetHealthHandle().TakeDamage(ability.DamagePerTick, enemy);
                    DamageTextManager.Instance.DisplayDamageNumber(ability.DamagePerTick, enemy.transform.position);
                }
                tickTimer = ability.TickInterval;
            }

            tickTimer -= Time.deltaTime;
            dotDuration -= Time.deltaTime;

            yield return null;
        }

        // Remove the DOT when the duration ends
        if (activeDOTs.ContainsKey(enemy))
        {
            activeDOTs.Remove(enemy);
        }
    }
}
