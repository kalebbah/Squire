using System;
using System.Collections.Generic;

public static class EventManager<TEventArgs>
{
    private static Dictionary<string, Action<TEventArgs>> eventDictionary = new Dictionary<string, Action<TEventArgs>>();
    private static HashSet<string> activeEvents = new HashSet<string>(); // Track active events

    // Register in Start
    public static void RegisterEvent(string name, Action<TEventArgs> eventMethod)
    {
        if (!eventDictionary.ContainsKey(name))
            eventDictionary[name] = eventMethod;
        else
            eventDictionary[name] += eventMethod;
    }

    // Unregister onDestroy
    public static void UnregisterEvent(string name, Action<TEventArgs> eventMethod)
    {
        if (eventDictionary.ContainsKey(name))
            eventDictionary[name] -= eventMethod;
    }

    // Trigger Instantaneous Event
    public static void TriggerEvent(string name, TEventArgs eventArgs)
    {
        if (eventDictionary.ContainsKey(name))
        {
            eventDictionary[name]?.Invoke(eventArgs);
        }
    }

    // Start a Long-Running Event
    public static void StartEvent(string name, TEventArgs eventArgs)
    {
        if (eventDictionary.ContainsKey(name))
        {
            activeEvents.Add(name); // Mark the event as active
            eventDictionary[name]?.Invoke(eventArgs);
        }
    }

    // End a Long-Running Event
    public static void EndEvent(string name)
    {
        activeEvents.Remove(name); // Mark the event as complete
    }

    // Check if an event is active
    public static bool IsEventActive(string name)
    {
        return activeEvents.Contains(name);
    }
}


public static class EventKey {
    // Game Events
    public const string GAME_STATE_CHANGED = "GameStateChanged";
    public const string UPDATE_SLIDER_HEALTH_DISPLAY = "UpdateSliderHealthDisplay";
    public const string UPDATE_PLAYER_CURRENCY_DISPLAY = "UpdatePlayerCurrencyDisplay";
    public const string UPDATE_SLIDER_EXPERIENCE_DISPLAY = "UpdateSliderExperienceDisplay";
    public const string SAVE_DATA = "SaveData";
    public const string LOAD_DATA = "LoadData";
    public const string RESTART = "Restart";
    public const string PLAYER_INSTANTIATE = "PlayerInstantiate";

    // Player Specific Events
    public const string UPDATE_PLAYER_HEALTH = "UpdatePlayerHealth";    
    public const string UPDATE_PLAYER_EXPERIENCE = "UpdatePlayerExperience";
    public const string UPDATE_PLAYER_LEVEL = "UpdatePlayerLevel";
    public const string UPDATE_PLAYER_COINS = "UpdatePlayerCoins";
    public const string UPDATE_PLAYER_GEMS = "UpdatePlayerGems";

    // Boss Specific Events
    public const string BOSS_HEALTH_UPDATED = "BOSS_HEALTH_UPDATED";
    public const string BOSS_SPAWNED = "BOSS_SPAWNED";
    public const string BOSS_DEFEATED = "BOSS_DEFEATED";

    // UI & State Events
    public const string SHOW_GAME_UI = "ShowGameUI";
    public const string HIDE_GAME_UI = "HideGameUI";
    public const string SHOW_LEVELUP_UI = "ShowLevelUpUI";
    public const string HIDE_LEVELUP_UI = "HIDE_LEVELUP_UI";
    public const string SHOW_PAUSE_MENU = "ShowPauseMenu";    
    public const string HIDE_PAUSE_MENU = "HidePauseMenu";
    public const string SHOW_HOME_SCREEN = "ShowHomeScreen";
    public const string HIDE_HOME_SCREEN = "HideHomeScreen";
    public const string SHOW_END_SCREEN = "ShowEndScreen";
    public const string HIDE_END_SCREEN = "HideEndScreen";
    public const string SHOW_SETTINGS_MENU = "ShowSettingsMenu";
    public const string HIDE_SETTINGS_MENU = "HideSettingsMenu";

    // Ability Slot Events (for triggering ability by UI slot)
    public const string ABILITY_SLOT_0 = "AbilitySlot0";
    public const string ABILITY_SLOT_1 = "AbilitySlot1";
    public const string ABILITY_SLOT_2 = "AbilitySlot2";
    public const string ABILITY_SLOT_3 = "AbilitySlot3";

    // Boosters event update
    public const string DAMAGE_BOOST_LVLUP = "DamageBoostLevelUp";
    public const string HEALTH_BOOST_LVLUP = "HealthBoostLevelUp";
    public const string LUCK_BOOST_LVLUP = "LuckBoostLevelUp";

    //Passive modifiers update (For when script is out of of context for internal apply or StatManager call)
    public const string MOVEMENT_SPEED_LVLUP = "MovementSpeedLevelUp";

}
