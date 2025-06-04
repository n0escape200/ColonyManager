using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public int totalWood;
    public int totalStone;
    public int totalIron;
    public int totalFood;
    public TMP_Text woodText;
    public TMP_Text stoneText;
    public TMP_Text ironText;
    public TMP_Text foodText;

    public Transform buildings;

    void Start()
    {
        totalWood = 800;
        totalStone = 800;
        totalIron = 800;
        totalFood = 800;
    }

    void Update()
    {
        woodText.text = totalWood.ToString();
        stoneText.text = totalStone.ToString();
        ironText.text = totalIron.ToString();
        foodText.text = totalFood.ToString();
    }

    public void AddResource(int ammount, Resource type)
    {
        switch (type)
        {
            case Resource.wood:
                {
                    totalWood += ammount;
                }
                break;
            case Resource.stone:
                {
                    totalStone += ammount;
                }
                break;
            case Resource.iron:
                {
                    totalIron += ammount;
                }
                break;
            case Resource.food:
                {
                    totalFood += ammount;
                }
                break;
            default:
                break;
        }
    }
    public void RemoveResource(int ammount, Resource type)
    {
        switch (type)
        {
            case Resource.wood:
                {
                    totalWood -= ammount;
                    // foreach (GameObject building in buildings)
                    // {
                    //     if (ammount > 0)
                    //     {
                    //         StockpileManager stockpileManager = building.GetComponent<StockpileManager>();
                    //         stockpileManager.woodPile -= ammount;
                    //         if (stockpileManager.woodPile < 0)
                    //         {
                    //             ammount += stockpileManager.woodPile;
                    //         }
                    //         else
                    //         {
                    //             ammount = 0;
                    //         }
                    //     }
                    // }
                }
                break;
            case Resource.stone:
                {
                    totalStone -= ammount;
                    // foreach (GameObject building in buildings)
                    // {
                    //     if (ammount > 0)
                    //     {
                    //         StockpileManager stockpileManager = building.GetComponent<StockpileManager>();
                    //         stockpileManager.stonePile -= ammount;
                    //         if (stockpileManager.stonePile < 0)
                    //         {
                    //             ammount += stockpileManager.stonePile;
                    //         }
                    //         else
                    //         {
                    //             ammount = 0;
                    //         }
                    //     }
                    // }
                }
                break;
            case Resource.iron:
                {
                    totalIron -= ammount;
                    // foreach (GameObject building in buildings)
                    // {
                    //     if (ammount > 0)
                    //     {
                    //         StockpileManager stockpileManager = building.GetComponent<StockpileManager>();
                    //         stockpileManager.ironPile -= ammount;
                    //         if (stockpileManager.ironPile < 0)
                    //         {
                    //             ammount += stockpileManager.ironPile;
                    //         }
                    //         else
                    //         {
                    //             ammount = 0;
                    //         }
                    //     }
                    // }
                }
                break;
            case Resource.food:
                {
                    totalFood -= ammount;
                    // foreach (GameObject building in buildings)
                    // {
                    //     if (ammount > 0)
                    //     {
                    //         StockpileManager stockpileManager = building.GetComponent<StockpileManager>();
                    //         stockpileManager.foodPile -= ammount;
                    //         if (stockpileManager.foodPile < 0)
                    //         {
                    //             ammount += stockpileManager.foodPile;
                    //         }
                    //         else
                    //         {
                    //             ammount = 0;
                    //         }
                    //     }
                    // }
                }
                break;
            default:
                break;
        }
    }
}
