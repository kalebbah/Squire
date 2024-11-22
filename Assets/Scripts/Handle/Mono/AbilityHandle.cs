using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityHandle : MonoBehaviour
{
    public List<BaseAbility> storedAbilities = new List<BaseAbility>();
    private Dictionary<BaseAbility, Coroutine> cooldownCoroutines = new Dictionary<BaseAbility, Coroutine>();

    // Retrieve a count of active abilities
    public int GetActiveCount() => storedAbilities.FindAll(a => a.IsActive).Count;

    // Retrieve a count of passive abilities
    public int GetPassiveCount() => storedAbilities.FindAll(a => !a.IsActive).Count;

    // Retrieve all active abilities
    public List<BaseAbility> GetActiveAbilities()
    {
        return storedAbilities.Where(a => a.IsActive).ToList();
    }

    // Retrieve all passive abilities
    public List<BaseAbility> GetPassiveAbilities()
    {
        return storedAbilities.Where(a => !a.IsActive).ToList();
    }

    public List<BaseAbility> GetStoredAbilities() => storedAbilities;

    public void AddAbility(BaseAbility ability)
    {
        if (!storedAbilities.Contains(ability))
        {
            storedAbilities.Add(ability);
            Debug.Log($"Added ability: {ability.Name}");
        }
    }

    public void RemoveAbility(BaseAbility ability)
    {
        if (storedAbilities.Remove(ability))
        {
            Debug.Log($"Removed ability: {ability.Name}");
            StopCooldown(ability);
        }
    }

    public bool HasAbility(BaseAbility ability)
    {
        return storedAbilities.Any(a => a.Name == ability.Name); // Match by name or unique identifier
    }

    public void RegisterCooldown(BaseAbility ability, Coroutine coroutine)
    {
        if (cooldownCoroutines.ContainsKey(ability))
        {
            StopCoroutine(cooldownCoroutines[ability]);
        }
        cooldownCoroutines[ability] = coroutine;
    }

    public void StopCooldown(BaseAbility ability)
    {
        if (cooldownCoroutines.TryGetValue(ability, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            cooldownCoroutines.Remove(ability);
        }
    }

    public void ClearAbilities()
    {
        foreach (var ability in storedAbilities)
        {
            StopCooldown(ability);
        }
        storedAbilities.Clear();
    }
}
