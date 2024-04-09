using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    // Define the zoom speed.
    public float zoomSpeed = 1.0f;
    // Define the maximum and minimum orthographic sizes to prevent too much zoom.
    public float maxZoom = 1.5f;
    public float minZoom = 2f;

    // Reference to the camera component.
    private Camera cameraComponent;

    private void Awake()
    {
        // Get the Camera component from the GameObject this script is attached to.
        cameraComponent = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for zoom in input (e.g., pressing "Z" key).
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ZoomIn();
        }
        // Check for zoom out input (e.g., pressing "X" key).
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ZoomOut();
        }
    }

    void ZoomIn()
    {
        // Decrease the orthographic size of the camera, simulating a zoom in effect, but do not exceed minZoom.
        cameraComponent.orthographicSize = Mathf.Max(cameraComponent.orthographicSize - zoomSpeed * Time.deltaTime, minZoom);
    }

    void ZoomOut()
    {
        // Increase the orthographic size of the camera, simulating a zoom out effect, but do not go below maxZoom.
        cameraComponent.orthographicSize = Mathf.Min(cameraComponent.orthographicSize + zoomSpeed * Time.deltaTime, maxZoom);
    }
}
