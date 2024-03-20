using UnityEngine;

public class Split : MonoBehaviour
{
    // Assign this through the Inspector to specify the enemy prefab you want to spawn.
    public GameObject enemyPrefab;

    // You can call this method at the point you want the split to happen.
    // For example, this could be triggered upon the GameObject's death.
    public void TriggerSplit()
    {
        if(enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned. Please assign it in the inspector.");
            return;
        }

        // The position where the current GameObject is located.
        Vector3 position = transform.position;

        // Destroy the current GameObject.
        Destroy(gameObject);

        // Instantiate the first new enemy at the position of the current GameObject.
        Instantiate(enemyPrefab, position, Quaternion.identity);

        // Optionally, you might want to offset the second enemy's position a bit.
        Vector3 offsetPosition = position + new Vector3(1.0f, 0, 0); // Example offset; adjust as needed.
        
        // Instantiate the second new enemy.
        Instantiate(enemyPrefab, offsetPosition, Quaternion.identity);
    }
}
