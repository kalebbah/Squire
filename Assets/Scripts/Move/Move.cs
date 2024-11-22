using UnityEngine;

public abstract class Move : MonoBehaviour
{
    public int speed = 5;
    public float rotationSpeed = 10f;  // Speed for smooth rotation
    protected bool isPaused = false;

    protected virtual void Start()
    {
        // Register for the game state change event
        EventManager<GameState>.RegisterEvent(EventKey.GAME_STATE_CHANGED, OnGameStateChanged);
        EventManager<int>.RegisterEvent(EventKey.MOVEMENT_SPEED_LVLUP, UpdateSpeed);
    }

    private void OnDestroy()
    {
        // Unregister from the event when this object is destroyed
        EventManager<GameState>.UnregisterEvent(EventKey.GAME_STATE_CHANGED, OnGameStateChanged);
        EventManager<int>.UnregisterEvent(EventKey.MOVEMENT_SPEED_LVLUP, UpdateSpeed);

    }

    private void OnGameStateChanged(GameState newState)
    {
        // Update the isPaused flag based on the game state
        isPaused = newState != GameState.GAME;
    }
    private void UpdateSpeed(int toSpeed) {
        speed = toSpeed;
    }

    // Abstract method that derived classes will implement
    public abstract void PerformMove();
    public abstract void PerformPause();

    protected void CheckAndMove()
    {
        // Only perform movement if the game is not paused
        if (!isPaused)
        {
            PerformMove();
        } else  
            PerformPause();
    }
}
