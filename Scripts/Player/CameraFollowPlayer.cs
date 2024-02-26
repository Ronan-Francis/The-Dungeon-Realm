using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public Vector3 offset; // Offset distance between the player and camera
    private Vector3 lastPlayerPosition; // To store the last position of the player
    private bool isPlayerMoving; // Flag to check if the player has moved

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Initialize lastPlayerPosition with the player's initial position
        transform.position = player.transform.position + offset;
        lastPlayerPosition = player.transform.position;
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // Check if the player has moved by comparing current position with the last known position
            isPlayerMoving = (player.transform.position != lastPlayerPosition);

            if (isPlayerMoving)
            {
                // Update the camera's position only if the player moved
                transform.position = player.transform.position + offset;
                // Update lastPlayerPosition with the new position
                lastPlayerPosition = player.transform.position;
            }
        }
    }
}
