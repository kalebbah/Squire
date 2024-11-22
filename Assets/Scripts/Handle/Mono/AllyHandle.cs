using System;
using UnityEngine;

[Serializable]
public class AllyHandle : MonoBehaviour
{
    [SerializeField] private int editorMaxHealth = 100;
    private HealthHandle healthHandle;

    public float collisionDamage = 10f;

    private void Start()
    {
        healthHandle = new HealthHandle(editorMaxHealth, editorMaxHealth);
    }

    // Ally Puts Damage on Enemy
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyHandle>().GetHealthHandle().TakeDamage((int)collisionDamage, collision.gameObject);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (healthHandle.TakeDamage(damageAmount, gameObject))
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        healthHandle.Heal(healAmount);
    }

    private void Die()
    {
        Debug.Log("Ally has died.");
        Destroy(gameObject);
    }
    public HealthHandle GetHealthHandle() => healthHandle;
}
