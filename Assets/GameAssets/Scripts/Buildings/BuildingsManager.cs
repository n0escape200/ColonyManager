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

    public JobType job;

}