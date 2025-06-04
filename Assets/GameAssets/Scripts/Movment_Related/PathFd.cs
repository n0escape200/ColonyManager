using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.PlayerLoop;



public partial struct PathFinding : ISystem
{
    //A* ALGORITM

    private const int MOVE_STRAIT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;


    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (
        var (pathfindingParams, pathPositionBuffer, entity)
        in SystemAPI.Query<RefRO<PathfindingParams>,
                    DynamicBuffer<PathPosition>>()
                    .WithEntityAccess())
        {
            Debug.Log("Find path");

            var path = new NativeList<int2>(Allocator.TempJob);

            //initialize walkabel map
            var walkableMap = WalkableManager.Instance.GetWalkableMap();
            var width = WalkableManager.Instance.GetWidth();
            var height = WalkableManager.Instance.Getheight();

            NativeArray<int> walkableMapArray = new NativeArray<int>(walkableMap.Length, Allocator.TempJob);
            for (int i = 0; i < walkableMap.GetLength(0); i++)
            {
                for (int j = 0; j < walkableMap.GetLength(1); j++)
                {
                    walkableMapArray[i + j * width] = walkableMap[i, j]; // Adjust based on your grid width
                }
            }


            FindPathJob findPathJob = new FindPathJob
            {
                startPosition = pathfindingParams.ValueRO.startPosition,
                endPosition = pathfindingParams.ValueRO.endPosition,
                resultPath = path,
                walkableMap = walkableMapArray,
                width = width,
                height = height,
            };

            findPathJob.Run();

            pathPositionBuffer.Clear();
            for (int i = path.Length - 1; i >= 0; i--) // Reverse order: start to end
            {
                pathPositionBuffer.Add(new PathPosition { position = path[i] });
            }

            path.Dispose();
            walkableMapArray.Dispose();

            ecb.RemoveComponent<PathfindingParams>(entity);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }


    [BurstCompile]
    private struct FindPathJob : IJob
    {
        public int2 startPosition;
        public int2 endPosition;
        public NativeArray<int> walkableMap;

        public int width;
        public int height;


        public NativeList<int2> resultPath;


        public void Execute()
        {
            int2 gridSize = new int2(width, height);

            NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PathNode pathNode = new PathNode();
                    pathNode.x = x;
                    pathNode.y = y;
                    pathNode.index = CalculateIndex(x, y, gridSize.x);

                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                    pathNode.fCost = CalculateFCost(pathNode.gCost, pathNode.hCost);

                    pathNode.isWalkable = walkableMap[x + y * width] == 0;
                    pathNode.cameFromNodeIndex = -1;

                    pathNodeArray[pathNode.index] = pathNode;
                }
            }

            //test walls (the code is no longer useful)
            //{
            //    PathNode walkablePathNode = pathNodeArray[CalculateIndex(1, 0, gridSize.x)];
            //    walkablePathNode.SetIsWalkable(false);
            //    pathNodeArray[CalculateIndex(1, 0, gridSize.x)] = walkablePathNode;

            //    walkablePathNode = pathNodeArray[CalculateIndex(1, 1, gridSize.x)];
            //    walkablePathNode.SetIsWalkable(false);
            //    pathNodeArray[CalculateIndex(1, 1, gridSize.x)] = walkablePathNode;

            //    walkablePathNode = pathNodeArray[CalculateIndex(1, 2, gridSize.x)];
            //    walkablePathNode.SetIsWalkable(false);
            //    pathNodeArray[CalculateIndex(1, 2, gridSize.x)] = walkablePathNode;

            //}
            NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
            {
                neighbourOffsetArray[0] = new int2(-1, 0);   //left
                neighbourOffsetArray[1] = new int2(+1, 0);   //right    
                neighbourOffsetArray[2] = new int2(0, +1);   //up
                neighbourOffsetArray[3] = new int2(0, -1);   //down
                neighbourOffsetArray[4] = new int2(-1, -1);  //left down
                neighbourOffsetArray[5] = new int2(-1, +1);  //left up
                neighbourOffsetArray[6] = new int2(+1, -1);  //right down
                neighbourOffsetArray[7] = new int2(+1, +1);  //right up
            }
            int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);
            PathNode endNode = pathNodeArray[endNodeIndex];


            PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.fCost = CalculateFCost(startNode.gCost, startNode.hCost);
            pathNodeArray[startNode.index] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closeList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.index);

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
                PathNode currentNode = pathNodeArray[currentNodeIndex];

                if (currentNodeIndex == endNodeIndex || !endNode.isWalkable) // cheks if the endpoint is reached or if its not walkable
                {
                    //reached destination
                    break;
                }
                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closeList.Add(currentNodeIndex);

                for (int i = 0; i < neighbourOffsetArray.Length; i++)
                {
                    int2 neighbourOffset = neighbourOffsetArray[i];
                    int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                    if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                    {
                        continue;   //not a valid position
                    }
                    int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);
                    if (closeList.Contains(neighbourNodeIndex))
                    {
                        continue;   //already searched
                    }

                    PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                    if (!neighbourNode.isWalkable)
                    {
                        continue;   //not walkable
                    }


                    //More testing needs to be done
                    if (math.abs(neighbourOffset.x) == 1 && math.abs(neighbourOffset.y) == 1) // Diagonal movement
                    {
                        int2 horizontalNeighborPos = new int2(currentNode.x + neighbourOffset.x, currentNode.y);
                        int2 verticalNeighborPos = new int2(currentNode.x, currentNode.y + neighbourOffset.y);

                        if (!IsPositionInsideGrid(horizontalNeighborPos, gridSize) ||
                            !IsPositionInsideGrid(verticalNeighborPos, gridSize))
                        {
                            continue; // Out of bounds
                        }

                        int horizontalNeighborIndex = CalculateIndex(horizontalNeighborPos.x, horizontalNeighborPos.y, gridSize.x);
                        int verticalNeighborIndex = CalculateIndex(verticalNeighborPos.x, verticalNeighborPos.y, gridSize.x);

                        if (!pathNodeArray[horizontalNeighborIndex].isWalkable || !pathNodeArray[verticalNeighborIndex].isWalkable)
                        {
                            continue; // Block diagonal move if either adjacent tile is non-walkable
                        }
                    }


                    int2 currentNodePosition = new int2(currentNode.x, currentNode.y);
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNodeIndex = currentNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.fCost = CalculateFCost(neighbourNode.gCost, neighbourNode.hCost);
                        pathNodeArray[neighbourNodeIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNode.index))
                        {
                            openList.Add(neighbourNode.index);
                        }
                    }
                }
            }

            endNode = pathNodeArray[endNodeIndex];
            if (endNode.cameFromNodeIndex == -1)
            {
                //no pathh
                //Debug.Log("NO PATH FOUND");
            }
            else
            {
                //path

                NativeList<int2> path = CalculatePath(pathNodeArray, endNode);

                foreach (int2 pathPosition in path)
                {
                    resultPath.Add(pathPosition);
                }

                path.Dispose();
            }

            neighbourOffsetArray.Dispose();
            pathNodeArray.Dispose();
            openList.Dispose();
            closeList.Dispose();

        }

        private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
        {
            if (endNode.cameFromNodeIndex == -1)
            {
                return new NativeList<int2>(Allocator.TempJob);
            }
            else
            {
                //found a path
                NativeList<int2> path = new NativeList<int2>(Allocator.TempJob);
                path.Add(new int2(endNode.x, endNode.y));

                PathNode currentNode = endNode;
                while (currentNode.cameFromNodeIndex != -1)
                {
                    PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                    path.Add(new int2(cameFromNode.x, cameFromNode.y));
                    currentNode = cameFromNode;
                }

                return path;
            }
        }

        private bool IsPositionInsideGrid(int2 gridPosition, int2 gridsize)
        {
            return
                gridPosition.x >= 0 &&
                gridPosition.y >= 0 &&
                gridPosition.x < gridsize.x &&
                gridPosition.y < gridsize.y;
        }

        private int CalculateIndex(int x, int y, int gridWidth)
        {
            return x + y * gridWidth;
        }
        private int CalculateDistanceCost(int2 aPosition, int2 bPosition)
        {
            int xDistance = math.abs(aPosition.x - bPosition.x);
            int yDistance = math.abs(aPosition.y - bPosition.y);
            int remaining = math.abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIT_COST * remaining;
        }

        private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
        {
            PathNode lowestCostPathNode = pathNodeArray[openList[0]];
            for (int i = 1; i < openList.Length; i++)
            {
                PathNode testPathNode = pathNodeArray[openList[i]];
                if (testPathNode.hCost < lowestCostPathNode.hCost)
                {
                    lowestCostPathNode = testPathNode;
                }
            }
            return lowestCostPathNode.index;
        }

        private int CalculateFCost(int gCost, int hCost)
        {
            return gCost + hCost;
        }



        private struct PathNode
        {
            public int x;
            public int y;

            public int index;

            public int gCost;
            public int hCost;
            public int fCost;

            public bool isWalkable;

            public int cameFromNodeIndex;

            public void SetIsWalkable(bool isWalkable)
            {
                this.isWalkable = isWalkable;
            }


        }
    }
}