using UnityEngine;

public class StockpileManager : MonoBehaviour
{
    public enum Resource
    {
        wood,
        stone,
        iron,
        food,
    }

    public bool isFull = false;
    public int capacity = 200;
    public int storage = 0;
    public int woodPile = 0;
    public int stonePile = 0;
    public int ironPile = 0;
    public int foodPile = 0;

    public void AddResource(int ammount, Resource type)
    {
        if (!isFull)
        {
            if (storage + ammount < capacity)
            {
                capacity += ammount;
                switch (type)
                {
                    case Resource.wood:
                        {
                            woodPile += ammount;
                        }
                        break;
                    case Resource.iron:
                        {
                            ironPile += ammount;
                        }
                        break;
                    case Resource.stone:
                        {
                            stonePile += ammount;
                        }
                        break;
                    case Resource.food:
                        {
                            foodPile += ammount;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
