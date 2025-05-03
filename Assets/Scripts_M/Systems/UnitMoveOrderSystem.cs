using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/*
[UpdateBefore(typeof(PathFinding))]
partial struct UnitMoveOrderSystem : ISystem
{


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRW<PathfindingParams> pathfindingParams)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<PathfindingParams>>())
        {
            pathfindingParams.ValueRW.startPosition = new int2(0, 0);
            pathfindingParams.ValueRW.endPosition = new int2(9, 0);
            
        }


    }


}
*/