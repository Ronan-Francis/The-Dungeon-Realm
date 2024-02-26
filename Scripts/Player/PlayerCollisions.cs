using UnityEngine;
using System;
using System.Collections;

public class PlayerCollisions : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Coroutine destroyCoroutine;
    public GameObject floorTilePrefab;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on the player object.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Walls"))
        {
            playerMovement.moveInput = Vector2.zero;
            playerMovement.isTouchingWall = true;
            Vector3 hitPosition = transform.InverseTransformPoint(other.ClosestPoint(transform.position));

            // Start the coroutine and keep a reference to it
            destroyCoroutine = StartCoroutine(ReplaceWithFloorTile(other.gameObject, playerMovement.breakTime));

            UpdateMovementRestrictions(hitPosition, true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Walls"))
        {
            playerMovement.isTouchingWall = false;

            // If exiting the collider, stop the coroutine to prevent destruction
            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
                destroyCoroutine = null; // Clear the reference
            }

            // Reset movement restrictions
            ResetMovementRestrictions();
        }
    }

    private void UpdateMovementRestrictions(Vector3 hitPosition, bool restrict)
    {
        if (Mathf.Abs(hitPosition.x) > Mathf.Abs(hitPosition.y))
        {
            if (hitPosition.x > 0) playerMovement.canMoveRight = !restrict;
            else playerMovement.canMoveLeft = !restrict;
        }
        else
        {
            if (hitPosition.y > 0) playerMovement.canMoveUp = !restrict;
            else playerMovement.canMoveDown = !restrict;
        }
    }

    private void ResetMovementRestrictions()
    {
        // Assuming default state is that the player can move freely in all directions.
        playerMovement.canMoveRight = true;
        playerMovement.canMoveLeft = true;
        playerMovement.canMoveUp = true;
        playerMovement.canMoveDown = true;
    }


    // Coroutine to destroy the object after a delay
    IEnumerator DestroyAfterTimeWithPerlinNoise(GameObject objectToDestroy, float delay, float animationDuration)
    {
        float startTime = Time.time;
        float endTime = startTime + animationDuration;
        Vector3 originalScale = objectToDestroy.transform.localScale;

        // Animation loop
        while (Time.time < endTime)
        {
            float elapsedTime = Time.time - startTime;
            float fraction = elapsedTime / animationDuration;

            // Generate Perlin noise based scale factor
            float noise = Mathf.PerlinNoise(elapsedTime * 0.5f, 0) * 0.5f + 0.5f; // Adjust parameters as needed
            Vector3 animatedScale = originalScale * noise * (1 - fraction); // Decrease size over time

            objectToDestroy.transform.localScale = animatedScale;

            yield return null; // Wait until next frame
        }

        // Wait for any remaining delay
        if (delay > animationDuration)
        {
            yield return new WaitForSeconds(delay - animationDuration);
        }

        // Finally, destroy the object
        Destroy(objectToDestroy);
    }


    IEnumerator ReplaceWithFloorTile(GameObject objectToReplace, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Instantiate floor tile at the position and rotation of the object to replace
        if (objectToReplace != null) // Check if the object hasn't been destroyed already
        {
            Instantiate(floorTilePrefab, objectToReplace.transform.position, objectToReplace.transform.rotation);

            // Destroy the original object
            Destroy(objectToReplace);
        }
    }
}
