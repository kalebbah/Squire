using UnityEngine;

public class EnemyAIMove : Move
{
    private string playerTag = "Player";    // Tag to identify the player
    private Transform player;               // Reference to the player's transform

    protected override void Start()
    {
        base.Start();

        // Find the player GameObject by tag and get its transform
        GameObject playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning($"No GameObject with tag '{playerTag}' found.");
        }

    }

    void Update()
    {
        CheckAndMove();  // Only moves if isPaused is false
    }

    public override void PerformMove()
    {
        if (player != null)
        {
            // Calculate direction toward the player
            Vector3 direction = (player.position - transform.position).normalized;

            // Calculate target position, locking y-axis
            Vector3 targetPosition = transform.position + direction * speed * Time.deltaTime;

            // Move enemy toward the target position
            transform.position = targetPosition;

            // Smoothly rotate to face the player
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    public override void PerformPause() {
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero; // Stop movement
        return;
    }
}
