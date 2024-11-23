using UnityEngine;

public class EnemyAIMove : Move
{
    private Transform playerTransform; // Reference to the player's transform
    private bool isPlayerSet = false;  // Flag to track if the player has been assigned

    void Awake()
    {
        EventManager<GameObject>.RegisterEvent(EventKey.PLAYER_INSTANTIATE, PlayerDependencies);
    }

    protected override void Start()
    {
        base.Start();

        // Check if the player is already instantiated
        GameObject existingPlayer = GameManager.Instance?.GetPlayerInstance();
        if (existingPlayer != null)
        {
            PlayerDependencies(existingPlayer);
        }
    }

    private void PlayerDependencies(GameObject player)
    {
        if (player != null)
        {
            playerTransform = player.transform;
            isPlayerSet = true;
            Debug.Log($"Enemy assigned player transform: {player.name}");
        }
        else
        {
            Debug.LogWarning("PlayerDependencies called with a null player!");
        }
    }

    void Update()
    {
        CheckAndMove(); // Only moves if isPaused is false
    }

    public override void PerformMove()
    {
        if (playerTransform != null && isPlayerSet)
        {
            // Calculate direction toward the player
            Vector3 direction = (playerTransform.position - transform.position).normalized;

            // Calculate target position, locking y-axis
            Vector3 targetPosition = transform.position + direction * speed * Time.deltaTime;

            // Move enemy toward the target position
            transform.position = targetPosition;

            // Smoothly rotate to face the player
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public override void PerformPause()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero; // Stop movement
        }
    }

    private void OnApplicationQuit()
    {
        EventManager<GameObject>.UnregisterEvent(EventKey.PLAYER_INSTANTIATE, PlayerDependencies);
    }
}
