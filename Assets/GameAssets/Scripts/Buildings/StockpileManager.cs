using UnityEngine;



public enum Resource
    {
        wood,
        stone,
        iron,
        food,
    }
    
public class StockpileManager : MonoBehaviour
{

    public bool isFull = false;
    public int capacity = 200;
    public int storage = 0;
    public int woodPile = 0;
    public int stonePile = 0;
    public int ironPile = 0;
    public int foodPile = 0;

    public GameManager gameManager;

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
                            gameManager.AddResource(ammount, Resource.wood);
                        }
                        break;
                    case Resource.iron:
                        {
                            ironPile += ammount;
                            gameManager.AddResource(ammount, Resource.iron);
                        }
                        break;
                    case Resource.stone:
                        {
                            stonePile += ammount;
                            gameManager.AddResource(ammount, Resource.stone);
                        }
                        break;
                    case Resource.food:
                        {
                            foodPile += ammount;
                            gameManager.AddResource(ammount, Resource.food);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
