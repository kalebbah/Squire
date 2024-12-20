using TMPro;
using UnityEngine;

public enum GameState
{
    HOME,
    GAME,
    PAUSE,
    LEVELUP,
    SETTINGS,
    END
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState currentState;

    [Header("Canvases")]
    public Canvas homeCanvas;
    public Canvas gameCanvas;
    public Canvas settingsCanvas;
    public Canvas pauseCanvas;
    public Canvas levelUpCanvas;
    public Canvas endCanvas;

    [Header("Player Settings")]
    public GameObject playerPrefab;  // Assign the player prefab in the Inspector
    private GameObject playerInstance; // Reference to the instantiated player
    public Vector3 initialPlayerPosition = Vector3.zero; // Default spawn position

    [Header("Pause Menu Settings")]
    public TextMeshProUGUI pauseAbilityText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager instance created.");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Duplicate GameManager destroyed.");
        }
    }

    private void Start()
    {
        ChangeState(GameState.HOME); // Start in the Home state
        RegisterEvents();
         // Instantiate the player at the start
        InstantiatePlayer();
    }

    private void Update()
    {
        HandleInput();
    }
    public GameObject GetPlayerInstance()
{
    return playerInstance;
}


    private void InstantiatePlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is not assigned in GameManager!");
            return;
        }

        // Destroy the previous player instance if it exists
        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }

        // Instantiate the player at the initial position
        playerInstance = Instantiate(playerPrefab, initialPlayerPosition, Quaternion.identity);
        Debug.Log("Player instantiated at position: " + initialPlayerPosition);
        EventManager<GameObject>.TriggerEvent(EventKey.PLAYER_INSTANTIATE, playerInstance);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentState == GameState.HOME)
            {
                ChangeState(GameState.GAME);
                EventManager<object>.TriggerEvent(EventKey.RESTART, null);
            }
            else if (currentState == GameState.GAME)
                ChangeState(GameState.PAUSE);
            else if (currentState == GameState.PAUSE)
                ChangeState(GameState.GAME);
        } else if (Input.GetKeyDown(KeyCode.K)) {
            PlayerManager.Instance.Kill();
        }
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        EventManager<object>.RegisterEvent(EventKey.PLAYER_INSTANTIATE, null);
        EventManager<object>.RegisterEvent(EventKey.SHOW_GAME_UI, ShowGameUI);
        EventManager<object>.RegisterEvent(EventKey.HIDE_GAME_UI, HideGameUI);
        EventManager<object>.RegisterEvent(EventKey.SHOW_LEVELUP_UI, ShowLevelUpUI);
        EventManager<object>.RegisterEvent(EventKey.HIDE_LEVELUP_UI, HideLevelUpUI);
        EventManager<object>.RegisterEvent(EventKey.SHOW_HOME_SCREEN, ShowHomeScreen);
        EventManager<object>.RegisterEvent(EventKey.HIDE_HOME_SCREEN, HideHomeScreen);
        EventManager<object>.RegisterEvent(EventKey.SHOW_PAUSE_MENU, ShowPauseMenu);
        EventManager<object>.RegisterEvent(EventKey.HIDE_PAUSE_MENU, HidePauseMenu);
        EventManager<object>.RegisterEvent(EventKey.SHOW_END_SCREEN, ShowEndGameScreen);
        EventManager<object>.RegisterEvent(EventKey.HIDE_END_SCREEN, HideEndGameScreen);
        EventManager<object>.RegisterEvent(EventKey.SHOW_SETTINGS_MENU, ShowSettingsMenu);
        EventManager<object>.RegisterEvent(EventKey.HIDE_SETTINGS_MENU, HideSettingsMenu);

        Debug.Log("UI events registered.");
    }

    private void UnregisterEvents()
    {
        EventManager<object>.UnregisterEvent(EventKey.PLAYER_INSTANTIATE, null);
        EventManager<object>.UnregisterEvent(EventKey.SHOW_GAME_UI, ShowGameUI);
        EventManager<object>.UnregisterEvent(EventKey.HIDE_GAME_UI, HideGameUI);
        EventManager<object>.UnregisterEvent(EventKey.SHOW_LEVELUP_UI, ShowLevelUpUI);
        EventManager<object>.UnregisterEvent(EventKey.HIDE_LEVELUP_UI, HideLevelUpUI);
        EventManager<object>.UnregisterEvent(EventKey.SHOW_HOME_SCREEN, ShowHomeScreen);
        EventManager<object>.UnregisterEvent(EventKey.HIDE_HOME_SCREEN, HideHomeScreen);
        EventManager<object>.UnregisterEvent(EventKey.SHOW_PAUSE_MENU, ShowPauseMenu);
        EventManager<object>.UnregisterEvent(EventKey.HIDE_PAUSE_MENU, HidePauseMenu);
        EventManager<object>.UnregisterEvent(EventKey.SHOW_END_SCREEN, ShowEndGameScreen);
        EventManager<object>.UnregisterEvent(EventKey.HIDE_END_SCREEN, HideEndGameScreen);
        EventManager<object>.UnregisterEvent(EventKey.SHOW_SETTINGS_MENU, ShowSettingsMenu);
        EventManager<object>.UnregisterEvent(EventKey.HIDE_SETTINGS_MENU, HideSettingsMenu);
        Debug.Log("UI events unregistered.");
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        EventManager<GameState>.TriggerEvent(EventKey.GAME_STATE_CHANGED, newState);
        UpdateUI();

        // Reset the player when returning to HOME
        if (newState == GameState.HOME)
        {
            InstantiatePlayer();
        }
    }

    private void UpdateUI()
    {
        homeCanvas.gameObject.SetActive(currentState == GameState.HOME);
        gameCanvas.gameObject.SetActive(currentState == GameState.GAME);
        pauseCanvas.gameObject.SetActive(currentState == GameState.PAUSE);
        settingsCanvas.gameObject.SetActive(currentState == GameState.SETTINGS);
        levelUpCanvas.gameObject.SetActive(currentState == GameState.LEVELUP);
        endCanvas.gameObject.SetActive(currentState == GameState.END);

        Debug.Log($"UI updated to state: {currentState}");
    }

    // UI Display Methods
    private void ShowHomeScreen(object obj) => ChangeState(GameState.HOME);
    private void HideHomeScreen(object obj) => homeCanvas.gameObject.SetActive(false);

    private void ShowGameUI(object obj) => ChangeState(GameState.GAME);
    private void HideGameUI(object obj) => gameCanvas.gameObject.SetActive(false);

    private void ShowLevelUpUI(object obj) => ChangeState(GameState.LEVELUP);
    private void HideLevelUpUI(object obj)
    {
        levelUpCanvas.gameObject.SetActive(false);
        ChangeState(GameState.GAME);
    }

    private void ShowPauseMenu(object obj)
    {
        ChangeState(GameState.PAUSE);
    }

    private void HidePauseMenu(object obj) => ChangeState(GameState.GAME);

    private void ShowEndGameScreen(object obj) => ChangeState(GameState.END);
    private void HideEndGameScreen(object obj) => endCanvas.gameObject.SetActive(false);

    private void ShowSettingsMenu(object obj) => ChangeState(GameState.SETTINGS);
    private void HideSettingsMenu(object obj) => settingsCanvas.gameObject.SetActive(false);

    public void Quit() => Application.Quit();
    public void GoHome() {
        ChangeState(GameState.HOME);
        EventManager<object>.TriggerEvent(EventKey.RESTART, null);
    }
}
