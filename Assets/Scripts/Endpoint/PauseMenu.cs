using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Ability UI Elements")]
    public List<Image> abilityImages; // Images for the abilities (size 8)
    public List<TextMeshProUGUI> abilityDescriptions; // Text fields for descriptions (size 8)

    private AbilityHandle playerAbilityHandle; // Reference to the player's ability handle

    private void Start()
    {
        // Find and assign the player's ability handle
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerAbilityHandle = player.GetComponent<PlayerHandle>().GetAbilityHandle();
            if (playerAbilityHandle == null)
            {
                Debug.LogError("Player does not have an AbilityHandle component!");
            }
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
        EventManager<GameState>.RegisterEvent(EventKey.GAME_STATE_CHANGED, OnGameStateChange);
        // Initialize the UI
        UpdatePauseMenuUI();
    }

    public void UpdatePauseMenuUI()
    {
        if (playerAbilityHandle == null) return;

        List<BaseAbility> storedAbilities = playerAbilityHandle.GetStoredAbilities();

        for (int i = 0; i < abilityImages.Count; i++)
        {
            if (i < storedAbilities.Count)
            {
                BaseAbility ability = storedAbilities[i];

                // Set ability image
                Sprite abilitySprite = ability.LoadAbilitySprite();
                if (abilitySprite != null)
                {
                    abilityImages[i].sprite = abilitySprite;
                    abilityImages[i].gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"Sprite for ability '{ability.Name}' not found.");
                }

                // Set ability description and level
                abilityDescriptions[i].text = ability.Print();
                abilityDescriptions[i].gameObject.SetActive(true);
            }
            else
            {
                // Clear unused slots
                abilityImages[i].gameObject.SetActive(false);
                abilityDescriptions[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnGameStateChange(GameState state) {
        // Triggered when the pause menu is opened
        if (state == GameState.PAUSE)
            {
                UpdatePauseMenuUI();
            }
    }
}
