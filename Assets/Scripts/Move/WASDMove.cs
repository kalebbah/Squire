using UnityEngine;

public class WASDMove : Move
{
    void Update()
    {
        CheckAndMove();  // Only move if not paused
    }

    public override void PerformMove()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrow keys
        float vertical = Input.GetAxisRaw("Vertical");     // W/S or Up/Down arrow keys

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude > 0.1f)
        {
            Vector3 targetPosition = transform.position + direction * speed * Time.deltaTime;
            //Debug.Log(speed);
            targetPosition.y = 0f;  // Keep the y-position fixed

            // Move the player to the target position
            transform.position = targetPosition;

            // Smoothly rotate the player to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    public override void PerformPause() {
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero; // Stop movement
        return;
    }
}
