using UnityEngine;

public class CameraMove : Move
{
    private string playerTag = "Player";      // Tag to identify the player
    private Transform playerTransform;       // Reference to the player's transform
    private Vector3 offset;                  // Offset of the camera relative to the player
    private Quaternion initialRotation;      // Initial rotation of the camera
    private Vector3 initialPosition = new Vector3(0, 9, -8); // Starting position of the camera
    private float retryDelay = 1f;           // Time between retry attempts to find the player

    void Awake()
    {
        // Register for the player instantiation and game restart events
        EventManager<GameObject>.RegisterEvent(EventKey.PLAYER_INSTANTIATE, PlayerDependencies);
        EventManager<object>.RegisterEvent(EventKey.RESTART, ResetCameraPosition);
    }

    protected override void Start()
    {
        base.Start();
        initialRotation = transform.rotation; // Capture the initial rotation of the camera
        TryFindPlayer();                      // Attempt to find the player if not already assigned
    }

    private void PlayerDependencies(GameObject player)
    {
        if (player != null)
        {
            playerTransform = player.transform;
            offset = initialPosition - playerTransform.position; // Recalculate offset
            SnapToPlayer(); // Immediately snap to the new player position
            Debug.Log("Camera assigned player transform via event.");
        }
    }

    void LateUpdate()
    {
        if (playerTransform == null)
        {
            TryFindPlayer(); // Retry finding the player if the reference is lost
        }

        CheckAndMove(); // Only move if not paused
    }

    public override void PerformMove()
    {
        if (playerTransform != null)
        {
            Vector3 targetPosition = playerTransform.position + offset;

            // Smoothly move the camera towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

            // Lock the rotation to the initial rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, speed * Time.deltaTime);
        }
    }

    public override void PerformPause()
    {
        // Camera remains static during pause
        return;
    }

    private void TryFindPlayer()
    {
        GameObject playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            offset = initialPosition - playerTransform.position; // Recalculate offset
            SnapToPlayer(); // Immediately snap to the new player position
            Debug.Log("Camera successfully found player using tag.");
        }
        else
        {
            Debug.LogWarning($"Camera could not find player with tag '{playerTag}'. Retrying in {retryDelay} seconds...");
            Invoke(nameof(TryFindPlayer), retryDelay); // Retry after a delay
        }
    }

    private void SnapToPlayer()
    {
        if (playerTransform != null)
        {
            // Instantly position the camera at the correct position
            transform.position = playerTransform.position + offset;

            // Instantly set the camera rotation to the initial rotation
            transform.rotation = initialRotation;
            Debug.Log("Camera snapped to player position.");
        }
    }

    private void ResetCameraPosition(object obj)
    {
        Debug.Log("Resetting camera to initial position and rotation.");
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // Reattempt to find the player after reset
        playerTransform = null;
        TryFindPlayer();
    }

    private void OnApplicationQuit()
    {
        EventManager<GameObject>.UnregisterEvent(EventKey.PLAYER_INSTANTIATE, PlayerDependencies);
        EventManager<object>.UnregisterEvent(EventKey.RESTART, ResetCameraPosition);
    }
}
