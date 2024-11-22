using System;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    private const string CoinCountKey = "CoinCount";
    private const string GemCountKey = "GemCount";
    private const string LevelProgressionKey = "LevelProgression";
    private const string LevelKey = "Level";

    private const string DamageLevelKey = "DamageLevel";
    private const string HealthLevelKey = "HealthLevel";
    private const string LuckLevelKey = "LuckLevel";

    public int CoinCount { get; private set; }
    public int GemCount { get; private set; }
    public float LevelProgression { get; set; }
    public int Level { get; private set; } // Added Level field

    private void OnEnable()
    {
        EventManager<EventArgs>.RegisterEvent(EventKey.SAVE_DATA, OnSaveData);
        EventManager<EventArgs>.RegisterEvent(EventKey.LOAD_DATA, OnLoadData);
    }

    private void OnDisable()
    {
        EventManager<EventArgs>.UnregisterEvent(EventKey.SAVE_DATA, OnSaveData);
        EventManager<EventArgs>.UnregisterEvent(EventKey.LOAD_DATA, OnLoadData);
    }

    private void Start()
    {
        // Automatically load data when the game starts
        EventManager<EventArgs>.TriggerEvent(EventKey.LOAD_DATA, EventArgs.Empty);
    }

    public void SetLevelProgression(float levelProgression)
    {
        LevelProgression = levelProgression;
        EventManager<EventArgs>.TriggerEvent(EventKey.SAVE_DATA, EventArgs.Empty);
    }

    public void SetLevel(int newLevel)
    {
        Level = newLevel;
        EventManager<EventArgs>.TriggerEvent(EventKey.SAVE_DATA, EventArgs.Empty);
    }

    private void OnSaveData(EventArgs args)
    {
        PlayerPrefs.SetInt(CoinCountKey, CoinCount);
        PlayerPrefs.SetInt(GemCountKey, GemCount);
        if(GameManager.Instance.currentState == GameState.GAME) {
            PlayerPrefs.SetFloat(LevelProgressionKey, LevelProgression);
            PlayerPrefs.SetInt(LevelKey, Level); // Save Level
        }
            

        // Save boost levels
        PlayerPrefs.SetInt(DamageLevelKey, (int)StatManager.Instance.DamageBonus);
        PlayerPrefs.SetInt(HealthLevelKey, (int)StatManager.Instance.HealthBoost);
        PlayerPrefs.SetInt(LuckLevelKey, (int)StatManager.Instance.LuckBoost);

        PlayerPrefs.Save();
        Debug.Log("Game data saved.");
    }

    private void OnLoadData(EventArgs args)
    {
        CoinCount = PlayerPrefs.GetInt(CoinCountKey, 0); // Default to 0 if no data exists
        GemCount = PlayerPrefs.GetInt(GemCountKey, 0);
        LevelProgression = PlayerPrefs.GetFloat(LevelProgressionKey, 0);
        Level = PlayerPrefs.GetInt(LevelKey, 0); // Default Level to 1 if no data exists

        // Load boost levels
        StatManager.Instance.DamageBoost = PlayerPrefs.GetInt(DamageLevelKey, 0);
        StatManager.Instance.HealthBoost = PlayerPrefs.GetInt(HealthLevelKey, 0);
        StatManager.Instance.LuckBoost = PlayerPrefs.GetInt(LuckLevelKey, 0);

        // Recalculate stat bonuses based on loaded levels
        StatManager.Instance.DamageBonus = StatManager.Instance.DamageBoost + (int)StatManager.Instance.DamageBonus;

        Debug.Log("Game data loaded.");
    }
}
