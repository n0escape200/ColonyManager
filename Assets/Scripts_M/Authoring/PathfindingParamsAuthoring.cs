using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

//start and finish positon atribution
public class PathfindingParamsAuthoring : MonoBehaviour
{
    public int2 startPosition;
    public int2 endPosition;

    public class Baker : Baker<PathfindingParamsAuthoring>
    {
        public override void Bake(PathfindingParamsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PathfindingParams
            {
                startPosition = authoring.startPosition,
                endPosition = authoring.endPosition,
            });
        }
    }
}


public struct PathfindingParams : IComponentData
{
    public int2 startPosition;
    public int2 endPosition;

}
