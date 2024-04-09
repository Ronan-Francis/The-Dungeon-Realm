using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health;
    public float knockbackStrength = 3f;
    public float knockbackDuration = 0.2f;
    private PlayerMovement playerMovement;
    private Vector2 lastKnockbackDirection;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        health = maxHealth; // Initialize health to maxHealth at start
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            knockbackDirection.y += 0.5f; // Optionally add some vertical component
            
            lastKnockbackDirection = knockbackDirection;
            StartCoroutine(ApplyKnockback(knockbackDirection, knockbackDuration));
            TakeDamage(10); // Example damage value
        }
    }

    IEnumerator ApplyKnockback(Vector2 direction, float duration)
    {
        DisableMovement();

        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            transform.position += (Vector3)direction * knockbackStrength * Time.deltaTime;
            yield return null;
        }

        EnableMovement();
    }

    void DisableMovement()
    {
        playerMovement.canMoveRight = false;
        playerMovement.canMoveLeft = false;
        playerMovement.canMoveUp = false;
        playerMovement.canMoveDown = false;
    }

    void EnableMovement()
    {
        playerMovement.canMoveRight = true;
        playerMovement.canMoveLeft = true;
        playerMovement.canMoveUp = true;
        playerMovement.canMoveDown = true;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        health = Mathf.Max(health, 0); // Ensure health doesn't drop below 0
        if (health <= 0)
        {
            // Handle player death here (e.g., trigger animation, game over screen)
        }
    }

    public void Heal(float amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth); // Ensure health doesn't exceed maxHealth
        // Optionally, trigger a healing effect or sound
    }
}
