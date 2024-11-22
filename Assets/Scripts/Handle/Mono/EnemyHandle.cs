using System;
using UnityEngine;

[Serializable]
public class EnemyHandle : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int MaxHealth = 100;
    public int ExperienceReward = 50;

    [Header("Handles")]
    public HealthHandle HealthHandle;
    public CollisionHandle collisionHandle;

    [Header("Reward Settings")]
    [SerializeField] private RewardHandle rewardHandle = new RewardHandle(minCoins: 1, maxCoins: 3, gemDropChance: 0.2f, minGems: 1, maxGems: 5);

    private float lastDamageTime = 0f;
    private const float damageCooldown = 0.3f;
    private bool isPaused = false;

    void Start()
    {
        HealthHandle = new HealthHandle(MaxHealth, MaxHealth);
        HealthHandle.OnDeath += OnDefeated;  // Subscribe to the death event

        collisionHandle = GetComponent<CollisionHandle>();
        collisionHandle.OnCollisionEnterAction = HandleCollisionEnter;
        collisionHandle.OnCollisionStayAction = HandleCollisionStay;

        EventManager<GameState>.RegisterEvent(EventKey.GAME_STATE_CHANGED, OnGameStateChanged);
    }

    private void OnDestroy()
    {
        HealthHandle.OnDeath -= OnDefeated;  // Unsubscribe from the event
        EventManager<GameState>.UnregisterEvent(EventKey.GAME_STATE_CHANGED, OnGameStateChanged);
    }

    private void OnGameStateChanged(GameState newState)
    {
        isPaused = newState != GameState.GAME;
    }

    private void HandleCollisionEnter(GameObject collider)
    {
        if (isPaused) return;

        if (collider.CompareTag("Player"))
        {
            collider.GetComponent<PlayerHandle>().GetHealthHandle().TakeDamage((int)collisionHandle.collisionDamage, collider);
        }
        else if (collider.CompareTag("Ally"))
        {
            collider.GetComponent<AllyHandle>().GetHealthHandle().TakeDamage((int)collisionHandle.collisionDamage, collider);
        }
    }

    private void HandleCollisionStay(GameObject collider)
    {
        if (isPaused) return;

        if (Time.time - lastDamageTime >= damageCooldown)
        {
            lastDamageTime = Time.time;
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerHandle>().GetHealthHandle().TakeDamage((int)collisionHandle.collisionDamage, collider);
            }
            else if (collider.CompareTag("Ally"))
            {
                collider.GetComponent<AllyHandle>().GetHealthHandle().TakeDamage((int)collisionHandle.collisionDamage, collider);
            }
        }
    }

    private void OnDefeated()
    {
        EventManager<int>.TriggerEvent(EventKey.UPDATE_PLAYER_EXPERIENCE, ExperienceReward);
        rewardHandle.OnDefeated();
        Debug.Log("Enemy defeated and rewards handled.");
        Destroy(gameObject);
    }

    public HealthHandle GetHealthHandle() => HealthHandle;
}
