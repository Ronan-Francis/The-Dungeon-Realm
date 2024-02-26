using UnityEngine;

public class Spawner1 : MonoBehaviour
{
    public GameObject objectToSpawn; // The GameObject to spawn
    public float spawnRadius = 10f; // The radius within which the player triggers spawning
    public float spawnInterval = 2f; // Time in seconds between spawns

    private GameObject player; // Reference to the player GameObject
    private float lastSpawnTime = 0; // Time since last spawn

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player GameObject by tag
    }

    void Update()
    {
        if (player == null) return; // Do nothing if player not found

        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        // Check if the player is within the spawn radius and if the spawn interval has elapsed
        if (distanceToPlayer <= spawnRadius && Time.time - lastSpawnTime >= spawnInterval)
        {
            SpawnObject();
            lastSpawnTime = Time.time; // Reset the spawn timer
        }
    }

    void SpawnObject()
    {
        Instantiate(objectToSpawn, transform.position, Quaternion.identity); // Spawn the object at the spawner's position
    }
}
