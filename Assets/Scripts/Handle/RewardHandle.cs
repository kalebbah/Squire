using UnityEngine;

[System.Serializable]
public class RewardHandle
{
    [Header("Coin Drop Settings")]
    [SerializeField] private int minCoins;
    [SerializeField] private int maxCoins;

    [Header("Gem Drop Settings")]
    [SerializeField] private float gemDropChance;
    [SerializeField] private int minGems;
    [SerializeField] private int maxGems;

    // Properties for internal access, if needed
    public int MinCoins => minCoins;
    public int MaxCoins => maxCoins;
    public float GemDropChance => gemDropChance;
    public int MinGems => minGems;
    public int MaxGems => maxGems;

    // Constructor to initialize the RewardHandle with specific values
    public RewardHandle(int minCoins, int maxCoins, float gemDropChance, int minGems, int maxGems)
    {
        this.minCoins = minCoins;
        this.maxCoins = maxCoins;
        this.gemDropChance = Mathf.Clamp01(gemDropChance); // Clamping to ensure the chance is between 0 and 1
        this.minGems = minGems;
        this.maxGems = maxGems;
    }

    // Method to call when the enemy is defeated
    public void OnDefeated()
    {
        DropLoot();
    }

    private void DropLoot()
    {
        int coinCount = GetRandomCoins();
        EventManager<int>.TriggerEvent(EventKey.UPDATE_PLAYER_COINS, coinCount);
        Debug.Log($"Dropped {coinCount} coins.");

        if (ShouldDropGems())
        {
            int gemCount = GetRandomGems();
            EventManager<int>.TriggerEvent(EventKey.UPDATE_PLAYER_GEMS, gemCount);
            Debug.Log($"Dropped {gemCount} gems.");
        }
        else
        {
            Debug.Log("No gems dropped.");
        }
        EventManager<PlayerHandle>.TriggerEvent(EventKey.UPDATE_PLAYER_CURRENCY_DISPLAY, PlayerManager.Instance.User);
    }

    // Helper methods
    private int GetRandomCoins()
    {
        return Random.Range(minCoins, maxCoins + 1);
    }

    private int GetRandomGems()
    {
        return Random.Range(minGems, maxGems + 1);
    }

    private bool ShouldDropGems()
    {
        return Random.value < gemDropChance;
    }

    // Methods to adjust drop settings
    public void SetCoinDropRange(int min, int max)
    {
        minCoins = min;
        maxCoins = max;
    }

    public void SetGemDropRange(int min, int max)
    {
        minGems = min;
        maxGems = max;
    }

    public void SetGemDropChance(float chance)
    {
        gemDropChance = Mathf.Clamp01(chance);
    }
}
