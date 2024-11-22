using TMPro;
using UnityEngine;

public class PlayerCounterText : MonoBehaviour {
    public static PlayerCounterText Instance { get; private set; }

    public GameObject[] coinTexts;        // Array to hold multiple coin text references
    public GameObject[] gemTexts;         // Array for multiple gem text references
    public GameObject[] experienceTexts;  // Array for multiple experience text references

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: if you want to persist across scenes
        } else {
            Destroy(gameObject);
            return;
        }

        EventManager<PlayerHandle>.RegisterEvent(EventKey.UPDATE_PLAYER_CURRENCY_DISPLAY, UpdateCurrencyText);
    }
    
    public void UpdateCurrencyText(PlayerHandle toUpdate) {
        if (toUpdate != null) {
            // Update all coin texts
            foreach (var coinText in coinTexts) {
                coinText.GetComponent<TextMeshProUGUI>().text = toUpdate.GetCurrencyHandle().GetCoins().ToString();
            }

            // Update all gem texts
            foreach (var gemText in gemTexts) {
                gemText.GetComponent<TextMeshProUGUI>().text = toUpdate.GetCurrencyHandle().GetGems().ToString();
            }

            float collectedExperience = toUpdate.GetExperienceHandle().GetCollectedExperience();
            float threshold = toUpdate.GetExperienceHandle().GetThreshold();
            float experiencePercentage = Mathf.Clamp((collectedExperience / threshold) * 100, 0, 100);

            // Update all experience texts
            foreach (var experienceText in experienceTexts) {
                experienceText.GetComponent<TextMeshProUGUI>().text = 
                    $"{toUpdate.GetExperienceHandle().GetLevel()}  {Mathf.FloorToInt(experiencePercentage)} %";
            }
        }
    }

    void OnDestroy() {
        if (Instance == this) {
            EventManager<PlayerHandle>.UnregisterEvent(EventKey.UPDATE_PLAYER_CURRENCY_DISPLAY, UpdateCurrencyText);
            Instance = null;
        }
    }
}
