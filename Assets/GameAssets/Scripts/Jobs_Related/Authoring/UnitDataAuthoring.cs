using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;



public enum JobType : byte
{
    NoJob,
    Lumberjack,
    StoneMiner,
    IronMiner,
    Farmer,
    Builder
}
public enum ResourceType : byte
{
    Nothing,
    Wood,
    Food,
    Stone,
    Iron
}
public enum WorkerStatus : byte
{
    Idle,
    Working,
    TravelingToStorage,
    TravelingToJob,
    TravelingToResouce

}


public class UnitDataAuthoring : MonoBehaviour
{
    public JobType jobType;
    public int2 jobLocation;  
    public int2 closestStorageLocation;
    public int2 closestResource; 
    public int inventoryQuantity;
    public ResourceType InventoryResourceType;
    public double taskStartTime;
    public double taskDuration;
    public WorkerStatus workerStatus;
    public bool statusUpdated; 



    public class Baker : Baker<UnitDataAuthoring>
    {
        public override void Bake(UnitDataAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitData
            {
                jobType = authoring.jobType,
                jobLocation = authoring.jobLocation,
                closestStorageLocation = authoring.closestStorageLocation,
                closestResource = authoring.closestResource,
                inventoryQuantity = authoring.inventoryQuantity,
                InventoryResourceType = authoring.InventoryResourceType,
                taskStartTime = authoring.taskStartTime,
                taskDuration = authoring.taskDuration,
                workerStatus = authoring.workerStatus,
                statusUpdated = authoring.statusUpdated
            });
        }
    }
}


public struct UnitData : IComponentData
{
    //if jobType.state == JobType.NoJob then the others will be ignored
    public JobType jobType;         // 0 - no job , 1 - lumberjack, 2 - stone miner , 3 - iron miner, 4 - farmer , 5 - builder(if time allows), 6 - transporter(if time allows),7 tools crafter(if time allows makes jobs *2 more fast but you need one per 5 workers) )
    public int2 jobLocation;    // does not mater if job type = 0
    public int2 closestStorageLocation;    // does not mater if job type = 0
    public int2 closestResource;     //needed for logick
    public int inventoryQuantity;   //( 0 resources  10 max of one type)maybe if time allows 10 for food, 5 for iron because of weight
    public ResourceType InventoryResourceType;  //enum (Wood, Food,Stone, Iron)
    public double taskStartTime;            //the moment in time when the unit started working on its task
    public double taskDuration;         //depending on the job the task will take longer
    public WorkerStatus workerStatus;   //enum:  Idle, Working, TravelingToStorage ,TravelingToJob ,TravelingToResouce
    public bool statusUpdated;         // FALSE if the status was changed but the actions needed were not done / TRUE if the needed actions were made
}