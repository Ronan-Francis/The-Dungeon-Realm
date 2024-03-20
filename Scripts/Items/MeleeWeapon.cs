using UnityEngine;

public class MeleeWeapon : Item
{
    public int damage = 10; // Default damage, adjust as necessary
    public float range = 5f; // Default range, adjust as necessary
    // Add more weapon-specific properties here, such as attack speed, durability, etc.

    // Implement the Use method for the weapon
    // This could be triggering an attack animation and calculating damage to an enemy
    public override void Use()
    {
        Attack();
    }

    // Example Attack method demonstrating what might happen when a weapon is used
    private void Attack()
    {
        Debug.Log($"Attacking with {name}: {damage} damage over {range} range.");
        // Implement the attack logic here
        // For instance, you might check for enemies within `range` and apply `damage` to them
    }

    // Additional weapon-specific methods can be added here
    // For example, a method to enhance the weapon, repair it, etc.
}
