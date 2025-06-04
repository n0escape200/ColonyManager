using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Runtime.CompilerServices;
using Unity.Collections;
using System;


partial struct UnitJobManagerSystem : ISystem
{
    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Create an EntityCommandBuffer to defer structural changes
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        float deltaTime = SystemAPI.Time.DeltaTime;

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
            int2 destination = new int2(-1, -1);
            int2 currentPositionInINT2 = new int2(
                                Mathf.FloorToInt(currentPosition.x + 0.5f),
                                Mathf.FloorToInt(currentPosition.y + 0.5f)
                            );
            switch (unitData.ValueRO.jobType)
            {
                case JobType.NoJob:
                    //logic move a bit from time to time
                    break;

                case JobType.Lumberjack:
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
                                pathIndex.ValueRW.pathIndex = 0;
                                HandlePathfindingParams(ref ecb, state.EntityManager, entity, currentPosition, destination);
                                unitData.ValueRW.statusUpdated = true;
                            }
                            //needed conversion

                            if (currentPositionInINT2.Equals(unitData.ValueRO.jobLocation))
                            {
                                unitData.ValueRW.workerStatus = WorkerStatus.TravelingToResouce;
                                unitData.ValueRW.statusUpdated = false;
                            }
                            break;

                        case WorkerStatus.TravelingToResouce:

                            if (unitData.ValueRW.statusUpdated == false)
                            {
                                ResourceType resourceTypeRequest = ResourceType.Wood; // Replace with the desired resource type
                                unitData.ValueRW.closestResource = FindClosestResource(ref state, currentPosition, resourceTypeRequest);

                                pathIndex.ValueRW.pathIndex = 0;
                                HandlePathfindingParams(ref ecb, state.EntityManager, entity, currentPosition, unitData.ValueRW.closestResource);
                                unitData.ValueRW.statusUpdated = true;
                            }
                            if (currentPositionInINT2.Equals(unitData.ValueRO.closestResource))
                            {
                                unitData.ValueRW.workerStatus = WorkerStatus.Working;
                                unitData.ValueRW.statusUpdated = false;
                            }
                            break;


                        case WorkerStatus.Working:
                            if (unitData.ValueRW.statusUpdated == false)
                            {
                                unitData.ValueRW.taskStartTime = 0;
                                unitData.ValueRW.statusUpdated = true;

                            }
                            unitData.ValueRW.taskStartTime += deltaTime;
                            if (unitData.ValueRO.taskStartTime >= unitData.ValueRO.taskDuration)
                            {
                                DecreseResourceAmount(ref state, unitData.ValueRO.closestResource, ref ecb, ResourceType.Wood);
                                unitData.ValueRW.inventoryQuantity = 10;
                                unitData.ValueRW.InventoryResourceType = ResourceType.Wood;
                                unitData.ValueRW.statusUpdated = false;
                                unitData.ValueRW.workerStatus = WorkerStatus.TravelingToStorage;
                            }
                            break;
                        case WorkerStatus.TravelingToStorage:
                            if (unitData.ValueRW.statusUpdated == false)
                            {
                                destination = unitData.ValueRO.closestStorageLocation;
                                pathIndex.ValueRW.pathIndex = 0;
                                HandlePathfindingParams(ref ecb, state.EntityManager, entity, currentPosition, destination);
                                unitData.ValueRW.statusUpdated = true;
                            }
                            //needed conversion

                            if (currentPositionInINT2.Equals(unitData.ValueRO.closestStorageLocation))
                            {
                                unitData.ValueRW.inventoryQuantity = 0;
                                unitData.ValueRW.InventoryResourceType = ResourceType.Nothing;

                                StockpileManager stockpile = StockpileManager.Instance;
                                if (stockpile != null)
                                {
                                    stockpile.AddResourcee(10, Resource.wood);
                                }
                                unitData.ValueRW.workerStatus = WorkerStatus.TravelingToJob;
                                unitData.ValueRW.statusUpdated = false;
                            }
                            break;
                    }
                    break;

                case JobType.StoneMiner:
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
                                pathIndex.ValueRW.pathIndex = 0;
                                HandlePathfindingParams(ref ecb, state.EntityManager, entity, currentPosition, destination);
                                unitData.ValueRW.statusUpdated = true;
                            }
                            //needed conversion

                            if (currentPositionInINT2.Equals(unitData.ValueRO.jobLocation))
                            {
                                unitData.ValueRW.workerStatus = WorkerStatus.TravelingToResouce;
                                unitData.ValueRW.statusUpdated = false;
                            }
                            break;

                        case WorkerStatus.TravelingToResouce:

                            if (unitData.ValueRW.statusUpdated == false)
                            {
                                ResourceType resourceTypeRequest = ResourceType.Stone; // Replace with the desired resource type
                                unitData.ValueRW.closestResource = FindClosestResource(ref state, currentPosition, resourceTypeRequest);

                                pathIndex.ValueRW.pathIndex = 0;
                                HandlePathfindingParams(ref ecb, state.EntityManager, entity, currentPosition, unitData.ValueRW.closestResource);
                                unitData.ValueRW.statusUpdated = true;
                            }
                            if (currentPositionInINT2.Equals(unitData.ValueRO.closestResource))
                            {
                                unitData.ValueRW.workerStatus = WorkerStatus.Working;
                                unitData.ValueRW.statusUpdated = false;
                            }
                            break;


                        case WorkerStatus.Working:
                            if (unitData.ValueRW.statusUpdated == false)
                            {
                                unitData.ValueRW.taskStartTime = 0;
                                unitData.ValueRW.statusUpdated = true;

                            }
                            unitData.ValueRW.taskStartTime += deltaTime;
                            if (unitData.ValueRO.taskStartTime >= unitData.ValueRO.taskDuration)
                            {
                                DecreseResourceAmount(ref state, unitData.ValueRO.closestResource, ref ecb, ResourceType.Stone);
                                unitData.ValueRW.inventoryQuantity = 10;
                                unitData.ValueRW.InventoryResourceType = ResourceType.Stone;
                                unitData.ValueRW.statusUpdated = false;
                                unitData.ValueRW.workerStatus = WorkerStatus.TravelingToStorage;
                            }
                            break;
                        case WorkerStatus.TravelingToStorage:
                            if (unitData.ValueRW.statusUpdated == false)
                            {
                                destination = unitData.ValueRO.closestStorageLocation;
                                pathIndex.ValueRW.pathIndex = 0;
                                HandlePathfindingParams(ref ecb, state.EntityManager, entity, currentPosition, destination);
                                unitData.ValueRW.statusUpdated = true;
                            }
                            //needed conversion

                            if (currentPositionInINT2.Equals(unitData.ValueRO.closestStorageLocation))
                            {
                                unitData.ValueRW.inventoryQuantity = 0;
                                unitData.ValueRW.InventoryResourceType = ResourceType.Nothing;

                                StockpileManager stockpile = StockpileManager.Instance;
                                if (stockpile != null)
                                {
                                    stockpile.AddResourcee(10, Resource.stone);
                                }
                                unitData.ValueRW.workerStatus = WorkerStatus.TravelingToJob;
                                unitData.ValueRW.statusUpdated = false;
                            }
                            break;
                    }
                    break;

                case JobType.IronMiner:
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
                                pathIndex.ValueRW.pathIndex = 0;
                                HandlePathfindingParams(ref ecb, state.EntityManager, entity, currentPosition, destination);
                                unitData.ValueRW.statusUpdated = true;
                            }
                            //needed conversion

                            if (currentPositionInINT2.Equals(unitData.ValueRO.jobLocation))
                            {
                                unitData.ValueRW.workerStatus = WorkerStatus.TravelingToResouce;
                                unitData.ValueRW.statusUpdated = false;
                            }
                            break;

                        case WorkerStatus.TravelingToResouce:

                            if (unitData.ValueRW.statusUpdated == false)
                            {
                                ResourceType resourceTypeRequest = ResourceType.Iron; // Replace with the desired resource type
                                unitData.ValueRW.closestResource = FindClosestResource(ref state, currentPosition, resourceTypeRequest);

                                pathIndex.ValueRW.pathIndex = 0;
                                HandlePathfindingParams(ref ecb, state.EntityManager, entity, currentPosition, unitData.ValueRW.closestResource);
                                unitData.ValueRW.statusUpdated = true;
                            }
                            if (currentPositionInINT2.Equals(unitData.ValueRO.closestResource))
                            {
                                unitData.ValueRW.workerStatus = WorkerStatus.Working;
                                unitData.ValueRW.statusUpdated = false;
                            }
                            break;


                        case WorkerStatus.Working:
                            if (unitData.ValueRW.statusUpdated == false)
                            {
                                unitData.ValueRW.taskStartTime = 0;
                                unitData.ValueRW.statusUpdated = true;

                            }
                            unitData.ValueRW.taskStartTime += deltaTime;
                            if (unitData.ValueRO.taskStartTime >= unitData.ValueRO.taskDuration)
                            {
                                DecreseResourceAmount(ref state, unitData.ValueRO.closestResource, ref ecb, ResourceType.Iron);
                                unitData.ValueRW.inventoryQuantity = 10;
                                unitData.ValueRW.InventoryResourceType = ResourceType.Iron;
                                unitData.ValueRW.statusUpdated = false;
                                unitData.ValueRW.workerStatus = WorkerStatus.TravelingToStorage;
                            }
                            break;
                        case WorkerStatus.TravelingToStorage:
                            if (unitData.ValueRW.statusUpdated == false)
                            {
                                destination = unitData.ValueRO.closestStorageLocation;
                                pathIndex.ValueRW.pathIndex = 0;
                                HandlePathfindingParams(ref ecb, state.EntityManager, entity, currentPosition, destination);
                                unitData.ValueRW.statusUpdated = true;
                            }
                            //needed conversion

                            if (currentPositionInINT2.Equals(unitData.ValueRO.closestStorageLocation))
                            {
                                unitData.ValueRW.inventoryQuantity = 0;
                                unitData.ValueRW.InventoryResourceType = ResourceType.Nothing;

                                StockpileManager stockpile = StockpileManager.Instance;
                                if (stockpile != null)
                                {
                                    stockpile.AddResourcee(10, Resource.iron);
                                }
                                unitData.ValueRW.workerStatus = WorkerStatus.TravelingToJob;
                                unitData.ValueRW.statusUpdated = false;
                            }
                            break;
                    }
                    break;

                case JobType.Farmer:
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
                                pathIndex.ValueRW.pathIndex = 0;
                                HandlePathfindingParams(ref ecb, state.EntityManager, entity, currentPosition, destination);
                                unitData.ValueRW.statusUpdated = true;
                            }
                            //needed conversion

                            if (currentPositionInINT2.Equals(unitData.ValueRO.jobLocation))
                            {
                                unitData.ValueRW.workerStatus = WorkerStatus.Working;
                                unitData.ValueRW.statusUpdated = false;
                            }
                            break;

                        case WorkerStatus.Working:
                            if (unitData.ValueRW.statusUpdated == false)
                            {
                                unitData.ValueRW.taskStartTime = 0;
                                unitData.ValueRW.statusUpdated = true;

                            }
                            unitData.ValueRW.taskStartTime += deltaTime;
                            if (unitData.ValueRO.taskStartTime >= unitData.ValueRO.taskDuration)
                            {
                                unitData.ValueRW.inventoryQuantity = 10;
                                unitData.ValueRW.InventoryResourceType = ResourceType.Food;
                                unitData.ValueRW.statusUpdated = false;
                                unitData.ValueRW.workerStatus = WorkerStatus.TravelingToStorage;
                            }
                            break;
                        case WorkerStatus.TravelingToStorage:
                            if (unitData.ValueRW.statusUpdated == false)
                            {
                                destination = unitData.ValueRO.closestStorageLocation;
                                pathIndex.ValueRW.pathIndex = 0;
                                HandlePathfindingParams(ref ecb, state.EntityManager, entity, currentPosition, destination);
                                unitData.ValueRW.statusUpdated = true;
                            }
                            //needed conversion

                            if (currentPositionInINT2.Equals(unitData.ValueRO.closestStorageLocation))
                            {
                                unitData.ValueRW.inventoryQuantity = 0;
                                unitData.ValueRW.InventoryResourceType = ResourceType.Nothing;

                                StockpileManager stockpile = StockpileManager.Instance;
                                if (stockpile != null)
                                {
                                    stockpile.AddResourcee(10, Resource.food);
                                }
                                unitData.ValueRW.workerStatus = WorkerStatus.TravelingToJob;
                                unitData.ValueRW.statusUpdated = false;
                            }
                            break;
                    }
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
    //[BurstCompile]
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




    //[BurstCompile]
    private int2 FindClosestResource(ref SystemState state, float3 unitPosition, ResourceType resourceTypeRequested)
    {
        int2 closestPosition = new int2(0, 0);
        float closestDistance = float.MaxValue;

        int2 unitPosInt = new int2(Mathf.FloorToInt(unitPosition.x + 0.5f), Mathf.FloorToInt(unitPosition.y + 0.5f));


        foreach ((
                    RefRW<LocalTransform> localTransformResource,
                    RefRW<ResourceData> resourceData
                    )
                    in SystemAPI.Query<
                    RefRW<LocalTransform>,
                    RefRW<ResourceData>>())
        {
            float3 ResourcePosition = localTransformResource.ValueRO.Position;
            int2 resourcePos = new int2(Mathf.FloorToInt(ResourcePosition.x + 0.5f), Mathf.FloorToInt(ResourcePosition.y + 0.5f));

            int2 adjTile;
            FindAdjacentWalkableTile(resourcePos, unitPosInt, out adjTile);
            float distance = math.distance(unitPosInt, adjTile);
            if (distance < closestDistance && resourceData.ValueRO.resourceType == resourceTypeRequested)
            {
                closestDistance = distance;
                closestPosition = adjTile;
            }
        }

        return closestPosition;
    }

    private static void FindAdjacentWalkableTile(
        in int2 resourcePos,
        in int2 unitPos,
        out int2 bestTile)
    {
        // Get walkable map data from WalkableManager
        var walkableManager = WalkableManager.Instance;
        NativeArray<int> walkableMapArray = walkableManager.GetWalkableMapArray();
        int width = walkableManager.Width;
        int height = walkableManager.Height;
        // Directions: up, down, left, right
        int2 dir0 = new int2(0, 1);
        int2 dir1 = new int2(0, -1);
        int2 dir2 = new int2(-1, 0);
        int2 dir3 = new int2(1, 0);

        bestTile = resourcePos;
        float bestDist = float.MaxValue;

        for (int d = 0; d < 4; d++)
        {
            int2 dir = d == 0 ? dir0 : d == 1 ? dir1 : d == 2 ? dir2 : dir3;
            int2 neighbor = resourcePos + dir;
            // Check bounds
            if (neighbor.x < 0 || neighbor.y < 0 || neighbor.x >= width || neighbor.y >= height)
                continue;
            // Check if walkable (assuming 0 = walkable, 1 = blocked)
            int idx = neighbor.x + neighbor.y * width;
            if (walkableMapArray[idx] == 0)
            {
                float dist = math.distance(unitPos, neighbor);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestTile = neighbor;
                }
            }
        }

    }

    private void DecreseResourceAmount(ref SystemState state, in int2 targetResourceLocation, ref EntityCommandBuffer ecb, ResourceType resourceTypeRequested)
    {
        Entity searchedEntity;
        int2[] directions = new int2[]
        {
        new int2(0, -1),   // up
        new int2(0, 1),  // down
        new int2(1, 0),  // left
        new int2(-1, 0)    // right
        };

        foreach ((
                    RefRW<LocalTransform> localTransformResource,
                    RefRW<ResourceData> resourceData,
                    Entity entity
                    )
                    in SystemAPI.Query<
                    RefRW<LocalTransform>,
                    RefRW<ResourceData>>().WithEntityAccess())
        {
            float3 ResourcePosition = localTransformResource.ValueRO.Position;
            int2 resourcePos = new int2(Mathf.FloorToInt(ResourcePosition.x + 0.5f), Mathf.FloorToInt(ResourcePosition.y + 0.5f));

            for (int i = 0; i < 4; i++)
            {
                int2 searchResource = targetResourceLocation + directions[i];
                if (resourcePos.Equals(searchResource) && resourceTypeRequested == resourceData.ValueRO.resourceType)
                {
                    searchedEntity = entity;
                    resourceData.ValueRW.resourceAmmount -= 10;
                    if (resourceData.ValueRO.resourceAmmount == 0)
                    {
                        ecb.DestroyEntity(entity);
                        WalkableManager.Instance.UpdateWalkableMap(searchResource.x, searchResource.y, 0); 
                    }
                    return;
                }
            }


        }


    }


}




