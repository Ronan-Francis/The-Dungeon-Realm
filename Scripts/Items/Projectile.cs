using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f; // Speed at which the projectile moves
    public int damage = 10; // Damage dealt by the projectile

    void Start()
    {
        // Rotate the projectile by -45 degrees
        transform.Rotate(0, 0, -45);

        // Assuming you're using Rigidbody2D and moving in a 2D space now,
        // the forward vector won't apply. Use right for 2D space if moving horizontally.
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }

    void Update()
    {
        // Removed Move() as it conflicts with Rigidbody's physics.
        // Movement is now handled entirely by the Rigidbody2D's velocity.
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the collided object has an EnemyHealth component and reduce its health if so
        var health = collider.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            // Destroy the projectile since it hit an enemy
            Destroy(gameObject);
        }
        // Additionally, check if the projectile collides with a wall
        else if (collider.CompareTag("Walls"))
        {
            // Destroy the projectile since it hit a wall
            Destroy(gameObject);
        }
        // No action is taken if the projectile collides with other objects, like the player
    }
}
