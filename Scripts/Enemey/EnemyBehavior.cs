using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    public float timeToLiveInSecs = 5f;
    public float sightRadius = 5f; // Adjust the sight radius as needed
    public float moveSpeed = 5f; // Adjust the movement speed as needed
    public float wanderTime = 2f; // Time between new wander directions
    private GameObject player; // To keep a reference to the player
    private Vector3 lastPosition;
    private float lastMoveTime;
    private Rigidbody2D rb;
    private bool isMoving = true; // A flag to control movement

    public bool canMoveRight = true;
    public bool canMoveLeft = true;
    public bool canMoveUp = true;
    public bool canMoveDown = true;

    public bool isTouchingWall;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Make sure your player GameObject has the tag "Player"
        lastPosition = transform.position;
        lastMoveTime = Time.time;
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Wander());
    }

    void Update()
    {
        if (!isMoving) return; // Stop updating movement if the enemy has collided with a wall

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= sightRadius)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            Vector2 adjustedDirection = Vector2.zero;

            // Adjust direction based on movement restrictions
            if (direction.x > 0 && canMoveRight) adjustedDirection.x = direction.x;
            else if (direction.x < 0 && canMoveLeft) adjustedDirection.x = direction.x;

            if (direction.y > 0 && canMoveUp) adjustedDirection.y = direction.y;
            else if (direction.y < 0 && canMoveDown) adjustedDirection.y = direction.y;

            rb.velocity = adjustedDirection.normalized * moveSpeed;

            // Update last move time and position
            if (transform.position != lastPosition)
            {
                lastMoveTime = Time.time;
                lastPosition = transform.position;
            }
        }
        else if (rb.velocity == Vector2.zero)
        {
            StartCoroutine(Wander());
        }

        // Check if the enemy has not moved in the last 5 seconds
        if (Time.time - lastMoveTime >= timeToLiveInSecs)
        {
            Destroy(gameObject); // Destroy the enemy GameObject
        }
    }

    IEnumerator Wander()
    {
        while (!isTouchingWall)
        {
            Vector2 wanderDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, wanderDirection, 1f); // Adjust the distance based on your needs

            // Check if the raycast hit a wall
            if (hit.collider != null && hit.collider.CompareTag("Walls"))
            {
                // Maybe adjust direction or wait before next move attempt
                yield return new WaitForSeconds(wanderTime);
                continue; // Skip this movement and try again
            }

            // If no collision is detected, apply movement
            rb.velocity = wanderDirection * moveSpeed;
            yield return new WaitForSeconds(wanderTime);
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Walls"))
        {
            isTouchingWall = true;
            Vector3 hitPosition = transform.InverseTransformPoint(other.ClosestPoint(transform.position));
            UpdateMovementRestrictions(hitPosition, true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Walls"))
        {
            isTouchingWall = false;
            // Reset movement restrictions when exiting a collision with a wall.
            ResetMovementRestrictions();
        }
    }

    private void UpdateMovementRestrictions(Vector3 hitPosition, bool restrict)
    {
        if (Mathf.Abs(hitPosition.x) > Mathf.Abs(hitPosition.y))
        {
            if (hitPosition.x > 0) canMoveRight = !restrict;
            else canMoveLeft = !restrict;
        }
        else
        {
            if (hitPosition.y > 0) canMoveUp = !restrict;
            else canMoveDown = !restrict;
        }
    }

    private void ResetMovementRestrictions()
    {
        // Assuming default state is that the player can move freely in all directions.
        canMoveRight = true;
        canMoveLeft = true;
        canMoveUp = true;
        canMoveDown = true;
    }
}
