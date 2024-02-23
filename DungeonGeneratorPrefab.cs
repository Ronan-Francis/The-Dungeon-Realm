using UnityEngine;

public class DungeonGeneratorPrefab : MonoBehaviour
{
    // Prefabs for walls and floors
    public GameObject wallPrefab;
    public GameObject floorPrefab;
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

    // 2D array to store the map data
    private int[,] map;

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }

        InstantiatePrefabs();
    }

    void InstantiatePrefabs()
    {
        float distance;
        Vector3 closestFloorPosition1 = Vector3.zero;
        Vector3 closestFloorPosition2 = Vector3.zero;
        float minDistance = float.MaxValue;

        // Destroy all children to clean up old dungeon, except for the player
        foreach (Transform child in transform)
        {
            // Check if the child object is tagged as "Player"
            if (child.CompareTag("Player"))
            {
                // Skip this child and don't destroy it because it's the player
                continue;
            }
            else
            {
                // If it's not the player, destroy the child object
                Destroy(child.gameObject);
            }
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * prefabWidth, y * prefabHeight, 0); // Instantiate along the Y axis
                GameObject instantiatedPrefab;
                if (map[x, y] == 0)
                {
                    distance = Vector3.Distance(position, Vector3.zero);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        if (IsTouchingFloor(x, y)) {
                            closestFloorPosition1 = position;
                        }
                    }
                    instantiatedPrefab = Instantiate(floorPrefab, position, Quaternion.identity, transform);
                    instantiatedPrefab.transform.localScale = floorScale; // Apply floor scale
                }
                else
                {
                    instantiatedPrefab = Instantiate(wallPrefab, position, Quaternion.identity, transform);
                    instantiatedPrefab.transform.localScale = wallScale; // Apply wall scale
                }
            }
        }

        // Move player to the closest floor tile found
        if (player != null && closestFloorPosition1 != Vector3.zero)
        {
            player.transform.position = closestFloorPosition1;
        }
    }

    bool IsTouchingFloor(int x, int y)
    {
        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
                if (IsInMapBounds(neighbourX, neighbourY) && (neighbourX != x || neighbourY != y))
                {
                    if (map[neighbourX, neighbourY] == 0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }



    void RandomFillMap()
    {
        System.Random pseudoRandom = useRandomSeed ? new System.Random(seed.GetHashCode()) : new System.Random();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (IsInMapBounds(neighbourX, neighbourY))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    bool IsInMapBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
