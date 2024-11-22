using System.Collections;
using UnityEngine;
using TMPro;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;

    [Tooltip("Prefab for displaying damage numbers.")]
    public GameObject damageNumberPrefab;

    [Tooltip("Toggle for displaying damage numbers.")]
    public bool showDamageNumbers = true;

    private Camera mainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            mainCamera = Camera.main;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DisplayDamageNumber(int damage, Vector3 position)
    {
        if (!showDamageNumbers) return;

        // Offset the position slightly above the enemy
        Vector3 spawnPosition = position + Vector3.up * 1.5f;
        GameObject damageText = Instantiate(damageNumberPrefab, spawnPosition, Quaternion.identity);

        TextMeshPro textMesh = damageText.GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.text = StatManager.Instance.CalculateDamage(damage).ToString();
            StartCoroutine(FadeAndDestroy(damageText));
        }

        // Make the damage text face the camera
        damageText.transform.LookAt(damageText.transform.position + mainCamera.transform.forward);
    }

    private IEnumerator FadeAndDestroy(GameObject damageText)
    {
        float duration = 1f;  // Duration for fade out
        TextMeshPro textMesh = damageText.GetComponent<TextMeshPro>();
        Color originalColor = textMesh.color;

        // Generate a random direction vector for drift
        Vector3 randomDriftDirection = new Vector3(Random.Range(-0.5f, 0.5f), 1, Random.Range(-0.5f, 0.5f)).normalized;
        float driftSpeed = 0.5f;  // Adjust drift speed as needed

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // Move the text in the random direction with a slight upward drift
            damageText.transform.position += randomDriftDirection * driftSpeed * Time.deltaTime;
            textMesh.color = Color.Lerp(originalColor, Color.clear, t / duration);
            yield return null;
        }

        Destroy(damageText);
    }

    public void ToggleDamageNumbers(bool enabled)
    {
        showDamageNumbers = enabled;
    }
}
