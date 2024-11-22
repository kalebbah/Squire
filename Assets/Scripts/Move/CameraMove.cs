using UnityEngine;

public class CameraMove : Move
{
    private string playerTag = "Player";    // Tag to identify the player
    private Transform player;               // Reference to the player's transform
    private Vector3 offset;                 // Offset of the camera relative to the player
    private Quaternion initialRotation;     // Initial rotation of the camera

    protected override void Start()
    {
        base.Start();

        // Find the player GameObject by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
            offset = transform.position - player.position;
        }

        initialRotation = transform.rotation;  // Capture the initial rotation of the camera
    }

    void LateUpdate()
    {
        CheckAndMove();  // Only move if not paused
    }

    public override void PerformMove()
    {
        if (player != null)
        {
            Vector3 targetPosition = new Vector3(player.position.x + offset.x, transform.position.y, player.position.z + offset.z);

            // Smoothly move the camera towards the target position without changing y-axis
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

            // Lock the rotation to the initial rotation
            transform.rotation = initialRotation;
        }
    }
    public override void PerformPause() {
        
        return;
    }
}
