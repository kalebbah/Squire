using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIImages : MonoBehaviour
{
    [Tooltip("Card slots for active abilities.")]
    public List<AbilityUIImage> activeAbilitySlots;

    [Tooltip("Card slots for passive abilities.")]
    public List<AbilityUIImage> passiveAbilitySlots;

    void Update()
    {
        UpdateAbilityUIs();
    }

    public void RegisterAbilityUI(BaseAbility ability, Sprite abilitySprite)
    {
        AbilityUIImage targetSlot = ability.IsActive ? GetAvailableSlot(activeAbilitySlots) : GetAvailableSlot(passiveAbilitySlots);
        
        if (targetSlot != null)
        {
            targetSlot.Initialize(ability, abilitySprite);
            Debug.Log("Ability initialized in slot");
        }
        else
        {
            Debug.LogWarning($"No available UI slots for {(ability.IsActive ? "active" : "passive")} abilities.");
        }
    }

    public void UpdateAbilityUIs()
    {
        foreach (var slot in activeAbilitySlots)
        {
            if (slot != null && slot.ability != null)  // Update only if ability is assigned
            {
                slot.UpdateCooldown();
            }
        }

        foreach (var slot in passiveAbilitySlots)
        {
            if (slot != null && slot.ability != null)  // Update only if ability is assigned
            {
                slot.UpdateCooldown();
            }
        }
    }

    private AbilityUIImage GetAvailableSlot(List<AbilityUIImage> slots)
    {
        return slots.Find(slot => slot.ability == null);
    }
}
