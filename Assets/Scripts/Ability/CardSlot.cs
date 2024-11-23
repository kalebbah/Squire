using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSlot : MonoBehaviour
{
    public BaseAbility ability; // Reference to the ability associated with this slot
    public TextMeshProUGUI abilityDescriptionText; // UI element to display the ability description
    public Image abilityImage; // UI element to display the ability's image
    public bool isSelected = false; // Track if this card is selected
    public Button button; // Reference to the button component

    private void Start()
    {
        // Set up the ability description and image if the ability is assigned
        if (ability != null)
        {
            UpdateAbilityUI();
        }

        // Set up button click listener
        if (button != null)
        {
            button.onClick.AddListener(OnCardSelected);
        }
        else
        {
            Debug.LogWarning("Button is not assigned in CardSlot.");
        }
    }

    public void SetAbility(BaseAbility newAbility)
    {
        Debug.Log($"Setting ability: {(newAbility != null ? newAbility.Name : "null")}");
        ability = newAbility;
        UpdateAbilityUI();
    }

    private void UpdateAbilityUI()
    {
        if (ability != null)
        {
            // Update description text
            if (abilityDescriptionText != null)
            {
                abilityDescriptionText.text = ability.Description;
            }
            
            // Load and set ability image
            if (abilityImage != null)
            {
                // Assuming you have a Resources folder with images named after each ability
                Sprite abilitySprite = Resources.Load<Sprite>($"AbilityUI/{ability.Name}");
                if (abilitySprite != null)
                {
                    abilityImage.sprite = abilitySprite;
                }
                else
                {
                    Debug.LogWarning($"Image not found for ability: {ability.Name}");
                }
            }
        }
        else
        {
            Debug.LogWarning("Ability or UI elements are null. Cannot update ability UI.");
        }
    }

    // When the player clicks on this card slot, mark it as selected
    public void OnCardSelected()
    {
        isSelected = true;
        Debug.Log($"Card selected: {(ability != null ? ability.Name : "null")}");
        HighlightCard();
        DeselectOtherCards();
    }

    // Visual feedback for card selection
    private void HighlightCard()
    {
        // Example: Set the color of the card to indicate selection
        GetComponent<Image>().color = Color.yellow; // Assuming the card has an Image component
    }

    // Deselect other cards in the UI
    private void DeselectOtherCards()
    {
        CardSlot[] allCards = FindObjectsOfType<CardSlot>();
        foreach (CardSlot card in allCards)
        {
            if (card != this)
            {
                card.Deselect();
            }
        }
    }

    // Method to deselect this card and reset its appearance
    public void Deselect()
    {
        isSelected = false;
        GetComponent<Image>().color = Color.white; // Reset color to default
    }

    // Check if this card is currently selected
    public bool IsCardSelected()
    {
        return isSelected;
    }
}