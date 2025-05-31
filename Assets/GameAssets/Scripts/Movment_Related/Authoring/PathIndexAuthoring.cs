using UnityEngine;
using Unity.Entities;

//index for going through the buffer of positions
public class PathIndexAuthoring : MonoBehaviour
{
    //this shood probably be private!!
    public int pathIndex;

    public class Baker : Baker<PathIndexAuthoring>
    {
        public override void Bake(PathIndexAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PathIndex
            {
                pathIndex = authoring.pathIndex,
            });
        }
    }
}


public struct PathIndex : IComponentData
{
    public int pathIndex;

}