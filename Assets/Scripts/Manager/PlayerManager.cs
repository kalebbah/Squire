using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Player Handle")]
    [Tooltip("Reference to the PlayerHandle component controlling player stats.")]
    public PlayerHandle User;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Find and assign the PlayerHandle
        User = FindObjectOfType<PlayerHandle>();
        if (User == null)
        {
            Debug.LogError("PlayerHandle not found in the scene!");
            return;
        }

        RegisterEvents();
    }

    private void Start()
    {
        Display();
    }

    private void Update()
    {
        if (User.GetHealthHandle().CurrentHealth > 0)
        {
            User.GetComponent<WASDMove>().enabled = GameManager.Instance.currentState == GameState.GAME;
        }
    }

    // Registering all required events
    private void RegisterEvents()
    {
        EventManager<int>.RegisterEvent(EventKey.UPDATE_PLAYER_COINS, AddUserCoinCounter);
        EventManager<int>.RegisterEvent(EventKey.UPDATE_PLAYER_GEMS, AddUserGemCounter);
        EventManager<int>.RegisterEvent(EventKey.UPDATE_PLAYER_EXPERIENCE, AddUserExperience);
        EventManager<int>.RegisterEvent(EventKey.UPDATE_PLAYER_HEALTH, AddUserHealth);
        EventManager<int>.RegisterEvent(EventKey.UPDATE_PLAYER_LEVEL,AddUserLevel);
    }

    // Display UI for player stats
    private void Display()
    {
        EventManager<PlayerHandle>.TriggerEvent(EventKey.UPDATE_SLIDER_EXPERIENCE_DISPLAY, User);
        EventManager<HealthHandle>.TriggerEvent(EventKey.UPDATE_SLIDER_HEALTH_DISPLAY, User.GetHealthHandle());
        EventManager<PlayerHandle>.TriggerEvent(EventKey.UPDATE_PLAYER_CURRENCY_DISPLAY, User);
    }

    // Event-based handlers
    private void AddUserCoinCounter(int toCoin)
    {
        User.GetCurrencyHandle().AddCoins(toCoin);
        EventManager<PlayerHandle>.TriggerEvent(EventKey.UPDATE_PLAYER_CURRENCY_DISPLAY, User);
        EventManager<EventArgs>.TriggerEvent(EventKey.SAVE_DATA, EventArgs.Empty);
    }

    private void AddUserGemCounter(int toGem)
    {
        User.GetCurrencyHandle().AddGems(toGem);
        //MusicPlayerController.Instance.PlayCollectItemSound();
        EventManager<PlayerHandle>.TriggerEvent(EventKey.UPDATE_PLAYER_CURRENCY_DISPLAY, User);
        EventManager<EventArgs>.TriggerEvent(EventKey.SAVE_DATA, EventArgs.Empty);
    }
    private void SubUserCoinCounter(int toCoin)
    {
        User.GetCurrencyHandle().SubCoins(toCoin);
        EventManager<PlayerHandle>.TriggerEvent(EventKey.UPDATE_PLAYER_CURRENCY_DISPLAY, User);
        EventManager<EventArgs>.TriggerEvent(EventKey.SAVE_DATA, EventArgs.Empty);
    }

    private void SubUserGemCounter(int toGem)
    {
        User.GetCurrencyHandle().SubGems(toGem);
        //MusicPlayerController.Instance.PlayCollectItemSound();
        EventManager<PlayerHandle>.TriggerEvent(EventKey.UPDATE_PLAYER_CURRENCY_DISPLAY, User);
        EventManager<EventArgs>.TriggerEvent(EventKey.SAVE_DATA, EventArgs.Empty);
    }

    // Basically called by individual enemies death
    public void AddUserExperience(int toExperience)
    {
        User.GetExperienceHandle().AddCollectedExperience(toExperience);
        
        if (User.GetExperienceHandle().GetCollectedExperience() >= User.GetExperienceHandle().GetThreshold())
        {
            EventManager<int>.StartEvent(EventKey.UPDATE_PLAYER_LEVEL, 1);
        }
        EventManager<PlayerHandle>.TriggerEvent(EventKey.UPDATE_PLAYER_CURRENCY_DISPLAY, User);
        EventManager<ExperienceHandle>.TriggerEvent(EventKey.UPDATE_SLIDER_EXPERIENCE_DISPLAY, User.GetExperienceHandle());
    }
    // Called by AddUserExperience method above when levelup occurs
    public void AddUserLevel(int toLevel) {
        User.GetExperienceHandle().LevelUp();
        User.GetHealthHandle().ResetHealth();
        User.GetExperienceHandle().SetThreshold(Mathf.RoundToInt(User.GetExperienceHandle().GetThreshold() * 1.5f));
        User.GetExperienceHandle().SetCollectedExperience(0);
        //MusicPlayerController.Instance.PlayLevelUpSound();
        EventManager<HealthHandle>.TriggerEvent(EventKey.UPDATE_SLIDER_HEALTH_DISPLAY, User.GetHealthHandle());
        EventManager<PlayerHandle>.TriggerEvent(EventKey.UPDATE_PLAYER_CURRENCY_DISPLAY, User);
        EventManager<ExperienceHandle>.TriggerEvent(EventKey.UPDATE_SLIDER_EXPERIENCE_DISPLAY, User.GetExperienceHandle());
    }

    public void AddUserHealth(int toHealth)
    {
        User.GetHealthHandle().Heal(toHealth);
        EventManager<HealthHandle>.TriggerEvent(EventKey.UPDATE_SLIDER_HEALTH_DISPLAY, User.GetHealthHandle());
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    private void UnregisterEvents()
    {
        EventManager<int>.UnregisterEvent(EventKey.UPDATE_PLAYER_EXPERIENCE, AddUserExperience);
        EventManager<int>.UnregisterEvent(EventKey.UPDATE_PLAYER_HEALTH, AddUserHealth);
        EventManager<int>.UnregisterEvent(EventKey.UPDATE_PLAYER_COINS, AddUserCoinCounter);
        EventManager<int>.UnregisterEvent(EventKey.UPDATE_PLAYER_GEMS, AddUserGemCounter);
        EventManager<int>.UnregisterEvent(EventKey.UPDATE_PLAYER_LEVEL,AddUserLevel);
    }
}
