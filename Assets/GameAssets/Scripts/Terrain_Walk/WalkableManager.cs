using JetBrains.Annotations;
using UnityEngine;
using Unity.Collections;

//lazy singleton class
public class WalkableManager
{
    private static WalkableManager _instance;   // the instance

    private int width, height;

    private int[,] walkableMap;
    private NativeArray<int> walkableMapArray;

    private WalkableManager(int width, int height)
    {
        this.width = width;
        this.height = height;
        walkableMap = new int[width, height];
        walkableMapArray = new NativeArray<int>(width * height, Allocator.Persistent);
    }

    //initialize only once
    //example : WalkableManager.Initialize(width, height);
    public static void Initialize(int width, int height)
    {
        if (_instance == null)
        {
            _instance = new WalkableManager(width, height);
        }
        else
        {
            //already initialized
            Debug.Log("WalkableManager is already initialized!");
        }
    }

    //access to the instance
    //examples: WalkableManager.Instance.UpdateWalkableMap(2, 3, 1);
    //          int[,] map = WalkableManager.Instance.GetWalkableMap();
    public static WalkableManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("WalkableManager not initialized. Call Initialize() first.");
            }
            return _instance;
        }
    }

    public int GetWidth()
    {
        return width;
    }
    public int Getheight()
    {
        return height;
    }

    public int[,] GetWalkableMap()
    {
        return walkableMap;
    }

    // Burst-compatible accessors
    public NativeArray<int> GetWalkableMapArray()
    {
        return walkableMapArray;
    }

    public int Width => width;
    public int Height => height;

    // Update both managed and native arrays
    public void UpdateWalkableMap(int x, int y, int isWalkable)
    {
        walkableMap[x, y] = isWalkable;
        walkableMapArray[x + y * width] = isWalkable;
    }

    // Optional: update the entire map at once (for initialization)
    public void SetWalkableMap(int[,] newMap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                walkableMap[x, y] = newMap[x, y];
                walkableMapArray[x + y * width] = newMap[x, y];
            }
        }
    }

    // Dispose NativeArray when done (call this on application quit)
    public void Dispose()
    {
        if (walkableMapArray.IsCreated)
            walkableMapArray.Dispose();
    }
}