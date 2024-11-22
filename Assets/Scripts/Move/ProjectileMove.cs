using UnityEngine;

public class ProjectileMove : Move
{
    private Vector3 direction;
    private bool initialized = false;

    public void Initialize(Vector3 targetPosition)
    {
        // Set the direction towards the target position
        direction = (targetPosition - transform.position).normalized;
        initialized = true;
    }

    protected override void Start()
    {
        base.Start(); // Call the base class Start to register event listeners
    }

    private void Update()
    {
        CheckAndMove(); // Call the base class method to check if the game is paused
    }

    public override void PerformMove()
    {
        // Only move if the projectile has been initialized with a target direction
        if (initialized)
        {
            // Move the projectile in the direction set during initialization
            transform.position += direction * speed * Time.deltaTime;
        }
    }
    public override void PerformPause() {
        //transform.GetComponent<Rigidbody>().velocity = Vector3.zero; // Stop movement
        return;
    }
}
