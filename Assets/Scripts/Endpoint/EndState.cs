using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndState : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI first;  // Text for primary details (e.g., level progress)
    public int progressFill;      // Simulated progress fill (can be replaced with a slider or progress bar)
    public TextMeshProUGUI second; // Text for secondary details (e.g., performance summary)
    public Button homeButton;     // Button to return to home state

    public TextMeshProUGUI coins;
    public TextMeshProUGUI gems;
    private int coinCount;
    private int gemCount;

    private GameManager gameManager; // Reference to GameManager or currency/reward tracking systems
    private EnemyManager enemyManager; // Reference to EnemyManager
    private LevelManager levelManager; // Reference to LevelManager

    void Start()
    {
        FindDependencies();
        SetupHomeButton();
    }
    void OnEnable() {
        SetInSlot();
    }
    /// <summary>
    /// Finds and initializes references to dependencies.
    /// </summary>
    public void FindDependencies()
    {
        // Get the GameManager instance
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found. Ensure it exists in the scene.");
        }

        // Get the EnemyManager instance
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager == null)
        {
            Debug.LogError("EnemyManager not found. Ensure it exists in the scene.");
        }

        // Get the LevelManager instance
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found. Ensure it exists in the scene.");
        }
    }

    /// <summary>
    /// Sets up the home button to transfer the game state to the home screen.
    /// </summary>
    private void SetupHomeButton()
    {
        homeButton.onClick.AddListener(() =>
        {
            // Transition to the home screen
            Debug.Log("Returning to Home...");
            EventManager<GameState>.TriggerEvent(EventKey.GAME_STATE_CHANGED, GameState.HOME);
        });
    }

    /// <summary>
    /// Populates the UI with end-of-level details.
    /// </summary>
    public void SetInSlot()
    {
        if (gameManager == null || enemyManager == null || levelManager == null)
        {
            Debug.LogError("Dependencies not found. Cannot populate end-of-level details.");
            return;
        }

        // Retrieve rewards and progress from the PlayerManager and EnemyManager
        coinCount = PlayerManager.Instance.User.GetCurrencyHandle().GetCoins();
        gemCount = PlayerManager.Instance.User.GetCurrencyHandle().GetGems();
        coins.text = coinCount.ToString();
        gems.text = gemCount.ToString();
        progressFill = (int)FindObjectOfType<DataPersistenceManager>().LevelProgression;
        //progressFill = Mathf.RoundToInt(enemyManager.progressMeter.fillAmount * 100); // Convert fillAmount (0-1) to percentage

        // Retrieve the current level
        int currentLevelIndex = FindObjectOfType<DataPersistenceManager>().Level; // Assuming EnemyManager has a method to get the current level index
        string currentLevelName = levelManager.levels[currentLevelIndex].levelName;

        // Update UI Elements
        first.text = $"Level {currentLevelName} Progress: {progressFill}%";
        second.text = "Rewards Collected";

        Debug.Log("EndState UI updated with level progress and rewards.");
    }
}
