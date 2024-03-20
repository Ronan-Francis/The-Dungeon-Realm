using UnityEngine;

// The class abstract
public abstract class Item : MonoBehaviour
{
    public Sprite itemImage; // Assign this in the editor

    // Abstract method to define specific item usage
    public abstract void Use();

    // Method to call when item is picked up
    public void Pickup()
    {
        // Logic to add item to the inventory goes here
        InventorySystem.instance.AddItem(this.itemImage);
        
        // Optionally, perform additional actions like playing a sound or animation
        
        // Destroy the item game object
        Destroy(gameObject);
    }
}
