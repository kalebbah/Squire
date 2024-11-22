using System;
using UnityEngine;

[Serializable]
public class CurrencyHandle
{
    [Header("Currency Values")]
    [SerializeField] private int coins = 0;
    [SerializeField] private int gems = 0;

    // Constructors
    public CurrencyHandle() { }

    public CurrencyHandle(int initialCoins, int initialGems)
    {
        coins = initialCoins;
        gems = initialGems;
    }

    // Properties to expose currency values safely
    public int Coins => coins;
    public int Gems => gems;

    // Methods to get values directly
    public int GetCoins() => coins;
    public int GetGems() => gems;

    // Methods to add currency
    public void AddCoins(int amount)
    {
        if (amount > 0)
        {
            coins += amount;
            //EventManager<int>.TriggerEvent(EventKey.UPDATE_PLAYER_COINS, coins);
            Debug.Log($"{amount} coins added. Total coins: {coins}");
        }
    }

    public void AddGems(int amount)
    {
        if (amount > 0)
        {
            gems += amount;
            //EventManager<int>.TriggerEvent(EventKey.UPDATE_PLAYER_GEMS, gems);
            Debug.Log($"{amount} gems added. Total gems: {gems}");
        }
    }
    // Methods to add currency
    public void SubCoins(int amount)
    {
        if (amount > 0)
        {
            coins -= amount;
            //EventManager<int>.TriggerEvent(EventKey.UPDATE_PLAYER_COINS, coins);
            Debug.Log($"{amount} coins subtracteed. Total coins: {coins}");
        }
    }

    public void SubGems(int amount)
    {
        if (amount > 0)
        {
            gems -= amount;
            //EventManager<int>.TriggerEvent(EventKey.UPDATE_PLAYER_GEMS, gems);
            Debug.Log($"{amount} gems subtracted. Total gems: {gems}");
        }
    }

    // Methods to set currency directly
    public void SetCoins(int amount)
    {
        coins = Mathf.Max(0, amount); // Ensure coins are non-negative
        EventManager<int>.TriggerEvent(EventKey.UPDATE_PLAYER_COINS, coins);
        Debug.Log($"Coins set to: {coins}");
    }

    public void SetGems(int amount)
    {
        gems = Mathf.Max(0, amount); // Ensure gems are non-negative
        EventManager<int>.TriggerEvent(EventKey.UPDATE_PLAYER_GEMS, gems);
        Debug.Log($"Gems set to: {gems}");
    }

    // Method to reset currency to zero
    public void ResetCurrency()
    {
        coins = 0;
        gems = 0;
        EventManager<int>.TriggerEvent(EventKey.UPDATE_PLAYER_COINS, coins);
        EventManager<int>.TriggerEvent(EventKey.UPDATE_PLAYER_GEMS, gems);
        Debug.Log("Currency reset to 0 for both coins and gems.");
    }
}
