using System;
using UnityEngine;

[Serializable]
public class ExperienceHandle
{
    [Header("Leveling System")]
    [SerializeField] private int level = 1;
    [SerializeField] private float threshold = 50;
    [SerializeField] private int collectedExperience = 0;
    [SerializeField] private int deathExperience = 10;

    // Constructors
    public ExperienceHandle()
    {
        level = 1;
        threshold = 50;
        deathExperience = 10;
    }

    public ExperienceHandle(int initialCollected, int initialDeath)
    {
        level = 1;
        threshold = 50;
        collectedExperience = initialCollected;
        deathExperience = initialDeath;
    }

    // Properties for encapsulated access
    public int Level => level;
    public float Threshold => threshold;
    public int CollectedExperience => collectedExperience;
    public int DeathExperience => deathExperience;

    // Getters for encapsulation
    public int GetCollectedExperience() => collectedExperience;
    public int GetDeathExperience() => deathExperience;
    public int GetLevel() => level;
    public float GetThreshold() => threshold;

    // Methods to modify experience
    public void AddCollectedExperience(int amount)
    {
        collectedExperience += amount;
        EventManager<ExperienceHandle>.TriggerEvent(EventKey.UPDATE_SLIDER_EXPERIENCE_DISPLAY, this);
    }

    public void LevelUp()
    {
        level++;
        if (Lottery.Instance != null)
        {
            Lottery.Instance.StartCoroutine(Lottery.Instance.ConductLottery());
        }
        else
        {
            Debug.LogError("Lottery instance is null! Ensure the Lottery singleton is properly initialized.");
        }

        Debug.Log($"Leveled up to Level {level} with new threshold: {threshold}");
    }



    // Method to increase death experience
    public void AddDeathExperience(int amount)
    {
        deathExperience += amount;
    }

    // Setters to directly set values (if needed for adjustments)
    public void SetThreshold(float newThreshold)
    {
        threshold = newThreshold;
    }

    public void SetLevel(int newLevel)
    {
        level = newLevel;
    }

    public void SetCollectedExperience(int newCollected)
    {
        collectedExperience = newCollected;
    }

    public void SetDeathExperience(int newDeathExperience)
    {
        deathExperience = newDeathExperience;
    }

    // Method to reset all values to default
    public void ResetExperienceHandle()
    {
        level = 1;
        threshold = 50;
        deathExperience = 10;
        collectedExperience = 0;
        Debug.Log("Experience Handle reset to defaults.");
    }
}
