using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }

    public int maxActiveAbilities = 4;
    public int maxPassiveAbilities = 4;

    private AbilityHandle playerAbilityHandle;
    private AbilityUIImages abilityUIImages;
    private bool isPaused = false;

    private Dictionary<BaseAbility, Coroutine> activeCoroutines = new Dictionary<BaseAbility, Coroutine>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        EventManager<GameObject>.RegisterEvent(EventKey.PLAYER_INSTANTIATE, Initialize);
        EventManager<object>.RegisterEvent(EventKey.RESTART, _ => ResetManager()); // Listen for the RESTART event
    }

    private void Start()
    {
        abilityUIImages = FindObjectOfType<AbilityUIImages>();
        if (abilityUIImages == null)
        {
            Debug.LogError("No AbilityUIImages component found in the scene.");
        }

        EventManager<GameState>.RegisterEvent(EventKey.GAME_STATE_CHANGED, OnGameStateChanged);

        if (GameManager.Instance.currentState == GameState.GAME)
        {
            ResumeActiveAbilities();
        }
    }

    private void Initialize(GameObject player)
    {
        playerAbilityHandle = player.GetComponent<PlayerHandle>().GetAbilityHandle();
        if (playerAbilityHandle == null)
        {
            Debug.LogError("No AbilityHandle found on player.");
        }
    }

    private void OnDestroy()
    {
        EventManager<GameState>.UnregisterEvent(EventKey.GAME_STATE_CHANGED, OnGameStateChanged);
        EventManager<object>.UnregisterEvent(EventKey.RESTART, _ => ResetManager()); // Unregister RESTART event
    }

    private void OnApplicationQuit()
    {
        EventManager<GameObject>.UnregisterEvent(EventKey.PLAYER_INSTANTIATE, Initialize);
    }

    private void OnGameStateChanged(GameState newState)
    {
        isPaused = newState != GameState.GAME;

        if (isPaused)
        {
            foreach (var ability in activeCoroutines.Keys)
            {
                StopAbilityCoroutine(ability);
            }
        }
        else
        {
            ResumeActiveAbilities();
        }
    }

    public bool AddAbility(BaseAbility ability)
    {
        if (ability.IsActive && playerAbilityHandle.GetActiveCount() >= maxActiveAbilities)
        {
            Debug.LogWarning("Cannot add more active abilities.");
            return false;
        }
        else if (!ability.IsActive && playerAbilityHandle.GetPassiveCount() >= maxPassiveAbilities)
        {
            Debug.LogWarning("Cannot add more passive abilities.");
            return false;
        }

        playerAbilityHandle.AddAbility(ability);
        Debug.Log(ability + " added");
        if (!ability.IsActive)
        {
            ApplyPassiveEffect(ability);
        }
        else
        {
            StartAbilityCoroutine(AbilityCooldownCoroutine(ability));
        }

        return true;
    }

    public void RemoveAbility(BaseAbility ability)
    {
        playerAbilityHandle.RemoveAbility(ability);
        if (!ability.IsActive)
        {
            RemovePassiveEffect(ability);
        }
        else
        {
            StopAbilityCoroutine(ability);
        }
    }

    private void ApplyPassiveEffect(BaseAbility ability)
    {
        switch (ability)
        {
            case ProjectileCount pc:
                StatManager.Instance.AddProjectileCount(pc.Count);
                break;
            case CooldownReduction cr:
                StatManager.Instance.AddCooldownReduction(cr.ReductionMultiplier);
                break;
            case DamageBonus db:
                StatManager.Instance.AddDamageBonus(db.DamageMultiplier);
                break;
            case MovementSpeed ms:
                StatManager.Instance.AddMovementSpeed((int)ms.SpeedMultiplier);
                break;
            case HealthRegen hr:
                StatManager.Instance.AddHealthRegen(hr.HealPerTick);
                break;
            default:
                Debug.LogWarning($"Unhandled passive ability effect: {ability.Name}");
                break;
        }
    }

    private void RemovePassiveEffect(BaseAbility ability)
    {
        switch (ability)
        {
            case ProjectileCount pc:
                StatManager.Instance.RemoveProjectileCount(pc.Count);
                break;
            case CooldownReduction cr:
                StatManager.Instance.RemoveCooldownReduction(cr.ReductionMultiplier);
                break;
            case DamageBonus db:
                StatManager.Instance.RemoveDamageBonus(db.DamageMultiplier);
                break;
            case MovementSpeed ms:
                StatManager.Instance.RemoveMovementSpeed((int)ms.SpeedMultiplier);
                break;
            case HealthRegen hr:
                StatManager.Instance.RemoveHealthRegen(hr.HealPerTick);
                break;
            default:
                Debug.LogWarning($"Unhandled passive ability effect removal: {ability.Name}");
                break;
        }
    }

    private IEnumerator AbilityCooldownCoroutine(BaseAbility ability)
    {
        if (ability.IsCoroutineActive) yield break;

        ability.StartCooldown();
        Debug.Log($"Cooldown coroutine started for {ability.Name}");

        while (true)
        {
            if (isPaused)
            {
                yield return null; // Wait until unpaused
                continue;
            }

            if (ability.IsReady())
            {
                Debug.Log($"Activating {ability.Name} at {Time.time}");
                ability.Activate(playerAbilityHandle.gameObject);
                abilityUIImages.UpdateAbilityUIs();

                // Start cooldown after activation
                ability.StartCooldown();
            }

            // Wait a frame before checking again
            yield return null;
        }
    }

    private void ResumeActiveAbilities()
    {
        foreach (var ability in playerAbilityHandle.GetActiveAbilities())
        {
            if (!ability.IsCoroutineActive)
            {
                Debug.Log($"Resuming ability: {ability.Name}");
                StartAbilityCoroutine(AbilityCooldownCoroutine(ability));
            }
        }
    }

    public void StartAbilityCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void StopAbilityCoroutine(BaseAbility ability)
    {
        if (activeCoroutines.TryGetValue(ability, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            activeCoroutines.Remove(ability);
            ability.StopCooldown(); // Reset cooldown state
            Debug.Log($"Stopped coroutine for {ability.Name}");
        }
    }

    private void ResetManager()
    {
        Debug.Log("Resetting AbilityManager...");

        // Stop all active coroutines
        foreach (var coroutine in activeCoroutines.Values)
        {
            StopCoroutine(coroutine);
        }
        activeCoroutines.Clear();

        // Remove all abilities
        if (playerAbilityHandle != null)
        {
            foreach (var ability in playerAbilityHandle.GetStoredAbilities())
            {
                RemoveAbility(ability);
            }
        }

        Debug.Log("AbilityManager reset complete.");
    }

    public GameObject InstantiateAbilityObject(string prefabName, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = Resources.Load<GameObject>($"VFX/{prefabName}");

        if (prefab == null)
        {
            Debug.LogError($"Prefab '{prefabName}' not found in Resources/VFX!");
            return null;
        }

        return Instantiate(prefab, position, rotation);
    }
}
