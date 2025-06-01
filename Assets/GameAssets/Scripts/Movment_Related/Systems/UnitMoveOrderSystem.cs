using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
//test system
/*
partial struct UnitMoveOrderSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // Check if the left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            // Get the clicked position from MouseClickHandler
            Vector3 destination = MouseClickHandler.instance.GetPosition();

            // Create an EntityCommandBuffer to defer structural changes
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach ((
                RefRW<LocalTransform> localTransform,
                RefRO<MoveSpeed> moveSpeed,
                DynamicBuffer<PathPosition> pathPositionBuffer,
                RefRW<PathIndex> pathIndex,
                Entity entity)
                in SystemAPI.Query<
                    RefRW<LocalTransform>,
                    RefRO<MoveSpeed>,
                    DynamicBuffer<PathPosition>,
                    RefRW<PathIndex>>().WithEntityAccess())
            {
                // Get the current position as the start position
                float3 currentPosition = localTransform.ValueRO.Position;

                pathIndex.ValueRW.pathIndex = 0;


                // Check if the entity has PathfindingParams
                if (!state.EntityManager.HasComponent<PathfindingParams>(entity))
                {
                    // Use the EntityCommandBuffer to add PathfindingParams
                    ecb.AddComponent(entity, new PathfindingParams
                    {
                        startPosition = new int2(
                            Mathf.FloorToInt(currentPosition.x+ 0.5f),//+0.5 is to count for a small error in the movment where the unit stops a bit lower than it shood and the start position is moved to a wrong tile
                            Mathf.FloorToInt(currentPosition.y+ 0.5f)
                        ),
                        endPosition = new int2(
                            Mathf.FloorToInt(destination.x),
                            Mathf.FloorToInt(destination.y)
                        )
                    });
                }
                else
                {
                    // Use the EntityCommandBuffer to update PathfindingParams
                    ecb.SetComponent(entity, new PathfindingParams
                    {
                        startPosition = new int2(
                            Mathf.FloorToInt(currentPosition.x + 0.5f),
                            Mathf.FloorToInt(currentPosition.y+ 0.5f)
                        ),
                        endPosition = new int2(
                            Mathf.FloorToInt(destination.x),
                            Mathf.FloorToInt(destination.y)
                        )
                    });
                }
            }

            // Playback the EntityCommandBuffer to apply the changes
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
*/