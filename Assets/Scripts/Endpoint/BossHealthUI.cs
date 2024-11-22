using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : GenericSlider<HealthHandle>
{
    [Header("UI Elements")]
    public GameObject healthBarGameObject; // The container for the boss health bar
    public TextMeshProUGUI bossNameText; // Optional: Text for the boss name

    private void Start()
    {
        healthBarGameObject.SetActive(false); // Initially hide the boss health bar
    }

    protected override void RegisterEvents()
    {
        // Register to specific events using your EventManager
        EventManager<HealthHandle>.RegisterEvent(EventKey.BOSS_HEALTH_UPDATED, UpdateSlider);
        EventManager<string>.RegisterEvent(EventKey.BOSS_SPAWNED, ShowHealthBar); // Passes boss name as string
        EventManager<string>.RegisterEvent(EventKey.BOSS_DEFEATED, HideHealthBar); // Boss defeated event
    }

    protected override void UnregisterEvents()
    {
        // Unregister to prevent memory leaks
        EventManager<HealthHandle>.UnregisterEvent(EventKey.BOSS_HEALTH_UPDATED, UpdateSlider);
        EventManager<string>.UnregisterEvent(EventKey.BOSS_SPAWNED, ShowHealthBar);
        EventManager<string>.UnregisterEvent(EventKey.BOSS_DEFEATED, HideHealthBar);
    }

    protected override float CalculateFillAmount(HealthHandle healthData)
    {
        return (float)healthData.CurrentHealth / healthData.MaxHealth;
    }

    protected override void UpdateSlider(HealthHandle healthData)
    {
        if (sliderObject != null)
        {
            // Update the fill amount
            sliderObject.GetComponent<Image>().fillAmount = CalculateFillAmount(healthData);

            // Optionally change color based on health percentage
            Image healthImage = sliderObject.GetComponent<Image>();
            float fillValue = CalculateFillAmount(healthData);

            healthImage.color = fillValue switch
            {
                > 0.6f => Color.green,
                > 0.3f => Color.yellow,
                _ => Color.red
            };
        }
    }

    private void ShowHealthBar(string bossName)
    {
        healthBarGameObject.SetActive(true);

        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }
    }

    private void HideHealthBar(string bossName)
    {
        healthBarGameObject.SetActive(false);
    }
}
