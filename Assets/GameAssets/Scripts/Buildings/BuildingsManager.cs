using Unity.Mathematics;
using UnityEngine;

public class BuildingsManager : MonoBehaviour
{
    public enum JobType
    {
        lumber,
        iron,
        stone,
        food,
        stockpile,
        house,
        townhal,

    }

    public int woodCost;
    public int stoneCost;
    public int ironCost;
    public int foodCost;
    public bool isPlaced;
    public bool hasWorker;
    public int2 buildingfront;

    public JobType job;

}