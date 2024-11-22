using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoostUI : MonoBehaviour
{
    [Header("Boost UI Text")]
    public TextMeshProUGUI damageBoostText;   // UI Text for Damage Boost Level
    public TextMeshProUGUI healthBoostText;   // UI Text for Health Boost Level
    public TextMeshProUGUI luckBoostText;     // UI Text for Luck Boost Level

    [Header("Boost UI Button")]
    public Button damageBoostButton;   // Button for Damage Boost
    public Button healthBoostButton;   // Button for Health Boost
    public Button luckBoostButton;     // Button for Luck Boost

    [Header("Boost Costs")]
    public int initialDamageBoostCost = 100; // Initial cost for Damage Boost
    public int initialHealthBoostCost = 150; // Initial cost for Health Boost
    public int initialLuckBoostCost = 200;   // Initial cost for Luck Boost
    public float costMultiplier = 1.5f;      // Multiplier for cost increase

    private int currentDamageBoostCost;
    private int currentHealthBoostCost;
    private int currentLuckBoostCost;

    private void Start()
    {


        // Initialize costs
        currentDamageBoostCost = initialDamageBoostCost;
        currentHealthBoostCost = initialHealthBoostCost;
        currentLuckBoostCost = initialLuckBoostCost;

        // Assign button click events
        damageBoostButton.onClick.AddListener(() => TryBoost("Damage"));
        healthBoostButton.onClick.AddListener(() => TryBoost("Health"));
        luckBoostButton.onClick.AddListener(() => TryBoost("Luck"));

        UpdateUI();
    }

    private void UpdateUI()
    {
        // Update UI text with level and cost
        damageBoostText.text = $"Damage Boost: Level {StatManager.Instance.DamageBoostLevel} (Cost: {currentDamageBoostCost})";
        healthBoostText.text = $"Health Boost: Level {StatManager.Instance.HealthBoostLevel} (Cost: {currentHealthBoostCost})";
        luckBoostText.text = $"Luck Boost: Level {StatManager.Instance.LuckBoostLevel} (Cost: {currentLuckBoostCost})";

        // Update button interactivity based on player currency
        damageBoostButton.interactable = PlayerManager.Instance.User.GetCurrencyHandle().GetCoins() >= currentDamageBoostCost;
        healthBoostButton.interactable = PlayerManager.Instance.User.GetCurrencyHandle().GetCoins() >= currentHealthBoostCost;
        luckBoostButton.interactable = PlayerManager.Instance.User.GetCurrencyHandle().GetCoins() >= currentLuckBoostCost;
    }

    private void TryBoost(string boostType)
    {
        int cost = 0;

        switch (boostType)
        {
            case "Damage":
                cost = currentDamageBoostCost;
                if (PlayerManager.Instance.User.GetCurrencyHandle().GetCoins() >= cost)
                {
                    PlayerManager.Instance.User.GetCurrencyHandle().SubCoins(cost);
                    StatManager.Instance.AddDamageBoost();
                    currentDamageBoostCost = Mathf.RoundToInt(currentDamageBoostCost * costMultiplier);
                }
                else
                {
                    Debug.Log($"Not enough coins for Damage Boost! Cost: {cost}, Available: {PlayerManager.Instance.User.GetCurrencyHandle().GetCoins()}");
                    return;
                }
                break;

            case "Health":
                cost = currentHealthBoostCost;
                if (PlayerManager.Instance.User.GetCurrencyHandle().GetCoins() >= cost)
                {
                    PlayerManager.Instance.User.GetCurrencyHandle().SubCoins(cost);
                    StatManager.Instance.AddHealthBoost();
                    currentHealthBoostCost = Mathf.RoundToInt(currentHealthBoostCost * costMultiplier);
                }
                else
                {
                    Debug.Log($"Not enough coins for Health Boost! Cost: {cost}, Available: {PlayerManager.Instance.User.GetCurrencyHandle().GetCoins()}");
                    return;
                }
                break;

            case "Luck":
                cost = currentLuckBoostCost;
                if (PlayerManager.Instance.User.GetCurrencyHandle().GetCoins() >= cost)
                {
                    PlayerManager.Instance.User.GetCurrencyHandle().SubCoins(cost);
                    StatManager.Instance.AddLuckBoost();
                    currentLuckBoostCost = Mathf.RoundToInt(currentLuckBoostCost * costMultiplier);
                }
                else
                {
                    Debug.Log($"Not enough coins for Luck Boost! Cost: {cost}, Available: {PlayerManager.Instance.User.GetCurrencyHandle().GetCoins()}");
                    return;
                }
                break;

            default:
                Debug.LogError("Invalid boost type!");
                return;
        }

        Debug.Log($"{boostType} Boost applied! Remaining coins: {PlayerManager.Instance.User.GetCurrencyHandle().GetCoins()}");
        UpdateUI();
    }
}
