using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


//buffer to store the path
public class PathPositionAuthoring : MonoBehaviour
{
    public List<int2> positions = new();

    public class Baker : Baker<PathPositionAuthoring>
    {
        public override void Bake(PathPositionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            var buffer = AddBuffer<PathPosition>(entity);

            foreach (var pos in authoring.positions)
            {
                buffer.Add(new PathPosition { position = pos });
            }
        }
    }
}


[InternalBufferCapacity(20)]
public struct PathPosition : IBufferElementData
{
    public int2 position;
}
