using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

public class AbilityRepository : MonoBehaviour
{
    private List<BaseAbility> abilities = new List<BaseAbility>();

    void Start()
    {
        LoadAbilities("Assets/Data/abilities.json");
    }

    public void LoadAbilities(string filePath)
    {
        string json = File.ReadAllText(filePath);
        List<AbilityData> data = JsonConvert.DeserializeObject<List<AbilityData>>(json);

        foreach (var abilityData in data)
        {
            BaseAbility ability = CreateAbilityFromData(abilityData);
            if (ability != null) abilities.Add(ability);
        }
    }

    // Method for creating abilities based on the ability data
    private BaseAbility CreateAbilityFromData(AbilityData data)
    {
        BaseAbility ability = null;

        // Use switch or conditionals to determine the correct derived class
        switch (data.name)
        {
            case "FrostAura":
                ability = new FrostAura(data.name, data.description, data.cooldown, data.isActive);
                break;
            case "Fireball":
                ability = new Fireball(data.name, data.description, data.cooldown, data.isActive);
                break;
            case "LightningBolt":
                ability = new LightningBolt(data.name, data.description, data.cooldown, data.isActive);
                break;
            case "LightningStrike":
                ability = new LightningStrike(data.name, data.description, data.cooldown, data.isActive);
                break;
            case "PoisonCloud":
                ability = new PoisonCloud(data.name, data.description, data.cooldown, data.isActive);
                break;
            case "ProjectileCount":
                ability = new ProjectileCount(data.name, data.description);
                break;
            case "CooldownReduction":
                ability = new CooldownReduction(data.name, data.description);
                break;
            case "HealthRegen":
                ability = new HealthRegen(data.name, data.description);
                break;
            case "DamageBonus":
                ability = new DamageBonus(data.name, data.description);
                break;
            case "MovementSpeed":
                ability = new MovementSpeed(data.name, data.description);
                break;
            default:
                Debug.LogWarning($"Ability '{data.name}' not recognized.");
                break;
        }

        if (ability != null)
        {
            ApplyAttributes(ability, data.attributes);
            Debug.Log($"Created ability: {ability.Name} ({(data.isActive ? "Active" : "Passive")})");
        }

        return ability;
    }


    // Apply attributes to abilities based on the data dictionary
    private void ApplyAttributes(BaseAbility ability, Dictionary<string, float> attributes)
    {
        foreach (var attribute in attributes)
        {
            var property = ability.GetType().GetProperty(attribute.Key, BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.CanWrite)
            {
                try
                {
                    property.SetValue(ability, Convert.ChangeType(attribute.Value, property.PropertyType));
                    Debug.Log($"Set {ability.Name}.{attribute.Key} to {attribute.Value}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to set {attribute.Key} on {ability.Name}: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Property '{attribute.Key}' not found or is read-only on {ability.Name}");
            }
        }
    }

    public List<BaseAbility> GetAbilities()
    {
        return abilities;
    }
}
