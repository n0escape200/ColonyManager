using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraScript : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap component
    public float moveSpeed = 5f; // Speed of the camera movement
    public float maxZoom = 10f; // Maximum zoom level

    private Camera cam; // Reference to the Camera component
    private Vector3 lastMousePosition; // To track the last mouse position for dragging

    public GameObject placeableObject;
    public TerrainGeneration terrainGeneration;
    public GameManager gameManager;


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

        if (placeableObject != null)
        {
            BuildingsManager buildingsManager = placeableObject.GetComponent<BuildingsManager>();
            buildingsManager.isPlaced = false;
            // Get mouse position in screen coordinates
            Vector3 mouseScreenPos = Input.mousePosition;
            // Convert to world coordinates
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
            // Convert to cell position on the tilemap
            Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);
            // Get the center of the cell in world coordinates
            Vector3 cellCenterWorld = tilemap.GetCellCenterWorld(cellPos);
            // Set the placeable object's position (keep its original z)
            CellSize cellSize = placeableObject.GetComponent<CellSize>();
            float offsetX, offsetY;
            offsetX = cellSize.width % 2 == 0 ? 0.5f : 0;
            offsetY = cellSize.height % 2 == 0 ? 0.5f : 0;
            placeableObject.transform.position = new Vector3(cellCenterWorld.x - offsetX, cellCenterWorld.y - offsetY, -2);

            if (Input.GetMouseButtonDown(0))
            {
                if (gameManager.totalWood - buildingsManager.woodCost >= 0 &&
                    gameManager.totalStone - buildingsManager.stoneCost >= 0 &&
                    gameManager.totalIron - buildingsManager.ironCost >= 0 &&
                    gameManager.totalFood - buildingsManager.foodCost >= 0 
                ) {
                    if (buildingsManager.woodCost > 0)
                    {
                        gameManager.RemoveResource(buildingsManager.woodCost, Resource.wood);
                    }
                    if (buildingsManager.stoneCost > 0)
                    {
                        gameManager.RemoveResource(buildingsManager.stoneCost, Resource.stone);
                    }
                    if (buildingsManager.ironCost > 0)
                    {
                        gameManager.RemoveResource(buildingsManager.ironCost, Resource.iron);
                    }
                    if (buildingsManager.foodCost > 0)
                    {
                        gameManager.RemoveResource(buildingsManager.foodCost, Resource.food);
                    }
                    if (cellSize != null)
                    {
                        int objWidth = cellSize.width;
                        int objHeight = cellSize.height;

                        // Calculate bottom-left and top-right corners based on center cell
                        int halfWidth = objWidth / 2;
                        int halfHeight = objHeight / 2;

                        int x1 = cellPos.x - halfWidth;
                        int y1 = cellPos.y - halfHeight;
                        int x2 = cellPos.x + (objWidth % 2 == 0 ? halfWidth - 1 : halfWidth);
                        int y2 = cellPos.y + (objHeight % 2 == 0 ? halfHeight - 1 : halfHeight);

                        terrainGeneration.SetPlaceableArea(x1, y1, x2, y2, 1);
                    }
                    buildingsManager.isPlaced = true;
                    placeableObject = null;
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(placeableObject);
            }
        }
    }
}
