using UnityEngine;

public class AllyAIMove : Move
{
    private string enemyTag = "Enemy";    // Tag to identify enemies
    private Transform targetEnemy;        // Reference to the nearest enemy's transform

    protected override void Start()
    {
        base.Start();
        FindNearestEnemy();  // Locate the nearest enemy initially
    }

    void Update()
    {
        // Continuously check for nearby enemies in case the target changes
        FindNearestEnemy();
        CheckAndMove();  // Only moves if isPaused is false
    }

    private void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        targetEnemy = closestEnemy;
    }

    public override void PerformMove()
    {
        if (targetEnemy != null)
        {
            // Calculate direction toward the nearest enemy
            Vector3 direction = (targetEnemy.position - transform.position).normalized;

            // Calculate target position, locking y-axis
            Vector3 targetPosition = transform.position + direction * speed * Time.deltaTime;

            // Move ally toward the target position
            transform.position = targetPosition;

            // Smoothly rotate to face the enemy
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    public override void PerformPause() {
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero; // Stop movement
        return;
    }
}
