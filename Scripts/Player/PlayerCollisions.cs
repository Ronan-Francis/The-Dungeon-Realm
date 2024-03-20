using UnityEngine;
using System;
using System.Collections;

public class PlayerCollisions : MonoBehaviour
{
    private PlayerMovement playerMovement; // To control the player's movement
    private Coroutine destroyCoroutine; // To keep track of the coroutine
    public GameObject floorTilePrefab; // Prefab to replace walls with

    private void Start()
    {
        // Ensure that the PlayerMovement component is attached to the same GameObject
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on the player object.");
        }
    }

    // Handle entering wall collisions
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Walls"))
        {
            Debug.Log("Wall");
            // Stop the player and mark them as touching a wall
            playerMovement.SetMovementEnabled(false);
            playerMovement.isTouchingWall = true;

            // Calculate collision position relative to the player
            Vector3 hitPosition = transform.InverseTransformPoint(other.ClosestPoint(transform.position));

            // Begin replacing the wall with a floor tile after a delay
            destroyCoroutine = StartCoroutine(ReplaceWithFloorTile(other.gameObject, playerMovement.breakTime));

            // Update movement restrictions based on collision side
            UpdateMovementRestrictions(hitPosition, true);
        }
        else if (other.CompareTag("Item"))
        {
            // Attempt to get the Item component from the collided object
            Item item = other.GetComponent<Item>();

            // Check if the object actually has an Item component
            if (item != null)
            {
                item.Pickup();
            }
        }
    }


    // Handle exiting wall collisions
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Walls"))
        {
            playerMovement.isTouchingWall = false;

            // Stop the replace coroutine if it's still running
            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
                destroyCoroutine = null;
            }

            // Reset movement restrictions
            ResetMovementRestrictions();
        }
    }

    // Update movement restrictions based on the collision's position
    private void UpdateMovementRestrictions(Vector3 hitPosition, bool restrict)
    {
        if (Mathf.Abs(hitPosition.x) > Mathf.Abs(hitPosition.y))
        {
            playerMovement.canMoveRight = hitPosition.x <= 0 || !restrict;
            playerMovement.canMoveLeft = hitPosition.x > 0 || !restrict;
        }
        else
        {
            playerMovement.canMoveUp = hitPosition.y <= 0 || !restrict;
            playerMovement.canMoveDown = hitPosition.y > 0 || !restrict;
        }
    }

    // Reset movement restrictions to allow free movement in all directions
    private void ResetMovementRestrictions()
    {
        playerMovement.canMoveRight = playerMovement.canMoveLeft = playerMovement.canMoveUp = playerMovement.canMoveDown = true;
    }

    // Coroutine to replace the collided wall with a floor tile after a delay
    private IEnumerator ReplaceWithFloorTile(GameObject objectToReplace, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (objectToReplace != null) // Ensure the object hasn't already been destroyed
        {
            Instantiate(floorTilePrefab, objectToReplace.transform.position, objectToReplace.transform.rotation);
            Destroy(objectToReplace);
        }
    }
}
