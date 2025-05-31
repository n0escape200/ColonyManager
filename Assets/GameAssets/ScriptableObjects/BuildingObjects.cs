using UnityEngine;

[CreateAssetMenu(fileName = "BuildingObjects", menuName = "Scriptable Objects/BuildingObjects")]
public class BuildingObjects : ScriptableObject
{
    public Sprite objectSprite;
    public int health;
    public int timeToBuild;
    public GameObject[] materialsToBuild;

    public string buildingName;
    public string buildingDescription;

}
