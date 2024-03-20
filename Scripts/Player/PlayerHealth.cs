using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public float knockbackStrength = 3f;
    public float knockbackDuration = 0.2f;
    private PlayerMovement playerMovement;
    private Vector2 lastKnockbackDirection;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            knockbackDirection.y += 0.5f; // Optionally add some vertical component
            
            lastKnockbackDirection = knockbackDirection;
            StartCoroutine(ApplyKnockback(knockbackDirection, knockbackDuration));
            health -= 10; // Adjust health reduction as needed
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            playerMovement.SetMovementEnabled(false);
        }
        //playerMovement.SetMovementEnabled(true);
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
}
