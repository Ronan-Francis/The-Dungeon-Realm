using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health of the enemy
    private int currentHealth; // Current health of the enemy

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth; // Initialize current health
    }

    // Method to call when the enemy takes damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce current health by the damage amount
        
        if (currentHealth <= 0) // Check if health has dropped to zero or below
        {
            Die(); // Call the die method
        }
    }

    // Method to call when the enemy dies
    void Die()
    {
        // Here you could call another method to handle what happens when the enemy dies,
        // such as spawning new enemies, playing effects, etc.
        Debug.Log("Enemy died!"); // Placeholder action

        Destroy(gameObject); // Destroy the enemy game object
    }
}
