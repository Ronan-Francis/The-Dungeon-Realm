using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 10f;
    public Tilemap tilemap;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Camera movement
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Translate(moveX, moveY, 0);

        // Camera zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float size = cam.orthographicSize;
        size -= scroll * zoomSpeed;
        size = Mathf.Clamp(size, minZoom, maxZoom);
        cam.orthographicSize = size;

        // Handle mouse input
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            CheckTile();
        }
        else if (Input.GetMouseButtonDown(1)) // Right click
        {
            CheckTile();
        }
    }

    private void CheckTile()
    {
        Vector3 mousePos = Input.mousePosition;
        Debug.Log($"Mouse Position: {mousePos}");

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mousePos);
        Debug.Log($"Mouse World Position: {mouseWorldPos}");

        Vector3Int coordinate = tilemap.WorldToCell(mouseWorldPos);
        Debug.Log($"Cell Coordinate: {coordinate}");

        TileBase clickedTile = tilemap.GetTile(coordinate);

        if (clickedTile != null)
        {
            Debug.Log($"Clicked on tile {clickedTile.name} at position {coordinate}");
        }
        else
        {
            Debug.Log("Clicked on an empty tile.");
        }
    }

}
