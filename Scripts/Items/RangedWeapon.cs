using UnityEngine;

public class RangedWeapon : Item
{
    public int damage = 8; // Default damage
    public int ammoCapacity = 30;
    private int currentAmmo;
    private float lastAttackTime = 0f; // Declare lastAttackTime to track the cooldown

    // Reference to the projectile prefab
    public GameObject projectilePrefab;

    // Fire point from where the projectile will be instantiated
    public Transform firePoint;

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

        // Instantiate the projectile at the fire point's position and orientation
        if (projectilePrefab && firePoint)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            // Here you can set the projectile's damage or any other properties if needed
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDamage(damage);
            }
        }

        if (durability <= 0)
        {
            Debug.Log($"{name} has broken.");
        }
    }

    public override void Enhance()
    {
        damage += 2; // Example of a simple enhancement for ranged weapons
        ammoCapacity += 10;
        Debug.Log($"{name} has been enhanced. New damage: {damage}, New ammo capacity: {ammoCapacity}");
    }
}
