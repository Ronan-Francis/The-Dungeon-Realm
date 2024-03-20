using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject wallPrefab, floorPrefab, playerPrefab;
    public Vector3 prefabScale = new Vector3(1, 1, 1);
    public int width, height;
    private bool[,] visited;
    private int[,] map; // 0 for path, 1 for wall

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        map = new int[width, height];
        visited = new bool[width, height];
        InitializeMaze();
        RecursiveBacktrack(Random.Range(0, width), Random.Range(0, height));
        InstantiateMaze();
        PlacePlayer();
    }

    void InitializeMaze()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1; // Initialize all cells as walls
    }

    void RecursiveBacktrack(int x, int y)
    {
        visited[x, y] = true;
        map[x, y] = 0; // Mark cell as part of the maze (path)

        int[][] directions = { new[] { 0, 1 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { -1, 0 } };
        Shuffle(directions);

        foreach (var dir in directions)
        {
            int nx = x + dir[0] * 2, ny = y + dir[1] * 2;
            if (IsInBounds(nx, ny) && !visited[nx, ny])
            {
                map[x + dir[0], y + dir[1]] = 0; // Remove wall
                RecursiveBacktrack(nx, ny);
            }
        }
    }

    bool IsInBounds(int x, int y)
    {
        return x > 0 && x < width - 1 && y > 0 && y < height - 1;
    }

    void Shuffle<T>(T[][] array)
    {
        System.Random rng = new System.Random();
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T[] value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }

    void InstantiateMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                GameObject prefabToInstantiate = (map[x, y] == 1) ? wallPrefab : floorPrefab;
                GameObject instance = Instantiate(prefabToInstantiate, position, Quaternion.identity, transform);
                instance.transform.localScale = prefabScale;
            }
        }
    }

    void PlacePlayer()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0) // Floor space
                {
                    Vector3 startPosition = new Vector3(x, 0.5f, y);
                    Instantiate(playerPrefab, startPosition, Quaternion.identity);
                    return;
                }
            }
        }
    }
}
