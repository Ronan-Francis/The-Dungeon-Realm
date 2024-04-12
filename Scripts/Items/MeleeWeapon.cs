using UnityEngine;
using System.Collections; // Required for IEnumerator

public class MeleeWeapon : Item
{
    public int damage = 10; // Default damage
    private float lastAttackTime = 0f; // Declare lastAttackTime to track the cooldown
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
<<<<<<< HEAD
    public float attackCooldown = 1f; // Cooldown time in seconds between attacks
=======
>>>>>>> 8ad74047893e2afc59c46bd5f345b8211468a3d7

    public override void Use()
    {
        // Ensure the weapon can only be used if enough time has passed based on attackSpeed
<<<<<<< HEAD
        // and if it is not currently attacking (cooldown)
        if (Time.time - lastAttackTime >= attackCooldown / attackSpeed && durability > 0)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        lastAttackTime = Time.time; // Update lastAttackTime to the current time

        // Rotate the item by -90 degrees around the Z-axis
        transform.Rotate(0, 0, -90);

        Attack();

        // Wait for a short delay before rotating back to give the visual effect of a swing
        yield return new WaitForSeconds(0.25f); // Adjust this time to control the delay

        // Rotate the item back by 90 degrees around the Z-axis to its original orientation
        transform.Rotate(0, 0, 90);

=======
        if (Time.time - lastAttackTime >= 1f / attackSpeed && durability > 0)
        {
            Attack();
        }
>>>>>>> 8ad74047893e2afc59c46bd5f345b8211468a3d7
    }

    private void Attack()
    {
<<<<<<< HEAD
=======
        lastAttackTime = Time.time; // Update lastAttackTime to the current time
>>>>>>> 8ad74047893e2afc59c46bd5f345b8211468a3d7
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
<<<<<<< HEAD
=======

>>>>>>> 8ad74047893e2afc59c46bd5f345b8211468a3d7
    }

    // Optionally override the Enhance method for specific enhancement behavior
    public override void Enhance()
    {
        damage += 5; // Simple enhancement logic for melee weapons
        Debug.Log($"{name} has been enhanced. New damage: {damage}");
    }
}
