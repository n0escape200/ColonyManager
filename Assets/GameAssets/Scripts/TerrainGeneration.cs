using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TerrainGeneration : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile dirtTile;
    public Tile waterTile;

    public Sprite ironSprite; // Sprite for iron deposits
    public Sprite treeSprite; // Sprite for trees
    public Sprite copperSprite; // Sprite for copper deposits

    public int width = 100;
    public int height = 100;

    public float fillPercent = 0.5f;
    public float noiseScale = 10f; // Controls the scale of the Perlin noise
    public int seed = 0; // Seed for consistent terrain generation

    private int[,] placeable; // 2D array to store tile states (0 = empty, 1 = full, 2 = water)




    void Start()
    {
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
    }

    void PlaceTreeSprites()
    {
        GameObject treeParent = new GameObject("Trees");

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
                        Vector3 worldPosition = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f, 0.5f, -1f);
                        GameObject tree = new GameObject("TreeSprite");
                        SpriteRenderer renderer = tree.AddComponent<SpriteRenderer>();
                        renderer.sprite = treeSprite;
                        tree.transform.position = worldPosition;

                        tree.transform.parent = treeParent.transform;

                        placeable[x, y] = 1; // Mark as full
                    }
                }
            }
        }
    }

    void PlaceIronSprites()
    {
        GameObject ironParent = new GameObject("Iron");

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
                        Vector3 worldPosition = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f, 0.5f, -1f);
                        GameObject iron = new GameObject("IronSprite");
                        SpriteRenderer renderer = iron.AddComponent<SpriteRenderer>();
                        renderer.sprite = ironSprite;
                        iron.transform.position = worldPosition;

                        iron.transform.parent = ironParent.transform;

                        placeable[x, y] = 1; // Mark as full
                        WalkableManager.Instance.UpdateWalkableMap(x, y, 1);    //updating the walkable grid
                    }
                }
            }
        }
    }

    void PlaceCopperSprites()
    {
        GameObject copperParent = new GameObject("Copper");

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
                        Vector3 worldPosition = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f, 0.5f, -1f);
                        GameObject copper = new GameObject("CopperSprite");
                        SpriteRenderer renderer = copper.AddComponent<SpriteRenderer>();
                        renderer.sprite = copperSprite;
                        copper.transform.position = worldPosition;

                        copper.transform.parent = copperParent.transform;

                        placeable[x, y] = 1; // Mark as full
                        WalkableManager.Instance.UpdateWalkableMap(x, y, 1);    //updating the walkable grid

                    }
                }
            }
        }
    }
}
