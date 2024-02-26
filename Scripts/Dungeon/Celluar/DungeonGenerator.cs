using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    // References to tilemap and tile types
    public Tilemap tilemap;
    public Tilemap wallColliderTilemap; // Separate Tilemap for walls
    public Tilemap floorTilemap;
    public Tile wallTile; // General wall tile (for reference or other uses)
    public Tile floorTile;
    public Tile colliderWallTile; 
    public Tile topEdgeTile;
    public Tile bottomEdgeTile;
    public Tile leftEdgeTile;
    public Tile rightEdgeTile;
    public Tile topLeftEdgeTile;
    public Tile topRightEdgeTile;
    public Tile bottomLeftEdgeTile;
    public Tile bottomRightEdgeTile;
    public Tile innerTopLeftTile;
    public Tile innerTopRightTile;
    public Tile innerBottomLeftTile;
    public Tile innerBottomRightTile;

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

        // Render the dungeon using separate tilemaps for floor and walls
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (map[x, y] == 0)
                {
                    floorTilemap.SetTile(tilePosition, floorTile);
                }
                else
                {
                    wallColliderTilemap.SetTile(tilePosition, colliderWallTile); // Use the specificWallTile for walls
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
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180)); // Create a rotation matrix for 180 degrees

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                Tile tileToSet = null;

                if (map[x, y] == 1) // If it's a wall tile
                {
                    // Check surrounding tiles for edges
                    bool isTopEdge = y < height - 1 && map[x, y + 1] == 0;
                    bool isBottomEdge = y > 0 && map[x, y - 1] == 0;
                    bool isLeftEdge = x > 0 && map[x - 1, y] == 0;
                    bool isRightEdge = x < width - 1 && map[x + 1, y] == 0;

                    // Check for outer corners
                    bool isTopLeft = isTopEdge && isLeftEdge;
                    bool isTopRight = isTopEdge && isRightEdge;
                    bool isBottomLeft = isBottomEdge && isLeftEdge;
                    bool isBottomRight = isBottomEdge && isRightEdge;

                    // Determine the tile to set based on the edge/corner
                    if (isTopLeft) tileToSet = topLeftEdgeTile;
                    else if (isTopRight) tileToSet = topRightEdgeTile;
                    else if (isBottomLeft) tileToSet = bottomLeftEdgeTile;
                    else if (isBottomRight) tileToSet = bottomRightEdgeTile;
                    else if (isLeftEdge) tileToSet = leftEdgeTile;
                    else if (isRightEdge) tileToSet = rightEdgeTile;
                    else if (isTopEdge) tileToSet = topEdgeTile;
                    else if (isBottomEdge) tileToSet = bottomEdgeTile;
                    else tileToSet = wallTile; // Default wall tile placement

                    tilemap.SetTile(tilePosition, tileToSet);
                    // Apply rotation to edge and corner tiles
                    if (tileToSet != null && tileToSet != wallTile) // Check if the tile is an edge or corner tile and not a regular wall tile
                    {
                        tilemap.SetTransformMatrix(tilePosition, rotationMatrix);
                    }
                }
                else // If it's a floor tile
                {
                    tilemap.SetTile(tilePosition, floorTile);
                }
            }
        }
    }






    // Additional methods to handle layout generation and tile placement can be added here...
}
