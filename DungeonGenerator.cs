using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    // References to tilemap and tile types
    public Tilemap tilemap;
    public Tile wallTile;
    public Tile floorTile;
    public Tile topEdgeTile;
    public Tile bottomEdgeTile;
    public Tile leftEdgeTile;
    public Tile rightEdgeTile;
    public Tile topLeftEdgeTile;
    public Tile topRightEdgeTile;
    public Tile bottomLeftEdgeTile;
    public Tile bottomRightEdgeTile;


    // Dungeon dimensions and generation parameters
    public int width;
    public int height;
    public string seed;
    public bool useRandomSeed;
    [Range(0, 100)]
    public int randomFillPercent;

    // 2D array to store the map data
    int[,] map;

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

        // Place edge tiles based on the generated map
        PlaceEdgeTiles();

        // Render the dungeon using tilemap
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    // Set the wall tile only if an edge tile hasn't been set
                    if (tilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                    }
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                }
            }
        }
    }


    void RandomFillMap()
    {
        // Initialize pseudoRandom with a seed if useRandomSeed is true
        System.Random pseudoRandom = useRandomSeed ? new System.Random(seed.GetHashCode()) : new System.Random();

        // Randomly fill the map with walls and empty space based on randomFillPercent
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
        // Apply cellular automata rules: convert cells based on neighboring walls
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
        // Count walls in the 8 surrounding cells
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
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

    void PlaceEdgeTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1) // If it's a wall tile
                {
                    // Check surrounding tiles
                    bool isTopEdge = y < height - 1 && map[x, y + 1] == 0;
                    bool isBottomEdge = y > 0 && map[x, y - 1] == 0;
                    bool isLeftEdge = x > 0 && map[x - 1, y] == 0;
                    bool isRightEdge = x < width - 1 && map[x + 1, y] == 0;

                    // Check for corners
                    bool isTopLeft = isTopEdge && isLeftEdge;
                    bool isTopRight = isTopEdge && isRightEdge;
                    bool isBottomLeft = isBottomEdge && isLeftEdge;
                    bool isBottomRight = isBottomEdge && isRightEdge;

                    // Place corner tiles
                    if (isTopLeft) tilemap.SetTile(new Vector3Int(x, y, 0), topLeftEdgeTile);
                    else if (isTopRight) tilemap.SetTile(new Vector3Int(x, y, 0), topRightEdgeTile);
                    else if (isBottomLeft) tilemap.SetTile(new Vector3Int(x, y, 0), bottomLeftEdgeTile);
                    else if (isBottomRight) tilemap.SetTile(new Vector3Int(x, y, 0), bottomRightEdgeTile);
                    // Place edge tiles
                    else if (isLeftEdge) tilemap.SetTile(new Vector3Int(x, y, 0), leftEdgeTile);
                    else if (isRightEdge) tilemap.SetTile(new Vector3Int(x, y, 0), rightEdgeTile);
                    else if (isTopEdge) tilemap.SetTile(new Vector3Int(x, y, 0), topEdgeTile);
                    else if (isBottomEdge) tilemap.SetTile(new Vector3Int(x, y, 0), bottomEdgeTile);
                    // If not an edge, place default wall tile
                    else tilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
                else // If it's a floor tile
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                }
            }
        }
    }




    // Additional methods to handle layout generation and tile placement can be added here...
}
