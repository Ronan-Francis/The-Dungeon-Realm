using UnityEngine;

public class RangedWeapon : Item
{
    public int damage = 8; // Default damage
    public int ammoCapacity = 30;
    private int currentAmmo;
    private float lastAttackTime = 0f; // Declare lastAttackTime to track the cooldown

    public RangedWeapon()
    {
        currentAmmo = ammoCapacity;
    }

    public override void Use()
    {
        if (Time.time - lastAttackTime >= 1f / attackSpeed && durability > 0 && currentAmmo > 0)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        lastAttackTime = Time.time;
        Debug.Log($"Shooting with {name}: {damage} damage.");
        currentAmmo--;
        durability--;
        if (durability <= 0)
        {
            Debug.Log($"{name} has broken.");
        }
    }

    // Specific Enhance method for ranged weapons could improve damage, ammo capacity, etc.
    public override void Enhance()
    {
        damage += 2; // Example of a simple enhancement for ranged weapons
        ammoCapacity += 10;
        Debug.Log($"{name} has been enhanced. New damage: {damage}, New ammo capacity: {ammoCapacity}");
    }
}
