using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSlider : GenericSlider<HealthHandle> {
    public Transform playerTransform; // Reference to the player
    public Vector3 offset = new Vector3(0, 2, 0); // Offset above the player
    public GameObject healthBarGameObject; // The container GameObject for the health bar

    private Coroutine hideHealthBarRoutine;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        healthBarGameObject.SetActive(false); // Initially hide the health bar container

        // Check if player already exists and assign it
        GameObject existingPlayer = GameManager.Instance?.GetPlayerInstance();
        if (existingPlayer != null)
        {
            PlayerDependency(existingPlayer);
        }
    }


    private void PlayerDependency(GameObject player) {
        playerTransform = player.transform;
    }

    void Update() {
        if (PlayerManager.Instance?.User.GetHealthHandle().CurrentHealth > 0) {
            UpdatePosition(); // Position the health bar above the player in screen space
        }
    }

    protected override void RegisterEvents() {
        EventManager<HealthHandle>.RegisterEvent(EventKey.UPDATE_SLIDER_HEALTH_DISPLAY, UpdateSlider);
        EventManager<GameObject>.RegisterEvent(EventKey.PLAYER_INSTANTIATE, PlayerDependency);
    }

    protected override void UnregisterEvents() {
        EventManager<GameObject>.UnregisterEvent(EventKey.PLAYER_INSTANTIATE, PlayerDependency);
        EventManager<HealthHandle>.UnregisterEvent(EventKey.UPDATE_SLIDER_HEALTH_DISPLAY, UpdateSlider);
    }

    protected override float CalculateFillAmount(HealthHandle healthData) {
        return (float)healthData.CurrentHealth / healthData.MaxHealth;
    }

    protected override void UpdateSlider(HealthHandle healthData) {
        healthBarGameObject.SetActive(true); // Show the health bar container
        float fillValue = CalculateFillAmount(healthData);

        if (sliderObject != null) {
            // Update fill amount
            sliderObject.GetComponent<Image>().fillAmount = fillValue;
            
            // Set color based on health percentage
            Image healthImage = sliderObject.GetComponent<Image>();
            if (fillValue > 0.6f) {
                healthImage.color = Color.green;
            } else if (fillValue > 0.3f) {
                healthImage.color = Color.yellow;
            } else {
                healthImage.color = Color.red;
            }
        }

        if (hideHealthBarRoutine != null) {
            StopCoroutine(hideHealthBarRoutine);
        }
        hideHealthBarRoutine = StartCoroutine(HideHealthBarAfterDelay(3f));
    }

    private IEnumerator HideHealthBarAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        healthBarGameObject.SetActive(false);  // Hide the health bar container
    }

    private void UpdatePosition() {
        if (playerTransform != null) {
            // Convert player position to screen space and apply the offset
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(playerTransform.position + offset);
            healthBarGameObject.transform.position = screenPosition;
        }
    }

    void OnApplicationQuit() {
        EventManager<GameObject>.UnregisterEvent(EventKey.PLAYER_INSTANTIATE, PlayerDependency);
    }
}
