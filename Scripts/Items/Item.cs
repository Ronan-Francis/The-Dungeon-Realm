using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public Sprite itemImage; // Assign this in the editor
    public int durability = 100; // Default durability for items that require it
    public float attackSpeed = 1f; // Default attack speed for weapons

    // Abstract method to define specific item usage
    public abstract void Use();

    // Method to call when item is picked up
public void Pickup()
{
    InventorySystem.instance.AddItem(this.itemImage);
    // Optionally, perform additional actions like playing a sound or animation
    gameObject.SetActive(false);

    // Set this item's parent to be the transform of the InventorySystem instance
    // Assumes InventorySystem is a MonoBehaviour and thus has a transform property
    gameObject.transform.SetParent(InventorySystem.instance.transform, false);
}

    // Method to enhance the item, increasing its effectiveness
    public virtual void Enhance()
    {
        // Default implementation could be empty or provide a basic enhancement
    }

    // Repair the item
    public void Repair()
    {
        durability = 100; // Reset durability, consider making this more nuanced
        Debug.Log($"{name} has been repaired.");
    }
}
