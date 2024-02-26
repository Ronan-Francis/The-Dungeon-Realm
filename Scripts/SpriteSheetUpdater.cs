using UnityEngine;

public class SpriteSheetUpdater : MonoBehaviour
{
    public Sprite[] sprites; // Array to hold the sliced sprites from the sprite sheet
    public GameObject gameObjectToUpdate; // The GameObject to update

    // Function to update the sprite of the GameObject
    public void UpdateSprite(int spriteIndex)
    {
        if (gameObjectToUpdate != null && sprites != null && spriteIndex < sprites.Length)
        {
            SpriteRenderer spriteRenderer = gameObjectToUpdate.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprites[spriteIndex];
            }
            else
            {
                Debug.LogError("GameObjectToUpdate does not have a SpriteRenderer component.");
            }
        }
        else
        {
            Debug.LogError("Invalid sprite index or sprites not set.");
        }
    }
}
