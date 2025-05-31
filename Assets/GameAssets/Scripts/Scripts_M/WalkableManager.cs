using JetBrains.Annotations;
using UnityEngine;

//lazy singleton class
public class WalkableManager
{
    private static WalkableManager _instance;   // the instance

    private int width, height;

    private int[,] walkableMap;

    private WalkableManager(int width, int height)
    {
        this.width = width;
        this.height = height;
        walkableMap = new int[width, height];
    }
    //initialize only oance
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

    //acces to the instance
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

    public void UpdateWalkableMap(int x, int y, int isWalkable)//isWalkable shood only be 1 for not walkabel and 0 if is walkabel
    {
        walkableMap[x, y] = isWalkable;
    }

}