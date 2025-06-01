using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


partial struct UnitMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRO<MoveSpeed> moveSpeed,
            DynamicBuffer<PathPosition> pathPositionBuffer,
            RefRW<PathIndex> pathIndex)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRO<MoveSpeed>,
                DynamicBuffer<PathPosition>,
                RefRW<PathIndex>>())
        {
            if (pathPositionBuffer.Length == 0 || pathIndex.ValueRW.pathIndex >= pathPositionBuffer.Length)
                continue;


            float3 targetPosition = new float3(pathPositionBuffer[pathIndex.ValueRW.pathIndex].position.x, pathPositionBuffer[pathIndex.ValueRW.pathIndex].position.y, 0);
            float3 directionToTarget = targetPosition - localTransform.ValueRO.Position;
            float distanceToTarget = math.length(directionToTarget);

            float stoppingDistance = 0.1f;

            if (distanceToTarget > stoppingDistance)
            {
                float3 moveDirection = math.normalize(directionToTarget);
                float moveStep = moveSpeed.ValueRO.value * SystemAPI.Time.DeltaTime;

                if (moveStep > distanceToTarget)
                    moveStep = distanceToTarget;
                localTransform.ValueRW.Position += moveDirection * moveStep;

            }
            else
            {
                pathIndex.ValueRW.pathIndex += 1;
            }

        }

    }

}
