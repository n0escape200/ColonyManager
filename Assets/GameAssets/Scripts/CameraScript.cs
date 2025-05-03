using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraScript : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap component
    public float moveSpeed = 5f; // Speed of the camera movement
    public float maxZoom = 10f; // Maximum zoom level

    private Camera cam; // Reference to the Camera component
    private Vector3 lastMousePosition; // To track the last mouse position for dragging

    void Start()
    {
        // Get the Camera component
        cam = Camera.main;

        // Center the camera on the tilemap
        int width = tilemap.GetComponent<TerrainGeneration>().width; // Get the width from TerrainGeneration script
        int height = tilemap.GetComponent<TerrainGeneration>().height; // Get the height from TerrainGeneration script
        this.transform.position = new Vector3(width / 2f, height / 2f, -10f); // Center the camera on the tilemap
    }

    void Update()
    {
        // Get input from WASD keys
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down Arrow

        // Calculate movement direction
        Vector3 movement = new Vector3(horizontal, vertical, 0f);

        // Move the camera
        transform.position += movement * moveSpeed * Time.deltaTime;

        // Handle zoom with the scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll; // Adjust the zoom level
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 1f, maxZoom); // Clamp between 1 and maxZoom
        }

        // Handle camera dragging with right mouse button
        if (Input.GetMouseButtonDown(1)) // Right mouse button pressed
        {
            lastMousePosition = Input.mousePosition; // Store the initial mouse position
        }
        else if (Input.GetMouseButton(1)) // Right mouse button held
        {
            Vector3 delta = Input.mousePosition - lastMousePosition; // Calculate the mouse movement
            Vector3 dragMovement = new Vector3(-delta.x, -delta.y, 0f) * (cam.orthographicSize / Screen.height); // Scale movement
            transform.position += dragMovement; // Move the camera
            lastMousePosition = Input.mousePosition; // Update the last mouse position
        }
    }
}
