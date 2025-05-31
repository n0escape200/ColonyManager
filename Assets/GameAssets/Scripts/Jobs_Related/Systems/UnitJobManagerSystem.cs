using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Runtime.CompilerServices;


partial struct UnitJobManagerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Create an EntityCommandBuffer to defer structural changes
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRW<UnitData> unitData,
            RefRW<PathIndex> pathIndex,
            Entity entity)
            in SystemAPI.Query<
            RefRW<LocalTransform>,
            RefRW<UnitData>,
            RefRW<PathIndex>>().WithEntityAccess()
        )
        {
            float3 currentPosition = localTransform.ValueRO.Position;
            int2 destination;
            switch (unitData.ValueRO.jobType)
            {
                case JobType.NoJob:
                    //logic move a bit from time to time
                    break;
                case JobType.Lumberjack:
                    //logic:

                    //go to work building
                    //go to cloasest resource offser by one
                    //work
                    //finis job and get the resource
                    //deliver it to the storrage
                    //repeat
                    switch (unitData.ValueRW.workerStatus)
                    {
                        case WorkerStatus.Idle:
                            unitData.ValueRW.workerStatus = WorkerStatus.TravelingToJob;
                            unitData.ValueRW.statusUpdated = false;
                            break;
                        case WorkerStatus.TravelingToJob:
                            if (unitData.ValueRW.statusUpdated == false)
                            {
                                destination = unitData.ValueRO.jobLocation;
                                HandlePathfindingParams(ref ecb, state.EntityManager, entity, currentPosition, destination);
                                unitData.ValueRW.statusUpdated = true;
                            }
                            //needed conversion
                            int2 currentPositionInINT2 = new int2(
                                Mathf.FloorToInt(currentPosition.x + 0.5f), // Adjust for movement error
                                Mathf.FloorToInt(currentPosition.y + 0.5f)
                            );
                            if (currentPositionInINT2.Equals(unitData.ValueRO.jobLocation))
                            {
                                unitData.ValueRW.workerStatus = WorkerStatus.TravelingToResouce;
                                unitData.ValueRW.statusUpdated = false;
                            }
                            break;
                        case WorkerStatus.TravelingToResouce:
                            //......
                            break;
                        case WorkerStatus.Working:
                            //......
                            break;
                        case WorkerStatus.TravelingToStorage:
                            //......
                            break;

                    }




                    break;
                case JobType.StoneMiner:
                    //logic
                    break;
                case JobType.IronMiner:
                    //logic
                    break;
                case JobType.Farmer:
                    //logic
                    break;
                case JobType.Builder:
                    //No logic yet
                    break;
            }


        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }


    // set index 0 before doing it
    // function for atributing a new path
    [BurstCompile]
    private static void HandlePathfindingParams(
            ref EntityCommandBuffer ecb,
            in EntityManager entityManager,
            in Entity entity,
            in float3 currentPosition,
            in int2 destination)
    {
        if (!entityManager.HasComponent<PathfindingParams>(entity))
        {
            // Add PathfindingParams if it doesn't exist
            ecb.AddComponent(entity, new PathfindingParams
            {
                startPosition = new int2(
                    Mathf.FloorToInt(currentPosition.x + 0.5f), // Adjust for movement error
                    Mathf.FloorToInt(currentPosition.y + 0.5f)
                ),
                endPosition = destination
            });
        }
        else
        {
            // Update PathfindingParams if it already exists
            ecb.SetComponent(entity, new PathfindingParams
            {
                startPosition = new int2(
                    Mathf.FloorToInt(currentPosition.x + 0.5f),
                    Mathf.FloorToInt(currentPosition.y + 0.5f)
                ),
                endPosition = destination
            });
        }
    }

}




