using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public int sceneToLoad; // The name of the scene to load

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the colliding object is the player
        {
            LoadScene();
            Debug.Log("Player");
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad); // Load the specified scene
    }
}
