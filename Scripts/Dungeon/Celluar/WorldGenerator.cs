using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    // List of floor tile prefabs  public List<GameObject> floorTilePrefabs;
    public List<GameObject> floorTilePrefabs;
    public List<GameObject> housePrefabs;
    public GameObject pathTilePrefab;
    public GameObject[] edgePrefabs = new GameObject[8];
    public List<GameObject> naturalElementsPrefabs; // Prefabs for sticks, rocks, trees, etc.

    public GameObject player;

    public Vector3 tileScale = new Vector3(1, 1, 1); // Default scale for tiles

    public float prefabHeight = 1f;
    public float prefabWidth = 1f;

    // World dimensions and generation parameters
    public int width;
    public int height;
    public string seed;
    public bool useRandomSeed;
    [Range(0, 100)]
    public int randomFillPercent;

    public int maxHouses;
    public int minHouses;

    // 2D array to store the map data
    private int[,] map;
    List<Vector2> housepositions = new List<Vector2>();

    void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }

        InstantiateTiles();
        PlaceHouses();
        CreatePathsToMidpoint();
        PlaceNaturalElements();
    }

    void InstantiateTiles()
    {
        // Destroy all children to clean up old world, except for the player
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Player"))
            {
                continue;
            }
            Destroy(child.gameObject);
        }

        System.Random pseudoRandom = useRandomSeed ? new System.Random(seed.GetHashCode()) : new System.Random();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * prefabWidth, y * prefabHeight, 0);
                if (map[x, y] == 0)
                {
                    // Choose a random floor tile prefab from the list
                    int tileIndex = pseudoRandom.Next(floorTilePrefabs.Count);
                    GameObject tilePrefab = floorTilePrefabs[tileIndex];
                    GameObject instantiatedTile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                    instantiatedTile.transform.localScale = tileScale;
                }
            }
        }
    }

    void PlaceNaturalElements()
    {
        System.Random pseudoRandom = useRandomSeed ? new System.Random(seed.GetHashCode()) : new System.Random();
        int areaSize = 3;  // Defines the area as 3x3
        float chanceToPlaceElement = naturalElementsPrefabs.Count;

        for (int x = 0; x < width; x += areaSize)
        {
            for (int y = 0; y < height; y += areaSize)
            {
                bool elementPlacedInArea = false;

                // Check this area for existing elements
                for (int subX = x; subX < x + areaSize && subX < width; subX++)
                {
                    for (int subY = y; subY < y + areaSize && subY < height; subY++)
                    {
                        if (map[subX, subY] > 0) // Assuming '1' is used for walls and '2' for houses
                        {
                            elementPlacedInArea = true;
                            break;
                        }
                    }
                    if (elementPlacedInArea)
                    {
                        break;
                    }
                }

                // If no element is placed in this 3x3 area, consider placing one
                if (!elementPlacedInArea)
                {
                    for (int subX = x; subX < x + areaSize && subX < width; subX++)
                    {
                        for (int subY = y; subY < y + areaSize && subY < height; subY++)
                        {
                            if (map[subX, subY] == 0) // Check for floor tiles
                            {
                                if (pseudoRandom.Next(0, 100) < chanceToPlaceElement)
                                {
                                    int elementIndex = pseudoRandom.Next(naturalElementsPrefabs.Count);
                                    GameObject elementPrefab = naturalElementsPrefabs[elementIndex];
                                    Vector3 pos = new Vector3(subX * prefabWidth, subY * prefabHeight, 0);
                                    Instantiate(elementPrefab, pos, Quaternion.identity, transform);
                                    map[subX, subY] = 4; // Marking the tile as occupied by a natural element
                                    elementPlacedInArea = true;
                                    break; // Stop after placing one element in this area
                                }
                            }
                        }
                        if (elementPlacedInArea)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }


    void PlaceHouses()
    {
        int houseCount = 0;
        System.Random pseudoRandom = useRandomSeed ? new System.Random(seed.GetHashCode()) : new System.Random();
        List<Vector2> availablePositions = new List<Vector2>();

        // Identify potential positions for houses
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0) // Assuming 0 represents a floor where a house can be placed
                {
                    availablePositions.Add(new Vector2(x, y));
                }
            }
        }

        // Shuffle the list to randomize house placement
        for (int i = 0; i < availablePositions.Count; i++)
        {
            Vector2 temp = availablePositions[i];
            int randomIndex = pseudoRandom.Next(i, availablePositions.Count);
            availablePositions[i] = availablePositions[randomIndex];
            availablePositions[randomIndex] = temp;
        }

        // Place houses, ensuring they do not touch and are placed on floor tiles
        foreach (var position in availablePositions)
        {
            if (houseCount >= maxHouses) break; // Stop if maximum number of houses reached

            int x = (int)position.x;
            int y = (int)position.y;

            // Choose a random house prefab
            int houseIndex = pseudoRandom.Next(housePrefabs.Count);
            GameObject housePrefab = housePrefabs[houseIndex];

            if (IsPositionValidForHouse(x, y, housePrefab))
            {
                housepositions.Add(new Vector2(x, y));
                Vector3 pos = new Vector3(x * 0.16f, y * 0.16f, 0);
                Instantiate(housePrefab, pos, Quaternion.identity, transform);

                map[x, y] = 2; // Mark this tile as occupied by a house to prevent overlapping
                houseCount++;
            }
        }

        // Ensure minimum number of houses is placed
        if (houseCount < minHouses)
        {
            Debug.LogWarning("Could not place the minimum number of houses. Consider adjusting placement rules or increasing the map size.");
        }
    }

    bool IsPositionValidForHouse(int x, int y, GameObject housePrefab)
    {
        // Assuming the house prefab might occupy more than one tile, check the surrounding area
        int houseRadius = 3; // Adjust based on your house prefab size

        // Expanded check to ensure houses do not touch
        int checkRadius = houseRadius + 1; // Increase radius by one to create a gap between houses

        for (int checkX = x - checkRadius; checkX <= x + checkRadius; checkX++)
        {
            for (int checkY = y - checkRadius; checkY <= y + checkRadius; checkY++)
            {
                // Ensure the position is within map bounds
                if (!IsInMapBounds(checkX, checkY)) return false;

                // Ensure the position is a floor tile and not already occupied by another house
                if (map[checkX, checkY] != 0) return false;
            }
        }


        return true; // The house can be safely placed here
    }


    private bool HasNonFloorNeighbor(int x, int y)
    {
        for (int nx = x - 1; nx <= x + 1; nx++)
        {
            for (int ny = y - 1; ny <= y + 1; ny++)
            {
                if (nx == x && ny == y) continue; // Skip the tile itself

                if (!IsInMapBounds(nx, ny) || map[nx, ny] != 0) // Check if out of bounds or not a floor tile
                {
                    return true; // Found a non-floor neighbor
                }
            }
        }
        return false; // No non-floor neighbors found
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

    public void CreatePathBetweenHouses(List<Vector2> housePositions)
    {
        for (int i = 0; i < housePositions.Count; i++)
        {
            for (int j = i + 1; j < housePositions.Count; j++)
            {
                DrawCurvedPath(housePositions[i], housePositions[j]);
            }
        }
    }

    public void CreatePathsToMidpoint()
    {
        Vector2 midPoint = GetMidPointOfHouses(); // Ensure you have this method implemented
        foreach (var housePos in housepositions)
        {
            DrawCurvedPathToMidpoint(housePos, midPoint);
        }
    }

    void DrawCurvedPathToMidpoint(Vector2 start, Vector2 midpoint)
    {
        Vector2 direction = midpoint - start;
        float steps = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            // Adjust the curve function or use a different method if needed
            Vector2 point = Vector2.Lerp(start, midpoint, t) + new Vector2(Mathf.Sin(t * Mathf.PI * 2), Mathf.Cos(t * Mathf.PI * 2)) * 0.25f; // Adjust the multiplier for the sine wave offset as needed
            Vector2Int gridPoint = new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));

            if (IsInMapBounds(gridPoint.x, gridPoint.y))
            {
                // Instantiate the path tile prefab at this position
                Vector3 position = new Vector3(gridPoint.x * prefabWidth, gridPoint.y * prefabHeight, 0);
                Instantiate(pathTilePrefab, position, Quaternion.identity, transform);
                // mark the tile as a path in your map array
                map[gridPoint.x, gridPoint.y] = 3;
            }
        }
    }

    public Vector2 GetMidPointOfHouses()
    {
        if (housepositions.Count == 0)
        {
            Debug.LogError("No houses have been placed.");
            return Vector2.zero;
        }

        Vector2 sumOfPositions = Vector2.zero;
        foreach (var position in housepositions)
        {
            sumOfPositions += position;
        }

        return sumOfPositions / housepositions.Count;
    }

    void DrawCurvedPath(Vector2 start, Vector2 end)
    {
        Vector2 direction = end - start;
        float steps = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            // Linear interpolation plus a sine wave for offset to create curvature
            Vector2 point = Vector2.Lerp(start, end, t) + new Vector2(Mathf.Sin(t * Mathf.PI * 2), Mathf.Cos(t * Mathf.PI * 2)) * 0.5f;
            Vector2Int gridPoint = new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));

            if (IsInMapBounds(gridPoint.x, gridPoint.y) && map[gridPoint.x, gridPoint.y] != 0)
            {
                // Set the tile at gridPoint to be a path tile, assuming 3 is the value for path tiles
                map[gridPoint.x, gridPoint.y] = 3;

                // Instantiate the path tile prefab at this position
                Vector3 position = new Vector3(gridPoint.x * prefabWidth, gridPoint.y * prefabHeight, 0);
                Instantiate(pathTilePrefab, position, Quaternion.identity, transform);
            }
        }
    }

}
