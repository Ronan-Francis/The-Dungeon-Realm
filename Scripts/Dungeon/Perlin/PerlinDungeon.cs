using UnityEngine;

public class PerlinDungeon : MonoBehaviour
{
    // Prefabs for walls, floors, and spawners
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject spawnerPrefab; // Spawner prefab
    public GameObject player;

    public Vector3 wallScale = new Vector3(1, 1, 1); // Default scale for walls
    public Vector3 floorScale = new Vector3(1, 1, 1); // Default scale for floors

    public float prefabHeight = 1f;
    public float prefabWidth = 1f;

    // Dungeon dimensions and generation parameters
    public int width;
    public int height;
    public string seed;
    public bool useRandomSeed;
    [Range(0, 100)]
    public int randomFillPercent;

    // Perlin Noise parameters
    public float noiseScale = 0.1f;
    public Vector2 noiseOffset = new Vector2(100, 100);

    // Spawner configuration
    public int numberOfSpawners; // Number of spawners to place

    // 2D array to store the map data
    private int[,] map;

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        map = new int[width, height];
        GenerateNoiseMap();
        PlaceWallsAlongEdges();
        InstantiatePrefabs();
        PlaceSpawners();
    }

    void GenerateNoiseMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = noiseOffset.x + x * noiseScale;
                float yCoord = noiseOffset.y + y * noiseScale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                map[x, y] = sample > 0.5 ? 1 : 0; // Adjust threshold as needed
            }
        }
    }

    void InstantiatePrefabs()
    {
        // Destroy all children to clean up old dungeon, except for the player
        foreach (Transform child in transform)
        {
            if (!child.CompareTag("Player"))
            {
                Destroy(child.gameObject);
            }
        }

        Vector3 closestFloorPosition = Vector3.zero;
        float minDistance = float.MaxValue;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * prefabWidth, y * prefabHeight, 0); // Note: Adjusted for Y-axis height
                GameObject instantiatedPrefab;

                if (map[x, y] == 0)
                {
                    float distance = Vector3.Distance(position, player.transform.position);
                    if (distance < minDistance)
                    {
                        closestFloorPosition = position;
                        minDistance = distance;
                    }
                    instantiatedPrefab = Instantiate(floorPrefab, position, Quaternion.identity, transform);
                    instantiatedPrefab.transform.localScale = floorScale;
                }
                else
                {
                    instantiatedPrefab = Instantiate(wallPrefab, position, Quaternion.identity, transform);
                    instantiatedPrefab.transform.localScale = wallScale;
                }
            }
        }

        if (player != null && closestFloorPosition != Vector3.zero)
        {
            player.transform.position = closestFloorPosition;
        }
    }

    void PlaceSpawners()
    {
        int spawnersPlaced = 0;
        while (spawnersPlaced < numberOfSpawners)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            if (map[x, y] == 0 && IsInMapBounds(x, y))
            {
                Vector3 spawnerPosition = new Vector3(x * prefabWidth, 0, y * prefabHeight); // Note: Adjusted for Y-axis height
                Instantiate(spawnerPrefab, spawnerPosition, Quaternion.identity, transform);
                spawnersPlaced++;
            }
        }
    }

    bool IsInMapBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void PlaceWallsAlongEdges()
    {
        for (int x = 0; x < width; x++)
        {
            map[x, 0] = 1; // Bottom edge
            map[x, height - 1] = 1; // Top edge
        }

        for (int y = 0; y < height; y++)
        {
            map[0, y] = 1; // Left edge
            map[width - 1, y] = 1; // Right edge
        }
    }

}
