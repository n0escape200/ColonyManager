using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Entities;
using Unity.Transforms;
using JetBrains.Annotations;




public class TerrainGeneration : MonoBehaviour
{


    public Tilemap tilemap;
    public Tile dirtTile;
    public Tile waterTile;

    public GameObject treeObj;
    public GameObject ironObj;
    public GameObject stoneObj;

    public int width = 100;
    public int height = 100;

    public float fillPercent = 0.5f;
    public float noiseScale = 10f; // Controls the scale of the Perlin noise
    public int seed = 0; // Seed for consistent terrain generation

    private int[,] placeable; // 2D array to store tile states (0 = empty, 1 = full, 2 = water)
    public GameObject highlightPrefab; // Assign a highlight tile in the Inspector
    public Transform highlightParent;

    public Transform subScene;


    private EntitiesRefrences entitiesRefrences;
    void Start()
    {

        //test code!!!!!!
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (entityManager.CreateEntityQuery(typeof(EntitiesRefrences)).CalculateEntityCount() > 0)
        {
            entitiesRefrences = entityManager.CreateEntityQuery(typeof(EntitiesRefrences))
                .GetSingleton<EntitiesRefrences>();
        }
        //end of test code!!!

        placeable = new int[width, height];
        WalkableManager.Initialize(width, height);  //initializing the class

        Random.InitState(seed);
        float offsetX = Random.Range(0f, 10000f);
        float offsetY = Random.Range(0f, 10000f);

        tilemap.ClearAllTiles();
        List<Vector3Int> positions = new List<Vector3Int>();
        List<Tile> tiles = new List<Tile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noiseValue = Mathf.PerlinNoise((x + offsetX) / noiseScale, (y + offsetY) / noiseScale);

                if (noiseValue < fillPercent)
                {
                    placeable[x, y] = 0; // Empty dirt tile
                    tiles.Add(dirtTile);
                }
                else
                {
                    placeable[x, y] = 2; // Water tile
                    tiles.Add(waterTile);
                    WalkableManager.Instance.UpdateWalkableMap(x, y, 1);    //updating the walkable grid
                }

                positions.Add(new Vector3Int(x, y, 0));
            }
        }

        tilemap.SetTiles(positions.ToArray(), tiles.ToArray());

        // Place objects in order
        PlaceTreeSprites();
        PlaceIronSprites();
        PlaceCopperSprites();
        //HighlightNonZeroCells();
    }

    public void SetPlaceableArea(int x1, int y1, int x2, int y2, int value)
    {
        int minX = Mathf.Min(x1, x2);
        int maxX = Mathf.Max(x1, x2);
        int minY = Mathf.Min(y1, y2);
        int maxY = Mathf.Max(y1, y2);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    placeable[x, y] = value;
                    WalkableManager.Instance.UpdateWalkableMap(x, y, 1);    //updating the walkable grid
                }
            }
        }
        HighlightNonZeroCells();
    }

    void HighlightNonZeroCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (placeable[x, y] == 2)
                {
                    Vector3 worldPosition = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f, 0.5f, -3f);
                    // Instantiate your highlight prefab at the cell position
                    Instantiate(highlightPrefab, worldPosition, Quaternion.identity, highlightParent);
                }
            }
        }
    }

    void PlaceTreeSprites()
    {

        GameObject treeParent = new GameObject("Trees");
        treeParent.transform.SetParent(subScene);

        float offsetX = Random.Range(0f, 10000f);
        float offsetY = Random.Range(0f, 10000f);
        float treeNoiseScale = noiseScale * 1f;
        float treeFillPercent = fillPercent * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (placeable[x, y] == 0) // Only place on empty dirt tiles
                {
                    float noiseValue = Mathf.PerlinNoise((x + offsetX) / treeNoiseScale, (y + offsetY) / treeNoiseScale);

                    if (noiseValue < treeFillPercent)
                    {
                        Vector3 worldPosition = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0, 0, -1f);

                        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                        Entity treeEntity = entityManager.Instantiate(entitiesRefrences.TreePrefabEntity);
                        entityManager.SetComponentData(treeEntity, LocalTransform.FromPosition(worldPosition));

                        // ald way of placing resources
                        //Instantiate(treeObj, worldPosition, Quaternion.identity, treeParent.transform);
                        placeable[x, y] = 1; // Mark as full
                    }
                }
            }
        }
    }

    void PlaceIronSprites()
    {
        GameObject ironParent = new GameObject("Iron");
        ironParent.transform.SetParent(subScene);

        float offsetX = Random.Range(0f, 10000f);
        float offsetY = Random.Range(0f, 10000f);
        float ironNoiseScale = noiseScale * 1f;
        float ironFillPercent = fillPercent * 0.15f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (placeable[x, y] == 0) // Only place on empty dirt tiles
                {
                    float noiseValue = Mathf.PerlinNoise((x + offsetX) / ironNoiseScale, (y + offsetY) / ironNoiseScale);

                    if (noiseValue < ironFillPercent)
                    {
                        Vector3 worldPosition = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0, 0, -1f);

                        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                        Entity treeEntity = entityManager.Instantiate(entitiesRefrences.IronPrefabEntity);
                        entityManager.SetComponentData(treeEntity, LocalTransform.FromPosition(worldPosition));

                        //Instantiate(ironObj, worldPosition, Quaternion.identity, ironParent.transform);

                        placeable[x, y] = 1; // Mark as full
                        WalkableManager.Instance.UpdateWalkableMap(x, y, 1);    //updating the walkable grid
                    }
                }
            }
        }
    }

    void PlaceCopperSprites()
    {
        GameObject stoneParent = new GameObject("Stone");
        stoneParent.transform.SetParent(subScene);

        float offsetX = Random.Range(0f, 10000f);
        float offsetY = Random.Range(0f, 10000f);
        float copperNoiseScale = noiseScale * 1f;
        float copperFillPercent = fillPercent * 0.15f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (placeable[x, y] == 0) // Only place on empty dirt tiles
                {
                    float noiseValue = Mathf.PerlinNoise((x + offsetX) / copperNoiseScale, (y + offsetY) / copperNoiseScale);

                    if (noiseValue < copperFillPercent)
                    {
                        Vector3 worldPosition = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0, 0, -1f);

                        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                        Entity treeEntity = entityManager.Instantiate(entitiesRefrences.StonePrefabEntity);
                        entityManager.SetComponentData(treeEntity, LocalTransform.FromPosition(worldPosition));

                        //Instantiate(stoneObj, worldPosition, Quaternion.identity, stoneParent.transform);
                        placeable[x, y] = 1; // Mark as full
                        WalkableManager.Instance.UpdateWalkableMap(x, y, 1);    //updating the walkable grid
                    }
                }
            }
        }
    }
}
