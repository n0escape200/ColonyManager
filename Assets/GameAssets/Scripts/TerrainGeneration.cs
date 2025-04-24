using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TerrainGeneration : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile dirtTile;
    public Tile waterTile;

    public Sprite ironSprite; // Sprite for iron deposits

    public int width = 100;
    public int height = 100;

    public float fillPercent = 0.5f;
    public float noiseScale = 10f; // Controls the scale of the Perlin noise
    public int seed = 0; // Seed for consistent terrain generation

    private TileType[,] tileTypes; // 2D array to store tile types

    public enum TileType
    {
        Water = 0,
        Dirt = 1
    }

    void Start()
    {
        tileTypes = new TileType[width, height];

        Random.InitState(seed); // Initialize Unity's random generator with the seed
        float offsetX = Random.Range(0f, 10000f); // Random offset for Perlin noise X
        float offsetY = Random.Range(0f, 10000f); // Random offset for Perlin noise Y

        tilemap.ClearAllTiles(); // Clear existing tiles
        List<Vector3Int> positions = new List<Vector3Int>();
        List<Tile> tiles = new List<Tile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noiseValue = Mathf.PerlinNoise((x + offsetX) / noiseScale, (y + offsetY) / noiseScale);

                if (noiseValue < fillPercent)
                {
                    tileTypes[x, y] = TileType.Dirt; // Store Dirt in the array
                    tiles.Add(dirtTile);
                }
                else
                {
                    tileTypes[x, y] = TileType.Water; // Store Water in the array
                    tiles.Add(waterTile);
                }

                positions.Add(new Vector3Int(x, y, 0));
            }
        }

        tilemap.SetTiles(positions.ToArray(), tiles.ToArray());

        // Place iron sprites after generating the map
        PlaceIronSprites();
    }

    void PlaceIronSprites()
    {
        // Create an empty GameObject to hold all iron sprites
        GameObject ironParent = new GameObject("Iron");

        // Use a new Perlin noise map for iron placement
        float offsetX = Random.Range(0f, 10000f); // Random offset for Perlin noise X
        float offsetY = Random.Range(0f, 10000f); // Random offset for Perlin noise Y
        float ironNoiseScale = noiseScale * 2f; // Increase noise scale to make patches smaller
        float ironFillPercent = fillPercent * 0.2f; // Lower fill percent for more scarcity

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Only place iron on dirt tiles
                if (tileTypes[x, y] == TileType.Dirt)
                {
                    float noiseValue = Mathf.PerlinNoise((x + offsetX) / ironNoiseScale, (y + offsetY) / ironNoiseScale);

                    if (noiseValue < ironFillPercent)
                    {
                        // Instantiate the iron sprite at the corresponding position
                        Vector3 worldPosition = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f, 0.5f, -1f);
                        GameObject iron = new GameObject("IronSprite");
                        SpriteRenderer renderer = iron.AddComponent<SpriteRenderer>();
                        renderer.sprite = ironSprite;
                        iron.transform.position = worldPosition;

                        // Attach the iron sprite to the parent "Iron" GameObject
                        iron.transform.parent = ironParent.transform;
                    }
                }
            }
        }
    }

    public TileType GetTileType(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return tileTypes[x, y];
        }
        return TileType.Water; // Default to Water if the position is out of bounds
    }
}
