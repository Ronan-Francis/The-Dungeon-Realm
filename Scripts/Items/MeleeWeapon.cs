using UnityEngine;

public class MeleeWeapon : Item
{
    public int damage = 10; // Default damage
    private float lastAttackTime = 0f; // Declare lastAttackTime to track the cooldown
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    public override void Use()
    {
        // Ensure the weapon can only be used if enough time has passed based on attackSpeed
        if (Time.time - lastAttackTime >= 1f / attackSpeed && durability > 0)
        {
            Attack();
        }
    }

    private void Attack()
    {
        lastAttackTime = Time.time; // Update lastAttackTime to the current time
        Debug.Log($"Attacking with {name}: {damage} damage.");
        durability--; // Decrease durability with each attack
        if (durability <= 0)
        {
            Debug.Log($"{name} has broken.");
        }

        // Implementation for checking enemies within range and applying damage
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(damage);
        }

    }

    // Optionally override the Enhance method for specific enhancement behavior
    public override void Enhance()
    {
        damage += 5; // Simple enhancement logic for melee weapons
        Debug.Log($"{name} has been enhanced. New damage: {damage}");
    }
}
